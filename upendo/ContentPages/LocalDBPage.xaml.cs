using System;
using upendo.Models;
using upendo.ViewModels;

namespace upendo.ContentPages
{
    public partial class LocalDBPage : LocalDBPageXaml
    {
        public LocalDBPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            nameEntry.Completed += NameEntry_Completed;
            phoneEntry.Completed += PhoneEntry_Completed;
        }

        protected override void OnDisappearing()
        {
            nameEntry.Completed -= NameEntry_Completed;
            phoneEntry.Completed -= PhoneEntry_Completed;
        }

        private void NameEntry_Completed(object sender, EventArgs e)
        {
            phoneEntry.Focus();
        }

        private void PhoneEntry_Completed(object sender, EventArgs e)
        {
            addButton.Focus();
        }
    }

    public class LocalDBPageXaml : BaseContentPage<LocalDBPageViewModel, LocalDBPageModel> { }
}
