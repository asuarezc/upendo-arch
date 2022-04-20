using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using upendo.CrossCutting.Localization;
using upendo.Helpers;
using upendo.Managers;
using Xamarin.Essentials;

namespace upendo.Models
{
    public class SettingsModel : BaseModel
    {
        private ObservableCollection<LanguageModel> languages;
        public ObservableCollection<LanguageModel> Languages => languages ??= new ObservableCollection<LanguageModel>(GetLanguageModels());

        private LanguageModel selectedLanguage;
        public LanguageModel SelectedLanguage
        {
            get => selectedLanguage;
            set
            {
                selectedLanguage = value;
                NotifyPropertyChanged();

                if (selectedLanguage == null)
                    return;

                Preferences.Set(nameof(ITranslationManager.CurrentLanguage), (int)selectedLanguage.Language);
                TranslationManager.SetLanguage(LanguageHelper.CurrentLanguage);
            }
        }

        public bool IsDarkThemeEnabled
        {
            get => ThemeManager.Instance.CurrentTheme == ThemeManager.Theme.Dark;
            set
            {
                ThemeManager.Instance.ChangeTheme(value ? ThemeManager.Theme.Dark : ThemeManager.Theme.Light);
                NotifyPropertyChanged();
            }
        }

        public SettingsModel()
        {
            selectedLanguage = new LanguageModel { Language = LanguageHelper.PreferredLanguage };
        }

        private IEnumerable<LanguageModel> GetLanguageModels()
        {
            return Enum.GetValues(typeof(AvailableLanguage))
                .Cast<AvailableLanguage>()
                .Select(it => new LanguageModel { Language = it })
                .OrderBy(it => it.Language);
        }
    }
}
