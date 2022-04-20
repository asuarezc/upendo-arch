using System.Globalization;
using upendo.CrossCutting.Localization;
using Xamarin.Essentials;

namespace upendo.Helpers
{
    public static class LanguageHelper
    {
        public static AvailableLanguage PreferredLanguage => (AvailableLanguage)Preferences.Get(nameof(ITranslationManager.CurrentLanguage), (int)AvailableLanguage.Default);

        public static AvailableLanguage DeviceLanguage => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant() switch
        {
            "en" => AvailableLanguage.English,
            "fr" => AvailableLanguage.French,
            "de" => AvailableLanguage.German,
            "it" => AvailableLanguage.Italian,
            "pt" => AvailableLanguage.Portuguese,
            "es" => AvailableLanguage.Spanish,
            _ => AvailableLanguage.Default,
        };

        public static AvailableLanguage CurrentLanguage => PreferredLanguage != AvailableLanguage.Default
            ? PreferredLanguage
            : DeviceLanguage != AvailableLanguage.Default
                ? DeviceLanguage
                : AvailableLanguage.English;
    }
}
