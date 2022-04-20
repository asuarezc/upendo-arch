using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using upendo.CrossCutting.Interfaces.Data.LocalCache;

namespace upendo.Services.Data.LocalCache
{
    public class MemoryCacheFactory : IMemoryCacheFactory
    {
        private static readonly object lockObject = new();
        private IDictionary<string, IMemoryCache> internalCaches;
        private Task pollingTask;
        private CancellationTokenSource cancellationTokenSource;
        private TimeSpan? pollingInterval;
        private bool disposedValue;

        public TimeSpan? PollingInterval => pollingInterval;

        public bool IsPolling { get; private set; }

        public MemoryCacheFactory()
        {
            if (pollingInterval != null && pollingInterval.Value <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(pollingInterval), $"{nameof(pollingInterval)} must be greater than {TimeSpan.Zero}.");

            internalCaches = new Dictionary<string, IMemoryCache>();
            pollingInterval = TimeSpan.FromMinutes(5);

            if (this.pollingInterval.HasValue)
                StartPolling(this.pollingInterval.Value);
        }

        public void StartPolling(TimeSpan pollingInterval)
        {
            if (pollingInterval <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(pollingInterval), $"{nameof(pollingInterval)} must be greater than {TimeSpan.Zero}.");

            if (pollingTask != null)
                throw new InvalidOperationException("You must stop polling first");

            this.pollingInterval = pollingInterval;
            cancellationTokenSource = new();
            pollingTask = DoPollingAsync(cancellationTokenSource.Token);
            IsPolling = true;
        }

        public void StopPolling()
        {
            if (pollingTask == null)
                throw new InvalidOperationException("You must start polling first");

            cancellationTokenSource.Cancel();

            if (!pollingTask.IsCompleted)
                pollingTask.Wait();

            pollingTask?.Dispose();
            cancellationTokenSource?.Dispose();
            pollingInterval = null;
            IsPolling = false;
        }

        public bool ExitsCache(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            lock (lockObject)
                return internalCaches != null && internalCaches.ContainsKey(key) && internalCaches[key] != null;
        }

        public IMemoryCache<T> GetCache<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            lock (lockObject)
            {
                if (internalCaches == null)
                    return null;

                IMemoryCache memoryCache = internalCaches.ContainsKey(key)
                    ? internalCaches[key]
                    : null;

                if (memoryCache == null)
                    return null;

                if (memoryCache.Type.FullName != typeof(T).FullName)
                    return null;

                return memoryCache as IMemoryCache<T>;
            }
        }

        public IMemoryCache<T> GetOrCreateCache<T>(string key, long sizeLimitInBytes, float oldestItemsRemovingPercentage = 0.2f, bool resetItemExpirationTimeOnGetted = false)
        {
            IMemoryCache<T> memoryCache = GetCache<T>(key);

            if (memoryCache != null)
                return memoryCache;

            lock (lockObject)
            {
                IMemoryCacheConfiguration memoryCacheConfiguration = new MemoryCacheConfiguration(
                    sizeLimitInBytes,
                    oldestItemsRemovingPercentage,
                    resetItemExpirationTimeOnGetted
                );

                memoryCache = new MemoryCache<T>(memoryCacheConfiguration);
                internalCaches.Add(key, memoryCache);

                return memoryCache;
            }
        }

        public void RemoveCache(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            lock (lockObject)
            {
                if (internalCaches == null || !internalCaches.ContainsKey(key))
                    return;

                IMemoryCache memoryCache = internalCaches[key];

                internalCaches.Remove(key);
                memoryCache?.Dispose();
            }
        }

        public void RemoveAllCaches()
        {
            lock (lockObject)
            {
                if (internalCaches == null || !internalCaches.Any())
                    return;

                foreach (KeyValuePair<string, IMemoryCache> memoryCache in internalCaches)
                {
                    IMemoryCache current = memoryCache.Value;

                    internalCaches.Remove(memoryCache.Key);
                    current?.Dispose();
                }
            }
        }

        private async Task DoPollingAsync(CancellationToken cancellationToken)
        {
            while(true)
            {
                if (cancellationToken.IsCancellationRequested || PollingInterval == null)
                    return;

                await Task.Delay(PollingInterval.Value);

                lock (lockObject)
                {
                    if (internalCaches == null || !internalCaches.Any())
                        continue;

                    foreach (IMemoryCache memoryCache in internalCaches.Values)
                        memoryCache?.ForceFlush();
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
            {
                if (pollingTask != null && !pollingTask.IsCompleted)
                {
                    Task.Run(() =>
                    {
                        cancellationTokenSource?.Cancel();
                        pollingTask.Wait();
                        pollingTask?.Dispose();
                    }).ContinueWith((Task task) =>
                    {
                        RemoveAllCaches();
                    });
                }
                else
                {
                    RemoveAllCaches();
                }
            }

            internalCaches = null;
            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
