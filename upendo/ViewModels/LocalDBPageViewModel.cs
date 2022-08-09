using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DryIoc;
using upendo.CrossCutting.Entities;
using upendo.CrossCutting.Interfaces.Logic;
using upendo.CrossCutting.Localization;
using upendo.Models;
using Xamarin.Forms;

namespace upendo.ViewModels
{
    public class LocalDBPageViewModel : BaseViewModel<LocalDBPageModel>
    {
        public override string Title => GetResource(StringKey.menu_local_database);

        private Command addConctactCommand;
        public Command AddConctactCommand => addConctactCommand ??= new Command(
            async () => await AddContactAsync(),
            () => IsNotBusy
        );

        public override async Task<LocalDBPageModel> GetInitialModelAsync()
        {
            LocalDBPageModel model = new();

            await RunUnitOfWorkAsync(
                job: (IResolverContext scope) =>
                {
                    IContactsService contactsService = scope.Resolve<IContactsService>();
                    return contactsService.GetStoredContacts();
                },
                onSuccess: (IEnumerable<Contact> contacts) =>
                {
                    model.Contacts = contacts != null && contacts.Any()
                        ? new(from contact in contacts select new ContactModel(contact))
                        : new();
                }
            );

            return model;
        }

        private async Task AddContactAsync()
        {
            await RunUnitOfWorkAsync(
                job: (IResolverContext scope) =>
                {
                    if (string.IsNullOrEmpty(Model.AddingContactName) || string.IsNullOrEmpty(Model.AddingContactPhone))
                    {
                        ShowStatusMessage(GetResource(StringKey.global_enter_name_and_phone));
                        return null;
                    }

                    Contact newContact = new()
                    {
                        Name = Model.AddingContactName,
                        Phone = Model.AddingContactPhone
                    };

                    IContactsService contactsService = scope.Resolve<IContactsService>();
                    contactsService.AddContact(newContact);

                    return new ContactModel(newContact);
                },
                onSuccess: (ContactModel contact) =>
                {
                    Model.Contacts.Add(contact);
                    Model.AddingContactName = string.Empty;
                    Model.AddingContactPhone = string.Empty;

                    ShowStatusMessage(GetResource(StringKey.global_i_will_remember_it));
                }
            );
        }
    }
}
