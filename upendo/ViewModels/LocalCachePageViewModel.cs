using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using DryIoc;
using upendo.CrossCutting.Interfaces.Logic;
using upendo.CrossCutting.Localization;
using upendo.Models;
using Xamarin.Forms;

namespace upendo.ViewModels
{
    public class LocalCachePageViewModel : BaseViewModel<LocalCachePageModel>
    {
        public override string Title => GetResource(StringKey.menu_local_memory_cache);

        private Command refreshCommand;
        public Command RefreshCommand => refreshCommand ??= new Command(
            async () => await ReloadModelAsync(),
            () => IsNotBusy
        );

        private Command addNewStringCommand;
        public Command AddNewStringCommand => addNewStringCommand ??= new Command(
            async () => await AddNewStringAsync(Model.NewString, TimeSpan.FromSeconds(Model.CurrentExpirationSeconds)),
            () => IsNotBusy
        );

        public override async Task<LocalCachePageModel> GetInitialModelAsync()
        {
            LocalCachePageModel model = new() { CurrentExpirationSeconds = 5 };

            await RunUnitOfWorkAsync(
                job: (IResolverContext scope) =>
                {
                    ILocalCachePageService localCachePageService = scope.Resolve<ILocalCachePageService>();
                    return localCachePageService.GetAllStrings();
                },
                onSuccess: (IEnumerable<string> strings) =>
                {
                    model.Strings = strings != null && strings.Any()
                        ? new(strings)
                        : new();
                }
            );

            return model;
        }

        private async Task AddNewStringAsync(string newString, TimeSpan expirationTime)
        {
            await RunUnitOfWorkAsync(
                job: (IResolverContext scope) =>
                {
                    ILocalCachePageService localCachePageService = scope.Resolve<ILocalCachePageService>();
                    localCachePageService.AddString(newString, expirationTime);
                },
                onSuccess: async () => await ReloadModelAsync()
            );
        }
    }
}
