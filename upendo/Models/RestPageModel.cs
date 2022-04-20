using System.Collections.ObjectModel;

namespace upendo.Models
{
    public class RestPageModel : BaseModel
    {
        private ObservableCollection<UserModel> users;
        public ObservableCollection<UserModel> Users
        {
            get => users;
            set
            {
                users = value;
                NotifyPropertyChanged();
            }
        }
    }
}
