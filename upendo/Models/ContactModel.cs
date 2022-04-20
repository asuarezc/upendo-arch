using System;
using upendo.CrossCutting.Entities;

namespace upendo.Models
{
    public class ContactModel : BaseModel
    {
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                NotifyPropertyChanged();
            }
        }

        private string phone;
        public string Phone
        {
            get => phone;
            set
            {
                phone = value;
                NotifyPropertyChanged();
            }
        }

        public ContactModel(Contact contact)
        {
            if (contact == null)
                throw new ArgumentNullException(nameof(contact));

            Name = contact.Name;
            Phone = contact.Phone;
        }
    }
}
