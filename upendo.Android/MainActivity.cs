using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using upendo.Managers;
using upendo.Droid.Services;
using upendo.CrossCutting.Interfaces.Data.LocalDB;
using Xamarin.Essentials;

namespace upendo.Droid
{
    [Activity(Label = "upendo", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            RegisterNativeTypes();
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void RegisterNativeTypes()
        {
            DependencyServiceManager.Instance.Register<IConnectionStringProviderForLocalDB, ConnectionStringProviderForLocalDB>();
        }
    }
}
