using System;
using System.Collections.Generic;

namespace upendo.CrossCutting.Interfaces.Logic
{
    public interface ILocalCachePageService
    {
        IEnumerable<string> GetAllStrings();

        void AddString(string item, TimeSpan expiration);
    }
}
