using System;

namespace upendo.CrossCutting.Interfaces.Data.LocalCache
{
    /// <summary>
    /// Cache configuration
    /// </summary>
    public interface IMemoryCacheConfiguration
    {
        /// <summary>
        /// Size limit in bytes
        /// </summary>
        long SizeLimitInBytes { get; }

        /// <summary>
        /// Removing perfentage of oldest items if size limit is reached and there is no updated items to flush
        /// </summary>
        float ItemsRemovingPercentage { get; }

        /// <summary>
        /// If true, when an item is getted, it's expiration time will be refreshed
        /// </summary>
        bool ResetItemExpirationTimeOnGetted { get; }
    }
}
