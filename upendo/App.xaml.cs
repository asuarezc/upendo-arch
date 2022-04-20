using System;
using DryIoc;
using upendo.ContentPages;
using upendo.CrossCutting.Interfaces.Data.LocalDB;
using upendo.CrossCutting.Localization;
using upendo.Helpers;
using upendo.Managers;
using Xamarin.Forms;

namespace upendo
{
    public partial class App : Application
    {
        private readonly ITranslationManager translationManager;
        private readonly IConnectionStringProviderForLocalDB connectionStringProviderForLocalDB;
        private readonly ILocalDBProvider localDBProvider;

        public App()
        {
            InitializeComponent();

            translationManager = DependencyServiceManager.Instance.Container.Resolve<ITranslationManager>();
            connectionStringProviderForLocalDB = DependencyServiceManager.Instance.Container.Resolve<IConnectionStringProviderForLocalDB>();
            localDBProvider = DependencyServiceManager.Instance.Container.Resolve<ILocalDBProvider>();

            translationManager.SetLanguage(LanguageHelper.CurrentLanguage);
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            ReOpenDatabase();
        }

        protected override void OnSleep()
        {
            DisposeDatabase();
        }

        protected override void OnResume()
        {
            ReOpenDatabase();
        }

        private void ReOpenDatabase()
        {
            localDBProvider.InitDatabase(connectionStringProviderForLocalDB.GetConnectionStringForLocalDB());
        }

        private void DisposeDatabase()
        {
            localDBProvider.DisposeDatabase();
        }
    }
}
