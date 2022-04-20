using System;
using System.Linq;
using System.Threading.Tasks;
using upendo.Managers;
using upendo.Models;
using Xamarin.Forms;

namespace upendo.ViewModels
{
    public abstract class BaseViewModel : UnitOfWorkBaseViewModel
    {
        private Command backCommand = null;
        private Command rightCommand = null;

        public virtual string Title => string.Empty;

        public bool UsingDarkTheme => ThemeManager.Instance.CurrentTheme == ThemeManager.Theme.Dark;

        private bool canBack = true;
        public bool CanBack
        {
            get => canBack && IsNotBusy;
            set
            {
                canBack = value;
                NotifyPropertyChanged();
            }
        }

        public ContentPage Page { get; set; }

        public virtual Command BackCommand => backCommand ??= new Command(
            async () =>
            {
                if (Page.Navigation.ModalStack != null && Page.Navigation.ModalStack.Any())
                    await Page.Navigation.PopModalAsync();
                else if (Page.Navigation.NavigationStack != null && Page.Navigation.NavigationStack.Any())
                    await Page.Navigation.PopAsync(true);
            },
            () => CanBack
        );

        public virtual Command RightCommand => rightCommand ??= new Command(
            () => { throw new InvalidOperationException($"{nameof(RightCommand)} must be overrided from {nameof(BaseViewModel)} to be executed."); },
            () => IsNotBusy
        );
    }

    public abstract class BaseViewModel<TModel> : BaseViewModel
        where TModel : BaseModel, new()
    {
        private TModel model = null;
        public TModel Model
        {
            get => model;
            set
            {
                model = value;
                NotifyPropertyChanged(() => Model);
            }
        }

        public virtual async Task<TModel> GetInitialModelAsync()
        {
            return await Task.Run(() => { return new TModel(); });
        }

        public async Task ReloadModelAsync()
        {
            Model = await GetInitialModelAsync();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Model?.Dispose();

            base.Dispose(disposing);
        }
    }
}
