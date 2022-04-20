using System;
using System.Collections.Generic;
using upendo.CrossCutting.Interfaces.Data.LocalCache;
using upendo.CrossCutting.Interfaces.Logic;

namespace upendo.Services.Logic
{
    public class LocalCachePageService : ILocalCachePageService
    {
        private static readonly string cacheName = "testCache";
        private readonly IMemoryCache<string> cache;

        public LocalCachePageService(IMemoryCacheFactory memoryCacheFactory)
        {
            if (memoryCacheFactory == null)
                throw new ArgumentNullException(nameof(memoryCacheFactory));

            cache = memoryCacheFactory.GetOrCreateCache<string>(cacheName, 102400);
        }

        public IEnumerable<string> GetAllStrings()
        {
            return cache.GetAll();
        }

        public void AddString(string item, TimeSpan expiration)
        {
            if (string.IsNullOrEmpty(item))
                throw new ArgumentNullException(nameof(item));

            if (cache.ContainsKey(item))
                return;

            cache.AddOrUpdate(item, item, expiration);
        }
    }
}
