using System;
using upendo.CrossCutting.Interfaces.Data.LocalCache;

namespace upendo.Services.Data.LocalCache
{
    public class MemoryCacheConfiguration : IMemoryCacheConfiguration
    {
        public long SizeLimitInBytes { get; private set; }

        public float ItemsRemovingPercentage { get; private set; }

        public bool ResetItemExpirationTimeOnGetted { get; private set; }

        public MemoryCacheConfiguration(
            long sizeLimitInBytes,
            float oldestItemsRemovingPercentage = 0.2f,
            bool resetItemExpirationTimeOnGetted = false)
        {
            if (sizeLimitInBytes <= 0)
                throw new ArgumentOutOfRangeException(nameof(sizeLimitInBytes), $"{nameof(sizeLimitInBytes)} must be greater than zero");

            if (oldestItemsRemovingPercentage <= 0f || oldestItemsRemovingPercentage >= 1f)
                throw new ArgumentOutOfRangeException(nameof(oldestItemsRemovingPercentage), $"{nameof(oldestItemsRemovingPercentage)} must be any value between 0 and 1");

            SizeLimitInBytes = sizeLimitInBytes;
            ItemsRemovingPercentage = oldestItemsRemovingPercentage;
            ResetItemExpirationTimeOnGetted = resetItemExpirationTimeOnGetted;
        }
    }
}
