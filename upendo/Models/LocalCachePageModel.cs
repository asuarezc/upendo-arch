using System.Collections.ObjectModel;

namespace upendo.Models
{
    public class LocalCachePageModel : BaseModel
    {
        private ObservableCollection<string> strings;
        public ObservableCollection<string> Strings
        {
            get => strings;
            set
            {
                strings = value;
                NotifyPropertyChanged();
            }
        }

        private string newString;
        public string NewString
        {
            get => newString;
            set
            {
                newString = value;
                NotifyPropertyChanged();
            }
        }

        private double currentExpirationSeconds;
        public double CurrentExpirationSeconds
        {
            get => currentExpirationSeconds;
            set
            {
                currentExpirationSeconds = value;
                NotifyPropertyChanged();
            }
        }
    }
}
