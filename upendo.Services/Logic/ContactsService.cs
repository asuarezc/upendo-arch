using System;
using System.Collections.Generic;
using LiteDB;
using upendo.CrossCutting.Entities;
using upendo.CrossCutting.Interfaces.Data.LocalDB;
using upendo.CrossCutting.Interfaces.Logic;

namespace upendo.Services.Logic
{
    public class ContactsService : IContactsService
    {
        private readonly ILocalDBProvider dBProvider;

        public ContactsService(ILocalDBProvider localDBProvider)
        {
            dBProvider = localDBProvider ?? throw new ArgumentNullException(nameof(localDBProvider));
        }

        public void AddContact(Contact contact)
        {
            if (contact == null)
                throw new ArgumentNullException(nameof(contact));

            ILiteCollection<Contact> collection = dBProvider.Database.GetCollection<Contact>();

            collection.Insert(contact);
        }

        public IEnumerable<Contact> GetStoredContacts()
        {
            ILiteCollection<Contact> collection = dBProvider.Database.GetCollection<Contact>();
            return collection.FindAll();
        }
    }
}
