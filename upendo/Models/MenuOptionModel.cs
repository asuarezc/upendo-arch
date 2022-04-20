using Xamarin.Forms;

namespace upendo.Models
{
    public class MenuOptionModel : BaseModel
    {
        private string description;
        public string Description
        {
            get => description;
            set
            {
                description = value;
                NotifyPropertyChanged();
            }
        }

        private Color topBarBackgroundColor;
        public Color TopBarBackgroundColor
        {
            get => topBarBackgroundColor;
            set
            {
                topBarBackgroundColor = value;
                NotifyPropertyChanged();
            }
        }

        private Color optionBackgroundColor;
        public Color OptionBackgroundColor
        {
            get => optionBackgroundColor;
            set
            {
                optionBackgroundColor = value;
                NotifyPropertyChanged();
            }
        }

        private ImageSource optionImageSource;
        public ImageSource OptionImageSource
        {
            get => optionImageSource;
            set
            {
                optionImageSource = value;
                NotifyPropertyChanged();
            }
        }

        private ImageSource goImageSource;
        public ImageSource GoImageSource
        {
            get => goImageSource;
            set
            {
                goImageSource = value;
                NotifyPropertyChanged();
            }
        }

        private Command command;
        public Command Command
        {
            get => command;
            set
            {
                command = value;
                NotifyPropertyChanged();
            }
        }
    }
}
