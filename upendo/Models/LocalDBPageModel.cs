using System.Collections.ObjectModel;

namespace upendo.Models
{
    public class LocalDBPageModel : BaseModel
    {
        private ObservableCollection<ContactModel> contacts;
        public ObservableCollection<ContactModel> Contacts
        {
            get => contacts;
            set
            {
                contacts = value;
                NotifyPropertyChanged();
            }
        }

        private string addingContactName;
        public string AddingContactName
        {
            get => addingContactName;
            set
            {
                addingContactName = value;
                NotifyPropertyChanged();
            }
        }

        private string addingContactPhone;
        public string AddingContactPhone
        {
            get => addingContactPhone;
            set
            {
                addingContactPhone = value;
                NotifyPropertyChanged();
            }
        }

        public LocalDBPageModel()
        {
            Contacts = new();
        }
    }
}
