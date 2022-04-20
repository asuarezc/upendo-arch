using System.Threading.Tasks;
using upendo.CrossCutting.Localization;
using upendo.Models;
using upendo.ViewModels;
using Xamarin.Forms;

namespace upendo.ContentPages
{
    public partial class MainPage : MainPageXaml
    {
        private readonly bool createdFromLanguageChange;

        public MainPage(bool createdFromLanguageChange = false)
        {
            InitializeComponent();
            this.createdFromLanguageChange = createdFromLanguageChange;
        }

        protected override void OnFirstAppearing()
        {
            if (!createdFromLanguageChange)
            {
                Task.Run(async () =>
                {
                    menuOptionsStackLayout.Opacity = 0;
                    await menuOptionsStackLayout.FadeTo(1, 1000);
                    menuOptionsStackLayout.Opacity = 1;
                    ViewModel.ShowStatusMessage(ViewModel.GetResource(StringKey.app_welcome));
                });
            }

            base.OnFirstAppearing();
        }

        protected override void OnAppearing()
        {
            if (!IsFirstOnAppearing)
                _ = ViewModel.ReloadModelAsync();

            base.OnAppearing();
        }
    }

    public class MainPageXaml : BaseContentPage<MainPageViewModel, MainPageModel> { }
}
