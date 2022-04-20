using System;
using System.Collections.Generic;
using System.Threading;
using upendo.Helpers;
using upendo.Themes;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace upendo.Managers
{
    public sealed class ThemeManager
    {
        public enum Theme
        {
            Light = 0,
            Dark = 1
        }

        private static readonly Lazy<ThemeManager> lazyInstance = new(
            () => new ThemeManager(), LazyThreadSafetyMode.PublicationOnly
        );

        private ThemeManager() { }

        public static ThemeManager Instance => lazyInstance.Value;

        public void ChangeTheme(Theme theme)
        {
            if (Application.Current.Resources.MergedDictionaries == null)
                return;

            Device.BeginInvokeOnMainThread(() =>
            {
                Application.Current.Resources.MergedDictionaries.Clear();

                switch (theme)
                {
                    case Theme.Light:
                        Application.Current.Resources.MergedDictionaries.Add(new Light());
                        break;

                    case Theme.Dark:
                        Application.Current.Resources.MergedDictionaries.Add(new Dark());
                        break;
                }
            });

            Preferences.Set(Constans.THEME, theme.ToString());
        }

        public Theme CurrentTheme => (Theme)Enum.Parse(typeof(Theme), Preferences.Get(Constans.THEME, "Light"));
    }
}
