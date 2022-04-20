using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Essentials;

namespace upendo.Managers
{
    public class TrackingManager
    {
        public enum TrakingEvent
        {
            TheImpossibleHasHappened,
            MenuOptionTapped
        };

        private static readonly string androidSecret = "c5ebbf44-8ffc-4ef5-bb27-ae5c00311a70";
        private static readonly string iosSecret = "76c1d1ff-1429-4081-9a3a-cc4b6fc82bee";

        private static readonly Lazy<TrackingManager> lazyInstance = new(
            () => new TrackingManager(), LazyThreadSafetyMode.PublicationOnly
        );

        private TrackingManager()
        {
            AppCenter.Start(androidSecret + iosSecret, typeof(Analytics), typeof(Crashes));
        }

        public static TrackingManager Instance => lazyInstance.Value;

        public void TrackEvent(TrakingEvent eventName, IDictionary<string, string> moreInfoParameters = null)
        {
            IDictionary<string, string> defaultParameters = new Dictionary<string, string>
            {
                { "Platform", DeviceInfo.Platform.ToString() },
                { "OSVersion", DeviceInfo.VersionString },
                { "Manufacturer", DeviceInfo.Manufacturer },
                { "Model", DeviceInfo.Model },
                { "DeviceName", DeviceInfo.Name }
            };

            if (moreInfoParameters != null && moreInfoParameters.Any())
            {
                foreach (KeyValuePair<string, string> pair in moreInfoParameters)
                    defaultParameters.Add(pair);
            }

            Analytics.TrackEvent(eventName.ToString(), defaultParameters);
        }

        public void TrackException(
            Exception ex,
            string description = "",
            IDictionary<string, string> moreInfoParameters = null,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (ex == null)
                return;

            IDictionary<string, string> defaultParameters = new Dictionary<string, string>
            {
                { "Description", !string.IsNullOrEmpty(description) ? description : "None" },
                { "Platform", DeviceInfo.Platform.ToString() },
                { "OSVersion", DeviceInfo.VersionString },
                { "Manufacturer", DeviceInfo.Manufacturer },
                { "Model", DeviceInfo.Model },
                { "DeviceName", DeviceInfo.Name },
                { "InvokingMethod" , callerName },
                { "InvokingClassFilePath", callerFilePath },
                { "LoggedAtLine",  callerLineNumber.ToString() }
            };

            if (moreInfoParameters != null && moreInfoParameters.Any())
            {
                foreach (KeyValuePair<string, string> pair in moreInfoParameters)
                    defaultParameters.Add(pair);
            }

            Crashes.TrackError(ex, defaultParameters);
        }
    }
}
