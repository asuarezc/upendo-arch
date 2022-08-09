using Foundation;
using UIKit;
using upendo.CrossCutting.Interfaces.Data.LocalDB;
using upendo.CrossCutting.Localization;
using upendo.Helpers;
using upendo.iOS.Services;
using upendo.Managers;

namespace upendo.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            #if ENABLE_TEST_CLOUD
            Xamarin.Calabash.Start();
            #endif

            global::Xamarin.Forms.Forms.Init();

            RegisterNativeTypes();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        private void RegisterNativeTypes()
        {
            DependencyServiceManager.Instance.Register<IConnectionStringProviderForLocalDB, ConnectionStringProviderForLocalDB>();
        }
    }
}
