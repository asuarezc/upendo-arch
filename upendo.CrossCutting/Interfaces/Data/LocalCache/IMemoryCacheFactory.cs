using System;

namespace upendo.CrossCutting.Interfaces.Data.LocalCache
{
    /// <summary>
    /// Factory to group memory caches by one polling thread, if StartPolling is called
    /// </summary>
    public interface IMemoryCacheFactory : IDisposable
    {
        /// <summary>
        /// Interval of time between all stored caches are flushed. Null if no polling is applied
        /// </summary>
        TimeSpan? PollingInterval { get; }

        /// <summary>
        /// Returns true if current factory is polling, otherwise returns false
        /// </summary>
        bool IsPolling { get; }

        /// <summary>
        /// Returns true if a cache with provided <paramref name="key"/> exits and it is not null
        /// </summary>
        bool ExitsCache(string key);

        /// <summary>
        /// Returns a stored cache if exits by provided <paramref name="key"/> and it is not null
        /// </summary>
        /// <typeparam name="T">Generic type of cache</typeparam>
        IMemoryCache<T> GetCache<T>(string key);

        /// <summary>
        /// Returns a stored cache if exits by provided <paramref name="key"/> and it is not null.
        /// Otherwise, creates and returns a new cache with provided <paramref name="sizeLimitInBytes"/> and <paramref name="oldestItemsRemovingPercentage"/>
        /// </summary>
        /// <typeparam name="T">Generic type of cache</typeparam>
        IMemoryCache<T> GetOrCreateCache<T>(string key, long sizeLimitInBytes, float oldestItemsRemovingPercentage = 0.2f, bool resetItemExpirationTimeOnGetted = false);

        /// <summary>
        /// Removes a stored cache if exits by provided <paramref name="key"/>
        /// </summary>
        void RemoveCache(string key);

        /// <summary>
        /// Removes all stored caches
        /// </summary>
        void RemoveAllCaches();

        /// <summary>
        /// Starts polling to remove outdated items from all stored caches every <paramref name="pollingInterval"/>
        /// </summary>
        void StartPolling(TimeSpan pollingInterval);

        /// <summary>
        /// Stops polling
        /// </summary>
        void StopPolling();
    }
}
