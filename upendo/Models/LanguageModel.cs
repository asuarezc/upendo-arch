using System;
using upendo.CrossCutting.Localization;

namespace upendo.Models
{
    public class LanguageModel : BaseModel
    {
        private AvailableLanguage language;
        public AvailableLanguage Language
        {
            get => language;
            set
            {
                language = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(() => Description);
            }
        }

        public string Description => GetResource(LanguageLocalizationKey);

        private StringKey LanguageLocalizationKey => language switch
        {
            AvailableLanguage.Default => StringKey.language_default,
            AvailableLanguage.English => StringKey.language_english,
            AvailableLanguage.French => StringKey.language_french,
            AvailableLanguage.German => StringKey.language_german,
            AvailableLanguage.Italian => StringKey.language_italian,
            AvailableLanguage.Portuguese => StringKey.language_portuguese,
            AvailableLanguage.Spanish => StringKey.language_spanish,
            _ => StringKey.language_default,
        };
    }
}