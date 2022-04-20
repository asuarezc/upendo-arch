using System;
using upendo.Managers;

namespace upendo.Helpers
{
    public static class ExtensionMethods
    {
        public static void Log(this Exception ex, string description = null)
        {
            TrackingManager.Instance.TrackException(ex, description);
        }
    }
}
