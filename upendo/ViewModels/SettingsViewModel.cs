using System.Linq;
using System.Threading.Tasks;
using upendo.CrossCutting.Localization;
using upendo.Helpers;
using upendo.Models;

namespace upendo.ViewModels
{
    public class SettingsViewModel : BaseViewModel<SettingsModel>
    {
        public override string Title => GetResource(StringKey.menu_settings);

        public override async Task<SettingsModel> InitializeModel()
        {
            return await Task.Run(
                () =>
                {
                    SettingsModel model = new();
                    model.SelectedLanguage = model.Languages.SingleOrDefault(it => it.Language == LanguageHelper.PreferredLanguage);

                    return model;
                }
            );
        }
    }
}
