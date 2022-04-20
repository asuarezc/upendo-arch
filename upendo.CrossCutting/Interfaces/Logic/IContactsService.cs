using System;
using System.Collections.Generic;
using upendo.CrossCutting.Entities;

namespace upendo.CrossCutting.Interfaces.Logic
{
    public interface IContactsService
    {
        IEnumerable<Contact> GetStoredContacts();

        void AddContact(Contact contact);
    }
}
