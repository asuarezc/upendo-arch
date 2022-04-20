using System.Threading.Tasks;
using Xamarin.Forms;
using System;
using upendo.ViewModels;
using upendo.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace upendo.ContentViews
{
    public partial class FooterView : ContentView
    {
        public static readonly BindableProperty IsVisibleAndRunningProperty = BindableProperty.Create(
            nameof(IsVisibleAndRunning),
            typeof(bool),
            typeof(FooterView),
            false,
            BindingMode.OneWay,
            propertyChanged: IsVisibleAndRunningPropertyChanged
        );

        private readonly Queue<string> messagesQueue;
        private Task showMessagesTask;

        public bool IsVisibleAndRunning
        {
            get => (bool)GetValue(IsVisibleAndRunningProperty);
            set => SetValue(IsVisibleAndRunningProperty, value);
        }

        public FooterView()
        {
            InitializeComponent();
            InitializeIsBusyActivityIndicator();

            messagesQueue = new Queue<string>();
        }

        private static void IsVisibleAndRunningPropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject != null && bindableObject is FooterView footerView && footerView.isBusyActivityIndicator != null)
                footerView.isBusyActivityIndicator.IsVisible = footerView.isBusyActivityIndicator.IsRunning = (bool)newValue;
        }

        private void InitializeIsBusyActivityIndicator()
        {
            if (isBusyActivityIndicator == null)
                return;

            isBusyActivityIndicator.IsVisible = isBusyActivityIndicator.IsRunning = IsVisibleAndRunning;
        }

        private void EnqueueMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            messagesQueue.Enqueue(message);

            if (showMessagesTask == null)
                showMessagesTask = Task.Run(ShowMessagesAsync);
            else
            {
                if (!showMessagesTask.IsCompleted)
                    return;
                else
                    showMessagesTask = Task.Run(ShowMessagesAsync);
            }
        }

        private async Task ShowMessagesAsync()
        {
            while (messagesQueue.Any())
            {
                string message = messagesQueue.Dequeue();

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    statusLabel.Opacity = 0d;
                    statusLabel.Text = message;
                    await statusLabel.FadeTo(1, 1000);

                    await Task.Delay(TimeSpan.FromSeconds(2));

                    await statusLabel.FadeTo(0, 1000);
                    statusLabel.Text = string.Empty;
                    statusLabel.Opacity = 0d;
                });
            }
        }

        private void LifecycleEffect_Loaded(object sender, EventArgs e)
        {
            MessagingCenter.Instance.Subscribe(this, Constans.SHOW_STATUS_MESSAGE,
                (BaseViewModel sender, string message) => EnqueueMessage(message)
            );
        }

        private void LifecycleEffect_Unloaded(object sender, EventArgs e)
        {
            MessagingCenter.Instance.Unsubscribe<BaseViewModel, string>(this, Constans.SHOW_STATUS_MESSAGE);

            Task.Run(() =>
            {
                if (showMessagesTask != null && !showMessagesTask.IsCompleted)
                    showMessagesTask.Wait();

                showMessagesTask?.Dispose();
            });
        }
    }
}
