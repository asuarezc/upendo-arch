using System;
using System.Threading;
using System.Threading.Tasks;
using upendo.CrossCutting.Localization;
using upendo.Models;
using Xamarin.Forms;

namespace upendo.ViewModels
{
    public class RunBusyPageViewModel : BaseViewModel<RunBusyPageModel>
    {
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        public override string Title => TranslationManager.GetResource(StringKey.menu_run_in_background);

        private bool cancellableCommandIsRunning;
        public bool CancellableCommandIsRunning
        {
            get => cancellableCommandIsRunning;
            set
            {
                cancellableCommandIsRunning = value;
                NotifyPropertyChanged();
            }
        }

        private Command notCancellableCommand;
        public Command NotCancellableCommand => notCancellableCommand ??= new Command(
            async () => await DoNotCancellableCommandActionAsync(),
            () => IsNotBusy
        );

        private Command cancellableCommand;
        public Command CancellableCommand => cancellableCommand ??= new Command(
            async () => await DoCancellableCommandActionAsync(),
            () => IsNotBusy
        );

        private Command cancelCommand;
        public Command CancelCommand => cancelCommand ??= new Command(
            () => cancellationTokenSource?.Cancel(),
            () => CancellableCommandIsRunning
        );

        private async Task DoNotCancellableCommandActionAsync()
        {
            await RunUnitOfWorkAsync(
                job: () =>
                {
                    Random random = new();

                    ShowStatusMessage(GetResource(StringKey.global_running_task));

                    for (int i = 0; i <= int.MaxValue; i++)
                        random.Next();
                },
                timeout: TimeSpan.FromSeconds(Model.CurrentTimeoutSeconds)
            );
        }

        private async Task DoCancellableCommandActionAsync()
        {
            CancellableCommandIsRunning = true;
            cancellationTokenSource = new();
            cancellationToken = cancellationTokenSource.Token;

            await RunCancellableUnitOfWorkAsync(
                job: (CancellationToken token) =>
                {
                    Random random = new();

                    ShowStatusMessage(GetResource(StringKey.global_running_task));

                    for (int i = 0; i <= int.MaxValue; i++)
                    {
                        token.ThrowIfCancellationRequested();
                        random.Next();
                    }
                },
                token: cancellationToken
            );
        }
    }
}
