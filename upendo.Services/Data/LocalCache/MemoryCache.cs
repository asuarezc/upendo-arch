using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using upendo.CrossCutting.Interfaces.Data.LocalCache;

namespace upendo.Services.Data.LocalCache
{
    public class MemoryCache<T> : IMemoryCache<T>
    {
        private static readonly int objectSize = IntPtr.Size == 8 ? 24 : 12;
        private static readonly int pointerSize = IntPtr.Size;

        private readonly ReaderWriterLockSlim locker;
        private IList<CacheItem<T>> internalCache;
        private ISet<int> alreadyConsumedObjects;
        private long currentSize; //Must be invoked into a read lock
        private bool disposedValue;

        public event EventHandler SizeLimitReached;

        public long CurrentSizeInBytes
        {
            get
            {
                locker.EnterReadLock();

                try
                {
                    return currentSize;
                }
                finally
                {
                    locker.ExitReadLock();
                }
            }
        }

        public IMemoryCacheConfiguration Configuration { get; private set; }
        public Type Type { get; private set; }

        public MemoryCache(IMemoryCacheConfiguration configuration)
        {
            internalCache = new List<CacheItem<T>>();
            locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Type = typeof(T);
        }

        public bool ContainsKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            CacheItem<T> outDatedItem = null;
            locker.EnterReadLock();

            try
            {
                if (internalCache == null || !internalCache.Any())
                    return false;

                CacheItem<T> cacheItem = internalCache.SingleOrDefault(it => it.Key == key);

                if (cacheItem == null)
                    return false;

                if (!cacheItem.IsOutdated)
                    return true;
                else
                    outDatedItem = cacheItem;
            }
            finally
            {
                locker.ExitReadLock();
            }

            locker.EnterWriteLock();

            try
            {
                Delete(outDatedItem);
            }
            finally
            {
                locker.ExitWriteLock();
            }

            return false;
        }

        public T Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            CacheItem<T> outDatedItem = null;
            locker.EnterReadLock();

            try
            {
                if (internalCache == null || !internalCache.Any())
                    return default;

                CacheItem<T> cacheItem = internalCache.SingleOrDefault(it => it.Key == key);

                if (cacheItem == null)
                    return default;

                if (!cacheItem.IsOutdated)
                {
                    if (Configuration.ResetItemExpirationTimeOnGetted)
                        cacheItem.UTCDateAdded = DateTime.UtcNow;

                    return cacheItem.Item;
                }
                else
                    outDatedItem = cacheItem;
            }
            finally
            {
                locker.ExitReadLock();
            }

            locker.EnterWriteLock();

            try
            {
                Delete(outDatedItem);
            }
            finally
            {
                locker.ExitWriteLock();
            }

