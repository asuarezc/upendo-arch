using System;
using System.Collections.Generic;

namespace upendo.CrossCutting.Interfaces.Data.LocalCache
{
    /// <summary>
    /// Non generic memory cache
    /// </summary>
    public interface IMemoryCache : IDisposable
    {
        /// <summary>
        /// Raises when size limit is reached
        /// </summary>
        event EventHandler SizeLimitReached;

        /// <summary>
        /// Current used size in bytes
        /// </summary>
        long CurrentSizeInBytes { get; }

        /// <summary>
        /// Configuration of this cache instance
        /// </summary>
        IMemoryCacheConfiguration Configuration { get; }

        /// <summary>
        /// Type of stored items
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Removes all stored items
        /// </summary>
        void Clear();

        /// <summary>
        /// Checks if this instance containes a cache item with provided <paramref name="key"/>
        /// </summary>
        /// <param name="key">Key to check if is contained</param>
        /// <returns>True if provided <paramref name="key"/> is contained, otherwise false</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Removes a item by provided <paramref name="key"/> if is contained
        /// </summary>
        /// <param name="key">Key of item to remove</param>
        void Remove(string key);

        /// <summary>
        /// Remove all outdated items
        /// </summary>
        void ForceFlush();
    }

    /// <summary>
    /// Generic memory cache
    /// </summary>
    /// <typeparam name="T">Generic type of memory cache</typeparam>
    public interface IMemoryCache<T> : IMemoryCache
    {
        /// <summary>
        /// Adds <paramref name="item"/> if there is no item with provided <paramref name="key"/> of if is outdated
        /// If there is a not outdated item with provided <paramref name="key"/>, it will update contained item with <paramref name="item"/>
        /// and it will reset item expiration
        /// </summary>
        /// <param name="key">Key to add or update <paramref name="item"/></param>
        /// <param name="item">Item to add or update</param>
        /// <param name="expirationTime"><paramref name="item"/> expiration time</param>
        void AddOrUpdate(string key, T item, TimeSpan expirationTime);

        /// <summary>
        /// Gets a contained cache item by a provided <paramref name="key"/>
        /// </summary>
        /// <param name="key">Key to obtain a contained item</param>
        /// <returns>Item if it is contained, null if not or if is outdated</returns>
        T Get(string key);

        /// <summary>
        /// Gets all not outdated items
        /// </summary>
        IEnumerable<T> GetAll();
    }
}
