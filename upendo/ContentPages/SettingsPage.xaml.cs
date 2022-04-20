using System;
using System.Linq;
using upendo.CrossCutting.Localization;
using upendo.Models;
using upendo.ViewModels;
using Xamarin.Forms;
using DryIoc;
using upendo.Managers;

namespace upendo.ContentPages
{
    public partial class SettingsPage : SettingsPageXaml
    {
        private readonly ITranslationManager translationManager;

        public SettingsPage()
        {
            InitializeComponent();
            translationManager = DependencyServiceManager.Instance.Container.Resolve<ITranslationManager>();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            translationManager.CurrentLanguageChanged += OnCurrentLanguageChanged;
        }

        protected override void OnDisappearing()
        {
            translationManager.CurrentLanguageChanged -= OnCurrentLanguageChanged;

            base.OnDisappearing();
        }

        private void OnCurrentLanguageChanged(object sender, EventArgs e)
        {
            ResetNavigationStack();
        }

        private void ResetNavigationStack()
        {
            Page newSettingsPage = new SettingsPage();
            Page newMainPage = new MainPage(true);

            Navigation.InsertPageBefore(newSettingsPage, this);
            Navigation.RemovePage(Navigation.NavigationStack.Single(it => it is MainPage));
            Navigation.InsertPageBefore(newMainPage, newSettingsPage);

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PopAsync();
            });
        }
    }

    public class SettingsPageXaml : BaseContentPage<SettingsViewModel, SettingsModel> { }
}