            return default;
        }

        public IEnumerable<T> GetAll()
        {
            locker.EnterUpgradeableReadLock();

            try
            {
                FlushOutdatedItems();

                if (internalCache == null || !internalCache.Any())
                    return null;

                return internalCache.Select(it => it.Item);
            }
            finally
            {
                locker.ExitUpgradeableReadLock();
            }
        }

        public void AddOrUpdate(string key, T item, TimeSpan expirationTime)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (expirationTime <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(expirationTime), $"{nameof(expirationTime)} must be greater than {TimeSpan.Zero}");

            locker.EnterUpgradeableReadLock();

            try
            {
                CacheItem<T> cacheItem = internalCache.SingleOrDefault(it => it.Key == key);

                if (cacheItem == null)
                    Add(key, item, expirationTime);
                else
                {
                    if (!cacheItem.IsOutdated)
                        Update(key, item, expirationTime);
                    else
                    {
                        Delete(cacheItem);
                        Add(key, item, expirationTime);
                    }
                }
            }
            finally
            {
                locker.ExitUpgradeableReadLock();
            }
        }

        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            locker.EnterUpgradeableReadLock();

            try
            {
                CacheItem<T> cacheItem = internalCache.SingleOrDefault(it => it.Key == key);

                if (cacheItem == null)
                    return;

                Delete(cacheItem);
            }
            finally
            {
                locker.ExitUpgradeableReadLock();
            }
        }

        public void Clear()
        {
            locker.EnterUpgradeableReadLock();

            try
            {
                RemoveAll();
            }
            finally
            {
                locker.ExitUpgradeableReadLock();
            }
        }

        public void ForceFlush()
        {
            locker.EnterUpgradeableReadLock();

            try
            {
                FlushOutdatedItems();
            }
            finally
            {
                locker.ExitUpgradeableReadLock();
            }
        }

        //Must be invoked into upgradeable read lock
        private void FlushOutdatedItems()
        {
            if (internalCache == null || !internalCache.Any())
                return;

            IEnumerable<CacheItem<T>> itemsToBeRemoved = internalCache.Where(it => it.IsOutdated);

            if (itemsToBeRemoved == null || !itemsToBeRemoved.Any())
                return;

            itemsToBeRemoved.ToList().ForEach(it => Delete(it));
        }

        //Must be invoked into a upgradeable read lock
        private void FlushOldestItems()
        {
            if (internalCache == null || !internalCache.Any())
                return;

            int numberOfItemsToBeRemoved = (int)Math.Truncate(
                internalCache.Count * Configuration.ItemsRemovingPercentage
            );

            //At least one element must always be removed
            if (numberOfItemsToBeRemoved == 0)
                Delete(internalCache.OrderBy(it => it.UTCDateAdded).First());
            else
            {
                foreach (CacheItem<T> cacheItem in internalCache.OrderBy(it => it.UTCDateAdded).Take(numberOfItemsToBeRemoved).ToList())
                    Delete(cacheItem);
            }
        }

        //Must be invoked into a upgradeable read lock
        private void RemoveAll()
        {
            if (!internalCache.Any())
                return;

            IEnumerable<CacheItem<T>> idisposableItems = internalCache.Where(
                it => it != null && it.Item != null && it.Item is IDisposable
            );

            if (idisposableItems != null && idisposableItems.Any())
            {
                foreach (IDisposable item in idisposableItems.Select(it => it.Item))
                    item.Dispose();
            }

            internalCache.Clear();
            currentSize = 0;
        }

        //Must be invoked into a upgradeable read lock
        private void Add(string key, T item, TimeSpan expirationTime)
        {
            long size = GetObjectMemorySize(item);

            if ((currentSize + size) >= Configuration.SizeLimitInBytes)
            {
                SizeLimitReached?.Invoke(this, EventArgs.Empty);
                FlushOutdatedItems();
            }

            while ((currentSize + size) >= Configuration.SizeLimitInBytes)
                FlushOldestItems();

            CacheItem<T> cacheItem = new(key, item, size, expirationTime);

            internalCache.Add(cacheItem);
            currentSize += cacheItem.Size;
        }

        //Must be invoked into a upgradeable read lock
        private void Update(string key, T item, TimeSpan expirationTime)
        {
            long size = GetObjectMemorySize(item);
            CacheItem<T> cacheItem = internalCache.SingleOrDefault(it => it.Key == key);

            if ((currentSize - cacheItem.Size + size) >= Configuration.SizeLimitInBytes)
            {
                SizeLimitReached?.Invoke(this, EventArgs.Empty);
                FlushOutdatedItems();
            }  

            while ((currentSize - cacheItem.Size + size) >= Configuration.SizeLimitInBytes)
                FlushOldestItems();

            currentSize -= cacheItem.Size;

            cacheItem.Item = item;
            cacheItem.Size = size;
            cacheItem.ExpirationTime = expirationTime;

            currentSize += cacheItem.Size;
        }

        //Must be invoked into a write lock or upgradeable read lock
        private void Delete(CacheItem<T> cacheItem)
        {
            if (cacheItem == null)
                return;

            if (cacheItem.Item != null && cacheItem.Item is IDisposable disposable)
                disposable.Dispose();

            internalCache.Remove(cacheItem);
            currentSize -= cacheItem.Size;
        }

        private long GetObjectMemorySize(object item)
        {
            alreadyConsumedObjects = new HashSet<int>();
            return GetObjectMemorySizeInternal(item);
        }

        private long GetObjectMemorySizeInternal(object item)
        {
            long memorySize = 0;
            Type objType = item.GetType();

            if (objType.IsValueType)
            {
                memorySize = Marshal.SizeOf(item);
            }
            else if (objType.Equals(typeof(string)))
            {
                string str = (string)item;

                memorySize = str.Length * 2 + 6 + objectSize;
            }
            else if (objType.IsArray)
            {
                Array arr = (Array)item;
                Type elementType = objType.GetElementType();

                if (elementType.IsValueType)
                {
                    long elementSize = Marshal.SizeOf(elementType);
                    long elementCount = arr.LongLength;

                    memorySize += elementSize * elementCount;
                }
                else
                {
                    foreach (object element in arr)
                        memorySize += element != null ? GetObjectMemorySizeInternal(element) + pointerSize : pointerSize;
                }

                memorySize += objectSize;
            }
            else if (item is IEnumerable enumerable)
            {
                foreach (object subItem in enumerable)
                {
                    Type itemType = subItem.GetType();

                    memorySize += subItem != null ? GetObjectMemorySizeInternal(subItem) : 0;
                    if (itemType.IsClass)
                        memorySize += pointerSize;
                }

                memorySize += objectSize;
            }
            else if (objType.IsClass)
            {
                int hashCode = objType.GetHashCode();

                if (!alreadyConsumedObjects.Contains(hashCode))
                {
                    alreadyConsumedObjects.Add(hashCode);
                    PropertyInfo[] properties = objType.GetProperties();

                    foreach (PropertyInfo property in properties)
                    {
                        object valueObject = property.GetValue(item);

                        memorySize += valueObject != null ? GetObjectMemorySizeInternal(valueObject) : 0;

                        if (property.GetType().IsClass)
                            memorySize += pointerSize;
                    }

                    memorySize += objectSize;
                }
            }

            return memorySize;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
            {
                Clear();
                locker?.Dispose();
                alreadyConsumedObjects?.Clear();
            }

            internalCache = null;
            SizeLimitReached = null;
            alreadyConsumedObjects = null;
            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private class CacheItem<K>
        {
            public string Key { get; set; }

            public K Item { get; set; }

            public long Size { get; set; }

            public DateTime UTCDateAdded { get; set; }

            public TimeSpan ExpirationTime { get; set; }

            public bool IsOutdated => DateTime.UtcNow - UTCDateAdded > ExpirationTime;

            public CacheItem(string key, K item, long size, TimeSpan expirationTime)
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException(nameof(key));

                if (item == null)
                    throw new ArgumentNullException(nameof(item));

                Key = key;
                Item = item;
                ExpirationTime = expirationTime;
                Size = size;
                UTCDateAdded = DateTime.UtcNow;
            }
        }
    }
}