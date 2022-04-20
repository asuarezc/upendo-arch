using System.Collections.ObjectModel;

namespace upendo.Models
{
    public class MainPageModel : BaseModel
    {
        private ObservableCollection<MenuOptionModel> menuOptions;
        public ObservableCollection<MenuOptionModel> MenuOptions
        {
            get => menuOptions;
            set
            {
                menuOptions = value;
                NotifyPropertyChanged();
            }
        }
    }
}
