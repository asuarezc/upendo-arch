using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace upendo.ContentViews
{
    public partial class MenuOption : ContentView
    {
        public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(
            nameof(Description),
            typeof(string),
            typeof(MenuOption),
            string.Empty,
            propertyChanged: DescriptionPropertyChanged
        );

        public static readonly BindableProperty TopBarBackgroundColorProperty = BindableProperty.Create(
            nameof(TopBarBackgroundColor),
            typeof(Color),
            typeof(MenuOption),
            Color.DarkBlue,
            propertyChanged: TopBarBackgroundColorPropertyChanged
        );

        public static readonly BindableProperty OptionBackgroundColorProperty = BindableProperty.Create(
            nameof(OptionBackgroundColor),
            typeof(Color),
            typeof(MenuOption),
            Color.Blue,
            propertyChanged: OptionBackgroundColorPropertyChanged
        );

        public static readonly BindableProperty OptionImageSourceProperty = BindableProperty.Create(
            nameof(OptionImageSource),
            typeof(ImageSource),
            typeof(MenuOption),
            ImageSource.FromFile("round_cloud_white_36"),
            propertyChanged: OptionImageSourcePropertyChanged
        );

        public static readonly BindableProperty GoImageSourceProperty = BindableProperty.Create(
            nameof(GoImageSource),
            typeof(ImageSource),
            typeof(MenuOption),
            ImageSource.FromFile("round_arrow_forward_ios_black_36"),
            propertyChanged: GoImageSourcePropertyChanged
        );

        public static readonly BindableProperty TapCommandProperty = BindableProperty.Create(
            nameof(TapCommand),
            typeof(Command),
            typeof(MenuOption),
            propertyChanged: CommandPropertyChanged
        );

        public static readonly BindableProperty UseDarkThemeProperty = BindableProperty.Create(
            nameof(UseDarkTheme),
            typeof(bool),
            typeof(MenuOption),
            propertyChanged: UseDarkThemePropertyChanged
        );

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public Color TopBarBackgroundColor
        {
            get => (Color)GetValue(TopBarBackgroundColorProperty);
            set => SetValue(TopBarBackgroundColorProperty, value);
        }

        public Color OptionBackgroundColor
        {
            get => (Color)GetValue(OptionBackgroundColorProperty);
            set => SetValue(OptionBackgroundColorProperty, value);
        }

        public ImageSource OptionImageSource
        {
            get => (ImageSource)GetValue(OptionImageSourceProperty);
            set => SetValue(OptionImageSourceProperty, value);
        }

        public ImageSource GoImageSource
        {
            get => (ImageSource)GetValue(GoImageSourceProperty);
            set => SetValue(GoImageSourceProperty, value);
        }

        public Command TapCommand
        {
            get => (Command)GetValue(TapCommandProperty);
            set => SetValue(TapCommandProperty, value);
        }

        public bool UseDarkTheme
        {
            get => (bool)GetValue(UseDarkThemeProperty);
            set => SetValue(UseDarkThemeProperty, value);
        }

        public MenuOption()
        {
            InitializeComponent();
            InitializeBindableProperties();
        }

        private void InitializeBindableProperties()
        {
            descriptionLabel.Text = Description;
            optionImage.Source = OptionImageSource;
            goImage.Source = GoImageSource;
            topBarBoxView.BackgroundColor = TopBarBackgroundColor;
            optionStackLayout.BackgroundColor = OptionBackgroundColor;
            mainLayout.BackgroundColor = UseDarkTheme ? Color.LightGray : Color.White;

            AddTapGestureRecognizerTo(this, TapCommand);
        }

        private static void AddTapGestureRecognizerTo(MenuOption menuOption, ICommand command = null)
        {
            if (menuOption == null || menuOption.mainLayout == null)
                return;

            if (menuOption.mainLayout.GestureRecognizers != null && menuOption.mainLayout.GestureRecognizers.Any())
                menuOption.mainLayout.GestureRecognizers.Clear();

            if (command == null)
            {
                menuOption.mainLayout.GestureRecognizers.Add(
                    new TapGestureRecognizer { Command = new Command(() =>
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await menuOption.FadeTo(0, 150, Easing.Linear);
                            await menuOption.FadeTo(1, 150, Easing.Linear);
                        });
                    }
                )});
            }
            else
            {
                ICommand tapCommand = new Command(() =>
                {
                    _ = Device.InvokeOnMainThreadAsync(async () =>
                    {
                        await menuOption.FadeTo(0, 150, Easing.Linear);
                        await menuOption.FadeTo(1, 150, Easing.Linear);
                    }).ContinueWith((Task fadeTask) =>
                    {
                        if (command.CanExecute(null))
                            command.Execute(null);
                    });
                });

                menuOption.mainLayout.GestureRecognizers.Add(new TapGestureRecognizer { Command = tapCommand });
            }
        }

        private static void DescriptionPropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject != null && bindableObject is MenuOption menuOption && menuOption.descriptionLabel != null)
                menuOption.descriptionLabel.Text = newValue.ToString();
        }

        private static void TopBarBackgroundColorPropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject != null && bindableObject is MenuOption menuOption && menuOption.topBarBoxView != null)
                menuOption.topBarBoxView.BackgroundColor = (Color)newValue;
        }

        private static void OptionBackgroundColorPropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject != null && bindableObject is MenuOption menuOption && menuOption.optionStackLayout != null)
                menuOption.optionStackLayout.BackgroundColor = (Color)newValue;
        }

        private static void OptionImageSourcePropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject != null && bindableObject is MenuOption menuOption && menuOption.optionImage != null)
                menuOption.optionImage.Source = (ImageSource)newValue;
        }

        private static void GoImageSourcePropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject != null && bindableObject is MenuOption menuOption && menuOption.goImage != null)
                menuOption.goImage.Source = (ImageSource)newValue;
        }

        private static void CommandPropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject == null || bindableObject is not MenuOption menuOption || menuOption.optionStackLayout == null || newValue == null || newValue is not ICommand newCommand)
                return;

            AddTapGestureRecognizerTo(menuOption, newCommand);
        }

        private static void UseDarkThemePropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject == null || bindableObject is not MenuOption menuOption || menuOption.mainLayout == null || newValue == null || newValue is not bool newBool)
                return;

            menuOption.mainLayout.BackgroundColor = newBool ? Color.LightGray : Color.White;
        }
    }
}
