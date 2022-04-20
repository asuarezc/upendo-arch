using System;
using upendo.ViewModels;

namespace upendo.Helpers
{
    public class BusyLifeTimeScope : IDisposable
    {
        private bool disposedValue;
        private readonly UnitOfWorkBaseViewModel viewModel;
        private readonly object lockObject = new();

        public BusyLifeTimeScope(UnitOfWorkBaseViewModel viewModel)
        {
            this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            lock (lockObject)
                viewModel.IsBusy = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            lock (lockObject)
                viewModel.IsBusy = false;

            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
