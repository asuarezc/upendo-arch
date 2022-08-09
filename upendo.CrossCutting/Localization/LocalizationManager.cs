using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace upendo.CrossCutting.Localization
{
    public class TranslationManager : ITranslationManager
    {
        private IDictionary<string, string> currentStrings;

        public AvailableLanguage CurrentLanguage { get; private set; }

        public event EventHandler CurrentLanguageChanged;

        public string GetResource(string key, string defaultValue = "")
        {
            return string.IsNullOrEmpty(key) || currentStrings == null || !currentStrings.Any() || !currentStrings.ContainsKey(key)
                ? string.IsNullOrEmpty(defaultValue) ? $"\"{key}\"" : defaultValue
                : currentStrings[key];
        }

        public string GetResource(StringKey key, string defaultValue = "")
        {
            return GetResource(key.ToString(), defaultValue);
        }

        public void SetLanguage(AvailableLanguage language)
        {
            if (CurrentLanguage == language)
                return;

            CurrentLanguage = language;
            SetLanguageInternal(CurrentLanguage);
        }

        private void SetLanguageInternal(AvailableLanguage language)
        {
            string jsonContent = GetJsonContentFromLanguage(language);

            currentStrings = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
            CurrentLanguageChanged?.Invoke(this, EventArgs.Empty);
        }

        private string GetJsonContentFromLanguage(AvailableLanguage language)
        {
            using Stream stream = GetType().Assembly.GetManifestResourceStream(GetResourceNameFromLanguage(language));
            using StreamReader reader = new(stream);
            return reader.ReadToEnd();
        }

        private string GetResourceNameFromLanguage(AvailableLanguage language)
        {
            return $"upendo.CrossCutting.Localization.Strings.{language}.json";
        }
    }
}
