using System;
using System.Threading.Tasks;
using upendo.Managers;
using upendo.Models;
using upendo.ViewModels;
using Xamarin.Forms;

namespace upendo.ContentPages
{
    public abstract class BaseContentPage : ContentPage, IDisposable
    {
        protected bool disposedValue;
        private bool isFirstOnAppearing = true;

        protected bool IsFirstOnAppearing => isFirstOnAppearing;

        protected BaseViewModel BaseViewModel { get; set; }

        public BaseContentPage()
        {
            NavigationPage.SetBackButtonTitle(this, null);
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override bool OnBackButtonPressed()
        {
            if (BaseViewModel.BackCommand != null && BaseViewModel.BackCommand.CanExecute(null))
            {
                BaseViewModel.BackCommand.Execute(null);
                return true;
            }

            return base.OnBackButtonPressed();
        }

        protected virtual void OnFirstAppearing() { }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ThemeManager.Instance.ChangeTheme(ThemeManager.Instance.CurrentTheme); //Workaround to Xamarin Forms 5 bug: https://github.com/xamarin/Xamarin.Forms/issues/10583

            if (isFirstOnAppearing)
            {
                OnFirstAppearing();
                isFirstOnAppearing = false;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public abstract class BaseContentPage<TViewModel, KModel> : BaseContentPage
        where TViewModel : BaseViewModel<KModel>, new()
        where KModel : BaseModel, new()
    {
        public TViewModel ViewModel { get; private set; }

        public BaseContentPage() : base()
        {
            BindingContext = BaseViewModel = ViewModel = new TViewModel();
            ViewModel.Page = this;
        }

        protected override void OnFirstAppearing()
        {
            base.OnFirstAppearing();

            ViewModel.Page = this;

            Task.Run(async () =>
            {
                ViewModel.Model = await ViewModel.InitializeModel();
            });
        }

        protected override void OnDisappearing()
        {
            ViewModel?.Dispose();

            base.OnDisappearing();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
                ViewModel?.Dispose();

            base.Dispose(disposing);
        }
    }
}
