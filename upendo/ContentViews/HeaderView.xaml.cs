﻿using System.Linq;
using Xamarin.Forms;

namespace upendo.ContentViews
{
    public partial class HeaderView : ContentView
    {
        public static readonly BindableProperty TitleTextProperty = BindableProperty.Create(
            nameof(TitleText),
            typeof(string),
            typeof(HeaderView),
            string.Empty,
            propertyChanged: TitleText_PropertyChanged
        );

        public static readonly BindableProperty IsBackButtonVisibleProperty = BindableProperty.Create(
            nameof(IsBackButtonVisible),
            typeof(bool),
            typeof(HeaderView),
            false,
            propertyChanged: IsBackButtonVisible_PropertyChanged
        );

        public static readonly BindableProperty IsBackButtonEnabledPropery = BindableProperty.Create(
            nameof(IsBackButtonEnabled),
            typeof(bool),
            typeof(HeaderView),
            false,
            propertyChanged: IsBackButtonEnabled_PropertyChanged
        );

        public static readonly BindableProperty BackCommandProperty = BindableProperty.Create(
            nameof(BackCommand),
            typeof(Command),
            typeof(HeaderView),
            null,
            propertyChanged: BackCommand_PropertyChanged
        );

        public static readonly BindableProperty IsRightButtonVisibleProperty = BindableProperty.Create(
            nameof(IsRightButtonVisible),
            typeof(bool),
            typeof(HeaderView),
            false,
            propertyChanged: IsRightButtonVisible_PropertyChanged
        );

        public static readonly BindableProperty IsRightButtonEnabledProperty = BindableProperty.Create(
            nameof(IsRightButtonEnabled),
            typeof(bool),
            typeof(HeaderView),
            false,
            propertyChanged: IsRightButtonEnabled_PropertyChanged
        );

        public static readonly BindableProperty RightCommandProperty = BindableProperty.Create(
            nameof(RightCommand),
            typeof(Command),
            typeof(HeaderView),
            null,
            propertyChanged: RightCommand_PropertyChanged
        );

        public static readonly BindableProperty RightButtonImageSourceProperty = BindableProperty.Create(
            nameof(RightButtonImageSource),
            typeof(ImageSource),
            typeof(HeaderView),
            null,
            BindingMode.OneWay,
            propertyChanged: RightButtonImageSource_PropertyChanged
        );

        public string TitleText
        {
            get => (string)GetValue(TitleTextProperty);
            set => SetValue(TitleTextProperty, value);
        }

        public bool IsBackButtonVisible
        {
            get => (bool)GetValue(IsBackButtonVisibleProperty);
            set => SetValue(IsBackButtonVisibleProperty, value);
        }

        public bool IsBackButtonEnabled
        {
            get => (bool)GetValue(IsBackButtonEnabledPropery);
            set => SetValue(IsBackButtonEnabledPropery, value);
        }

        public Command BackCommand
        {
            get => (Command)GetValue(BackCommandProperty);
            set => SetValue(BackCommandProperty, value);
        }

        public bool IsRightButtonVisible
        {
            get => (bool)GetValue(IsRightButtonVisibleProperty);
            set => SetValue(IsRightButtonVisibleProperty, value);
        }

        public bool IsRightButtonEnabled
        {
            get => (bool)GetValue(IsRightButtonEnabledProperty);
            set => SetValue(IsRightButtonEnabledProperty, value);
        }

        public Command RightCommand
        {
            get => (Command)GetValue(RightCommandProperty);
            set => SetValue(RightCommandProperty, value);
        }

        public ImageSource RightButtonImageSource
        {
            get => (ImageSource)GetValue(RightButtonImageSourceProperty);
            set => SetValue(RightButtonImageSourceProperty, value);
        }

        public HeaderView()
        {
            InitializeComponent();
            InitializeBackButton();
            InitializeRightButton();
        }

        private static void TitleText_PropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject != null && bindableObject is HeaderView headerView && headerView.titleLabel != null)
                headerView.titleLabel.Text = newValue.ToString();
        }

        private static void IsBackButtonVisible_PropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject != null && bindableObject is HeaderView headerView && headerView.backImage != null)
                headerView.backImage.IsVisible = (bool)newValue;
        }

        private static void IsBackButtonEnabled_PropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject != null && bindableObject is HeaderView headerView
                && headerView.backImage != null && newValue != null && bool.TryParse(newValue.ToString(), out bool enabled))
            {
                headerView.backImage.Opacity = enabled ? 1d : 0.5d;
            } 
        }

        private static void BackCommand_PropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject == null || bindableObject is not HeaderView headerView
                || headerView.backImage == null || headerView.backImage.GestureRecognizers == null)
            {
                return;
            }

            if (headerView.backImage.GestureRecognizers.Count == 1
                && headerView.backImage.GestureRecognizers.First() is TapGestureRecognizer tapGestureRecognizer)
            {
                tapGestureRecognizer.Command = (Command)newValue;
            }
            else
            {
                headerView.backImage.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = (Command)newValue
                });
            }
        }

        private static void IsRightButtonVisible_PropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject != null && bindableObject is HeaderView headerView && headerView.rightImage != null)
                headerView.rightImage.IsVisible = (bool)newValue;
        }

        private static void IsRightButtonEnabled_PropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject != null && bindableObject is HeaderView headerView
                && headerView.rightImage != null && newValue != null && bool.TryParse(newValue.ToString(), out bool enabled))
            {
                headerView.rightImage.Opacity = enabled ? 1d : 0.5d;
            }
        }

        private static void RightCommand_PropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject == null || bindableObject is not HeaderView headerView
                || headerView.rightImage == null || headerView.rightImage.GestureRecognizers == null)
            {
                return;
            }

            if (headerView.rightImage.GestureRecognizers.Count == 1
                && headerView.rightImage.GestureRecognizers.First() is TapGestureRecognizer tapGestureRecognizer)
            {
                tapGestureRecognizer.Command = (Command)newValue;
            }
            else
            {
                headerView.rightImage.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = (Command)newValue
                });
            }
        }

        private static void RightButtonImageSource_PropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject != null && bindableObject is HeaderView headerView && headerView.rightImage != null)
                headerView.rightImage.Source = (ImageSource)newValue;
        }

        private void InitializeBackButton()
        {
            if (backImage == null)
                return;

            backImage.IsVisible = IsBackButtonVisible;
            backImage.Opacity = IsBackButtonEnabled ? 1d : 0.5d;

            if (backImage.GestureRecognizers == null || BackCommand == null)
                return;

            backImage.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = BackCommand
            });
        }

        private void InitializeRightButton()
        {
            if (rightImage == null)
                return;

            rightImage.IsVisible = IsRightButtonVisible;
            rightImage.Opacity = IsRightButtonEnabled ? 1d : 0.5d;

            if (rightImage.GestureRecognizers == null || RightCommand == null)
                return;

            rightImage.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = RightCommand
            });
        }
    }
}
