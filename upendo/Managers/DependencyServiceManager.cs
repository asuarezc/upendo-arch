using System;
using System.Threading;
using DryIoc;
using upendo.CrossCutting.Interfaces.Data.CameraAndDeviceGallery;
using upendo.CrossCutting.Interfaces.Data.LocalCache;
using upendo.CrossCutting.Interfaces.Data.LocalDB;
using upendo.CrossCutting.Interfaces.Data.Location;
using upendo.CrossCutting.Interfaces.Data.Rest;
using upendo.CrossCutting.Interfaces.Logic;
using upendo.CrossCutting.Localization;
using upendo.Services.Data.CameraAndDeviceGallery;
using upendo.Services.Data.LocalCache;
using upendo.Services.Data.LocalDB;
using upendo.Services.Data.Location;
using upendo.Services.Data.Rest;
using upendo.Services.Logic;

namespace upendo.Managers
{
    public sealed class DependencyServiceManager
    {
        private static readonly Lazy<DependencyServiceManager> lazyInstance = new(
            () => new DependencyServiceManager(), LazyThreadSafetyMode.PublicationOnly
        );

        public static DependencyServiceManager Instance => lazyInstance.Value;

        private DependencyServiceManager()
        {
            Container = new Container(rules => rules.WithoutTrackingDisposableTransients());
            RegisterLogicTypes();
            RegisterDataTypes();
        }

        public IContainer Container { get; private set; }

        private void RegisterDataTypes()
        {
            Container.Register<ITranslationManager, TranslationManager>(Reuse.Singleton);
            Container.Register<ILocalDBProvider, LocalDBProvider>(Reuse.Singleton);
            Container.Register<IMemoryCacheFactory, MemoryCacheFactory>(Reuse.Singleton);
            Container.Register<IRestServiceFactory, RestServiceFactory>(Reuse.Singleton);

            Container.Register<ILocationService, LocationService>();
            Container.Register<IMediaService, MediaService>();
        }

        private void RegisterLogicTypes()
        {
            Container.Register<IMainPageService, MainPageService>();
            Container.Register<IRestPageService, RestPageService>();
            Container.Register<IContactsService, ContactsService>();
            Container.Register<ILocalCachePageService, LocalCachePageService>();
            Container.Register<ILocationPageService, LocationPageService>();
            Container.Register<ICameraAndGalleryPageService, CameraAndGalleryPageService>();
        }

        public void Register<TService, TImplementation>() where TImplementation : TService
        {
            Container.Register<TService, TImplementation>(Reuse.Singleton);
        }
    }
}
