using System;

namespace upendo.CrossCutting.Localization
{
    public interface ITranslationManager
    {
        AvailableLanguage? CurrentLanguage { get; }

        string GetResource(StringKey key, string defaultValue = "");
        string GetResource(string key, string defaultValue = "");
        void SetLanguage(AvailableLanguage language);

        event EventHandler CurrentLanguageChanged;
    }
}