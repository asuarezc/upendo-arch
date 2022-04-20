using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using DryIoc;
using upendo.CrossCutting.Entities;
using upendo.CrossCutting.Interfaces.Logic;
using upendo.CrossCutting.Localization;
using upendo.Models;
using Xamarin.Forms;

namespace upendo.ViewModels
{
    public class RestPageViewModel : BaseViewModel<RestPageModel>
    {
        public override string Title => GetResource(StringKey.menu_rest_client);

        private Command getUserCommand;
        public Command GetUserCommand => getUserCommand ??= new Command(
            async () => await GetUserAsync(),
            () => IsNotBusy
        );

        private Command resetUserCommand;
        public Command ResetUserCommand => resetUserCommand ??= new Command(
            () => ResetUser(),
            () => IsNotBusy
        );

        private async Task GetUserAsync()
        {
            await RunUnitOfWorkAsync(
                job: async (IResolverContext scope) =>
                {
                    IRestPageService restPageService = scope.Resolve<IRestPageService>();
                    return await restPageService.GetUsersAsync();
                },
                onSuccess: (IEnumerable<User> users) =>
                {
                    Model.Users = users != null && users.Any()
                        ? new(from user in users select new UserModel(user))
                        : new();
                }
            );
        }

        private void ResetUser()
        {
            Model.Users.Clear();
        }
    }
}
