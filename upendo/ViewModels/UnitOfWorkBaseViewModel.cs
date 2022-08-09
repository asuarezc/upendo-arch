using System;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using upendo.CrossCutting.Localization;
using upendo.Helpers;
using upendo.Models;
using Xamarin.Forms;

namespace upendo.ViewModels
{
    public abstract class UnitOfWorkBaseViewModel : BaseModel
    {
        private static readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(5);

        private bool isBusy = false;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(() => IsNotBusy);

                MessagingCenter.Instance.Send(this, Constans.IS_BUSY_CHANGED, value);
            }
        }

        public bool IsNotBusy => !IsBusy;

        /// <summary>
        /// Runs an unit of work with default timeout.
        /// </summary>
        /// <param name="job">What to do.</param>
        /// <param name="onSuccess">What to do if <paramref name="job"/> ends without exception.</param>
        /// <param name="onTimeout">What to do if <paramref name="timeout"/>.</param>
        /// <param name="onError">What to do if <paramref name="job"/> ends with exception.</param>
        public async Task RunUnitOfWorkAsync(
            Action job,
            Action onSuccess = null,
            Action onTimeout = null,
            Action<Exception> onError = null)
        {
            await RunUnitOfWorkAsync(
                job,
                defaultTimeout,
                onSuccess,
                onTimeout,
                onError
            );
        }

        /// <summary>
        /// Runs an unit of work with IoC scope and default timeout.
        /// </summary>
        /// <param name="job">What to do.</param>
        /// <param name="onSuccess">What to do if <paramref name="job"/> ends without exception.</param>
        /// <param name="onTimeout">What to do if <paramref name="timeout"/>.</param>
        /// <param name="onError">What to do if <paramref name="job"/> ends with exception.</param>
        public async Task RunUnitOfWorkAsync(
            Action<IResolverContext> job,
            Action onSuccess = null,
            Action onTimeout = null,
            Action<Exception> onError = null)
        {
            await RunUnitOfWorkAsync(
                job,
                defaultTimeout,
                onSuccess,
                onTimeout,
                onError
            );
        }

        /// <summary>
        /// Runs an unit of work with default timeout.
        /// </summary>
        /// <param name="job">What to do.</param>
        /// <param name="onSuccess">What to do if <paramref name="job"/> ends without exception.</param>
        /// <param name="onTimeout">What to do if <paramref name="timeout"/>.</param>
        /// <param name="onError">What to do if <paramref name="job"/> ends with exception.</param>
        public async Task RunUnitOfWorkAsync<T>(
            Func<T> job,
            Action<T> onSuccess = null,
            Action onTimeout = null,
            Action<Exception> onError = null)
        {
            await RunUnitOfWorkAsync(
                job,
                defaultTimeout,
                onSuccess,
                onTimeout,
                onError
            );
        }

        /// <summary>
        /// Runs an unit of work with IoC scope and default timeout.
        /// </summary>
        /// <param name="job">What to do.</param>
        /// <param name="onSuccess">What to do if <paramref name="job"/> ends without exception.</param>
        /// <param name="onTimeout">What to do if <paramref name="timeout"/>.</param>
        /// <param name="onError">What to do if <paramref name="job"/> ends with exception.</param>
        public async Task RunUnitOfWorkAsync<T>(
            Func<IResolverContext, T> job,
            Action<T> onSuccess = null,
            Action onTimeout = null,
            Action<Exception> onError = null)
        {
            await RunUnitOfWorkAsync(
                job,
                defaultTimeout,
                onSuccess,
                onTimeout,
                onError
            );
        }

        /// <summary>
        /// Runs an unit of work with IoC scope and default timeout.
        /// </summary>
        /// <param name="job">What to do.</param>
        /// <param name="onSuccess">What to do if <paramref name="job"/> ends without exception.</param>
        /// <param name="onTimeout">What to do if <paramref name="timeout"/>.</param>
        /// <param name="onError">What to do if <paramref name="job"/> ends with exception.</param>
        public async Task RunUnitOfWorkAsync<T>(
            Func<IResolverContext, Task<T>> job,
            Action<T> onSuccess = null,
            Action onTimeout = null,
            Action<Exception> onError = null)
        {
            await RunUnitOfWorkAsync(
                job,
                defaultTimeout,
                onSuccess,
                onTimeout,
                onError
            );
        }

        /// <summary>
        /// Runs an unit of work.
        /// </summary>
        /// <param name="job">What to do.</param>
        /// <param name="timeout">Time to do it.</param>
        /// <param name="onSuccess">What to do if <paramref name="job"/> ends without exception.</param>
        /// <param name="onTimeout">What to do if <paramref name="timeout"/>.</param>
        /// <param name="onError">What to do if <paramref name="job"/> ends with exception.</param>
        public async Task RunUnitOfWorkAsync(
            Action job,
            TimeSpan timeout,
            Action onSuccess = null,
            Action onTimeout = null,
            Action<Exception> onError = null)
        {
            ThrowIfJobIsNull(job);
            ThrowIfTimeoutIsNotValid(timeout);

            CancellationTokenSource cancellationTokenSource = new();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            Task timeoutTask = null;
            Task jobTask = null;
            Task firstCompletedTask = null;

            if (onTimeout == null)
                onTimeout = OnTimeoutDefaultAction;

            if (onError == null)
                onError = OnErrorDefaultAction;

            try
            {
                using BusyLifeTimeScope busyLifeTimeScope = new(this);

                timeoutTask = Task.Run(async () =>
                {
                    await Task.Delay(timeout);
                    cancellationTokenSource.Cancel();
                });

                jobTask = Task.Run(
                    () => job(), cancellationToken
                );

                firstCompletedTask = await Task.WhenAny(timeoutTask, jobTask);

                if (ReferenceEquals(timeoutTask, firstCompletedTask))
                {
                    onTimeout.Invoke();
                    return;
                }

                if (jobTask.Exception != null)
                    onError.Invoke(jobTask.Exception);
                else
                    onSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
            }
            finally
            {
                _ = Task.Run(() =>
                {
                    if (!timeoutTask.IsCompleted)
                        timeoutTask.Wait();

                    if (!jobTask.IsCompleted)
                        jobTask.Wait();

                    timeoutTask?.Dispose();
                    jobTask?.Dispose();
                    cancellationTokenSource?.Dispose();
                });
            }
        }

        /// <summary>
        /// Runs an unit of work with IoC scope
        /// </summary>
        /// <param name="job">What to do.</param>
        /// <param name="timeout">Time to do it.</param>
        /// <param name="onSuccess">What to do if <paramref name="job"/> ends without exception.</param>
        /// <param name="onTimeout">What to do if <paramref name="timeout"/>.</param>
        /// <param name="onError">What to do if <paramref name="job"/> ends with exception.</param>
        public async Task RunUnitOfWorkAsync(
            Action<IResolverContext> job,
            TimeSpan timeout,
            Action onSuccess = null,
            Action onTimeout = null,
            Action<Exception> onError = null)
        {
            ThrowIfJobIsNull(job);
            ThrowIfTimeoutIsNotValid(timeout);

            CancellationTokenSource cancellationTokenSource = new();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            Task timeoutTask = null;
            Task jobTask = null;
            Task firstCompletedTask = null;

            if (onTimeout == null)
                onTimeout = OnTimeoutDefaultAction;

            if (onError == null)
                onError = OnErrorDefaultAction;

            try
            {
                using BusyLifeTimeScope busyLifeTimeScope = new(this);

                timeoutTask = Task.Run(async () =>
                {
                    await Task.Delay(timeout);
                    cancellationTokenSource.Cancel();
                });

                jobTask = Task.Run(
                    () =>
                    {
                        using IResolverContext scope = Container.OpenScope();
                        job(scope);
                    }, cancellationToken
                );

                firstCompletedTask = await Task.WhenAny(timeoutTask, jobTask);

                if (ReferenceEquals(timeoutTask, firstCompletedTask))
                {
                    onTimeout.Invoke();
                    return;
                }

                if (jobTask.Exception != null)
                    onError.Invoke(jobTask.Exception);
                else
                    onSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
            }
            finally
            {
                _ = Task.Run(() =>
                {
                    if (!timeoutTask.IsCompleted)
                        timeoutTask.Wait();

                    if (!jobTask.IsCompleted)
                        jobTask.Wait();

                    timeoutTask?.Dispose();
                    jobTask?.Dispose();
                    cancellationTokenSource?.Dispose();
                });
            }
        }

        /// <summary>
        /// Runs an unit of work
        /// </summary>
        /// <typeparam name="T">What returns <paramref name="job"/>.</typeparam>
        /// <param name="job">What to do.</param>
        /// <param name="timeout">Time to do it.</param>
        /// <param name="onSuccess">What to do if <paramref name="job"/> ends without exception.</param>
        /// <param name="onTimeout">What to do if <paramref name="timeout"/>.</param>
        /// <param name="onError">What to do if <paramref name="job"/> ends with exception.</param>
        public async Task RunUnitOfWorkAsync<T>(
            Func<T> job,
            TimeSpan timeout,
            Action<T> onSuccess = null,
            Action onTimeout = null,
            Action<Exception> onError = null)
        {
            ThrowIfJobIsNull(job);
            ThrowIfTimeoutIsNotValid(timeout);

            CancellationTokenSource cancellationTokenSource = new();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            Task timeoutTask = null;
            Task jobTask = null;
            Task firstCompletedTask = null;
            T result = default;

            if (onTimeout == null)
                onTimeout = OnTimeoutDefaultAction;

            if (onError == null)
                onError = OnErrorDefaultAction;

            try
            {
                using BusyLifeTimeScope busyLifeTimeScope = new(this);

                timeoutTask = Task.Run(async () =>
                {
                    await Task.Delay(timeout);
                    cancellationTokenSource.Cancel();
                });

                jobTask = Task.Run(
                    () => result = job.Invoke(),
                    cancellationToken
                );

                firstCompletedTask = await Task.WhenAny(timeoutTask, jobTask);

                if (ReferenceEquals(timeoutTask, firstCompletedTask))
                {
                    onTimeout.Invoke();
                    return;
                }

                if (jobTask.Exception != null)
                    onError.Invoke(jobTask.Exception);
                else
                    onSuccess?.Invoke(result);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
            }
            finally
            {
                _ = Task.Run(() =>
                {
                    if (!timeoutTask.IsCompleted)
                        timeoutTask.Wait();

                    if (!jobTask.IsCompleted)
                        jobTask.Wait();

                    timeoutTask?.Dispose();
                    jobTask?.Dispose();
                    cancellationTokenSource?.Dispose();
                });
            }
        }

        /// <summary>
        /// Runs an unit of work with IoC scope
        /// </summary>
        /// <typeparam name="T">What returns <paramref name="job"/>.</typeparam>
        /// <param name="job">What to do.</param>
        /// <param name="timeout">Time to do it.</param>
        /// <param name="onSuccess">What to do if <paramref name="job"/> ends without exception.</param>
        /// <param name="onTimeout">What to do if <paramref name="timeout"/>.</param>
        /// <param name="onError">What to do if <paramref name="job"/> ends with exception.</param>
        public async Task RunUnitOfWorkAsync<T>(
            Func<IResolverContext, T> job,
            TimeSpan timeout,
            Action<T> onSuccess = null,
            Action onTimeout = null,
            Action<Exception> onError = null)
        {
            ThrowIfJobIsNull(job);
            ThrowIfTimeoutIsNotValid(timeout);

            CancellationTokenSource cancellationTokenSource = new();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            Task timeoutTask = null;
            Task jobTask = null;
            Task firstCompletedTask = null;
            T result = default;

            if (onTimeout == null)
                onTimeout = OnTimeoutDefaultAction;

            if (onError == null)
                onError = OnErrorDefaultAction;

            try
            {
                using BusyLifeTimeScope busyLifeTimeScope = new(this);

                timeoutTask = Task.Run(async () =>
                {
                    await Task.Delay(timeout);
                    cancellationTokenSource.Cancel();
                });

                jobTask = Task.Run(
                    () =>
                    {
                        using IResolverContext scope = Container.OpenScope();
                        result = job(scope);
                    }, cancellationToken
                );

                firstCompletedTask = await Task.WhenAny(timeoutTask, jobTask);

                if (ReferenceEquals(timeoutTask, firstCompletedTask))
                {
                    onTimeout.Invoke();
                    return;
                }

                if (jobTask.Exception != null)
                    onError.Invoke(jobTask.Exception);
                else
                    onSuccess?.Invoke(result);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
            }
            finally
            {
                _ = Task.Run(() =>
                {
                    if (!timeoutTask.IsCompleted)
                        timeoutTask.Wait();

                    if (!jobTask.IsCompleted)
                        jobTask.Wait();

                    timeoutTask?.Dispose();
                    jobTask?.Dispose();
                    cancellationTokenSource?.Dispose();
                });
            }
        }

        /// <summary>
        /// Runs an unit of work with IoC scope
        /// </summary>
        /// <typeparam name="T">What returns <paramref name="job"/>.</typeparam>
        /// <param name="job">What to do.</param>
        /// <param name="timeout">Time to do it.</param>
        /// <param name="onSuccess">What to do if <paramref name="job"/> ends without exception.</param>
        /// <param name="onTimeout">What to do if <paramref name="timeout"/>.</param>
        /// <param name="onError">What to do if <paramref name="job"/> ends with exception.</param>
        public async Task RunUnitOfWorkAsync<T>(
            Func<IResolverContext, Task<T>> job,
            TimeSpan timeout,
            Action<T> onSuccess = null,
            Action onTimeout = null,
            Action<Exception> onError = null)
        {
            ThrowIfJobIsNull(job);
            ThrowIfTimeoutIsNotValid(timeout);

            CancellationTokenSource cancellationTokenSource = new();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            Task timeoutTask = null;
            Task jobTask = null;
            Task firstCompletedTask = null;
            T result = default;

            if (onError == null)
                onError = OnErrorDefaultAction;

            if (onTimeout == null)
                onTimeout = OnTimeoutDefaultAction;

            try
            {
                using BusyLifeTimeScope busyLifeTimeScope = new(this);

                timeoutTask = Task.Run(async () =>
                {
                    await Task.Delay(timeout);
                    cancellationTokenSource.Cancel();
                });

                jobTask = Task.Run(
                    async () =>
                    {
                        using IResolverContext scope = Container.OpenScope();
                        result = await job(scope);
                    }, cancellationToken
                );

                firstCompletedTask = await Task.WhenAny(timeoutTask, jobTask);

                if (ReferenceEquals(timeoutTask, firstCompletedTask))
                {
                    onTimeout.Invoke();
                    return;
                }

                if (jobTask.Exception != null)
                    onError.Invoke(jobTask.Exception);
                else
                    onSuccess?.Invoke(result);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
            }
            finally
            {
                _ = Task.Run(() =>
                {
                    if (!timeoutTask.IsCompleted)
                        timeoutTask.Wait();

                    if (!jobTask.IsCompleted)
                        jobTask.Wait();

                    timeoutTask?.Dispose();
                    jobTask?.Dispose();
                    cancellationTokenSource?.Dispose();
                });
            }
        }

        /// <summary>
        /// Runs a cancellable unit of work.
        /// </summary>
        /// <param name="job">What to do.</param>
        /// <param name="token"></param>
        /// <param name="onSuccess">What to do if <paramref name="job"/> ends without exception.</param>
        /// <param name="onCancelled">What to do if <paramref name="token"/>.IsCancellationRequested is true.</param>
        /// <param name="onError">What to do if <paramref name="job"/> ends with exception.</param>
        public async Task RunCancellableUnitOfWorkAsync(
            Action<CancellationToken> job,
            CancellationToken token,
            Action onSuccess = null,
            Action onCancelled = null,
            Action<Exception> onError = null)
        {
            ThrowIfJobIsNull(job);
            Task jobTask = null;

            if (onError == null)
                onError = OnErrorDefaultAction;

            if (onCancelled == null)
                onCancelled = OnCancelledDefaultAction;

            try
            {
                using BusyLifeTimeScope busyLifeTimeScope = new(this);

                jobTask = Task.Run(
                    () => job(token), token
                );

                await jobTask;

                if (jobTask.Exception != null)
                    onError.Invoke(jobTask.Exception);
                else if (token.IsCancellationRequested)
                    onCancelled.Invoke();
                else
                    onSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                onError.Invoke(ex);
            }
            finally
            {
                _ = Task.Run(() =>
                {
                    if (!jobTask.IsCompleted)
                        jobTask.Wait();

                    jobTask?.Dispose();
                });
            }
        }

        /// <summary>
        /// Runs a cancellable unit of work.
        /// </summary>
        /// <typeparam name="T">What returns <paramref name="job"/>.</typeparam>
        /// <param name="job">What to do.</param>
        /// <param name="token"></param>
        /// <param name="onSuccess">What to do if <paramref name="job"/> ends without exception.</param>
        /// <param name="onCancelled">What to do if <paramref name="token"/>.IsCancellationRequested is true.</param>
        /// <param name="onError">What to do if <paramref name="job"/> ends with exception.</param>
        public async Task RunCancellableUnitOfWorkAsync<T>(
            Func<CancellationToken, Task<T>> job,
            CancellationToken token,
            Action onSuccess = null,
            Action onCancelled = null,
            Action<Exception> onError = null)
        {
            ThrowIfJobIsNull(job);
            Task jobTask = null;

            if (onError == null)
                onError = OnErrorDefaultAction;

            if (onCancelled == null)
                onCancelled = OnCancelledDefaultAction;

            try
            {
                using BusyLifeTimeScope busyLifeTimeScope = new(this);

                jobTask = Task.Run(
                    () => job(token), token
                );

                await jobTask;

                if (jobTask.Exception != null)
                    onError.Invoke(jobTask.Exception);
                else if (token.IsCancellationRequested)
                    onCancelled.Invoke();
                else
                    onSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                onError.Invoke(ex);
            }
            finally
            {
                _ = Task.Run(() =>
                {
                    if (!jobTask.IsCompleted)
                        jobTask.Wait();

                    jobTask?.Dispose();
                });
            }
        }

        /// <summary>
        /// Runs a cancellable unit of work with IoC scope
        /// </summary>
        /// <typeparam name="T">What returns <paramref name="job"/>.</typeparam>
        /// <param name="job">What to do.</param>
        /// <param name="token"></param>
        /// <param name="onSuccess">What to do if <paramref name="job"/> ends without exception.</param>
        /// <param name="onCancelled">What to do if <paramref name="token"/>.IsCancellationRequested is true.</param>
        /// <param name="onError">What to do if <paramref name="job"/> ends with exception.</param>
        public async Task RunCancellableUnitOfWorkAsync<T>(
            Func<IResolverContext, CancellationToken, Task<T>> job,
            CancellationToken token,
            Action onSuccess = null,
            Action onCancelled = null,
            Action<Exception> onError = null)
        {
            ThrowIfJobIsNull(job);
            Task jobTask = null;

            if (onError == null)
                onError = OnErrorDefaultAction;

            if (onCancelled == null)
                onCancelled = OnCancelledDefaultAction;

            try
            {
                using BusyLifeTimeScope busyLifeTimeScope = new(this);

                jobTask = Task.Run(
                    () =>
                    {
                        using IResolverContext scope = Container.OpenScope();
                        job(scope, token);
                    }, token
                );

                await jobTask;

                if (jobTask.Exception != null)
                    onError.Invoke(jobTask.Exception);
                else if (token.IsCancellationRequested)
                    onCancelled.Invoke();
                else
                    onSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                onError.Invoke(ex);
            }
            finally
            {
                _ = Task.Run(() =>
                {
                    if (!jobTask.IsCompleted)
                        jobTask.Wait();

                    jobTask?.Dispose();
                });
            }
        }

        public void ShowStatusMessage(string message)
        {
            MessagingCenter.Instance.Send(this, Constans.SHOW_STATUS_MESSAGE, message);
        }

        private void OnErrorDefaultAction(Exception ex)
        {
            ex.Log();
            ShowStatusMessage(GetResource(StringKey.global_error));
        }

        private void OnTimeoutDefaultAction()
        {
            ShowStatusMessage(GetResource(StringKey.global_timeout));
        }

        private void OnCancelledDefaultAction()
        {
            ShowStatusMessage(GetResource(StringKey.global_cancelled));
        }

        private void ThrowIfJobIsNull(Action job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));
        }

        private void ThrowIfJobIsNull<T>(Action<T> job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));
        }

        private void ThrowIfJobIsNull<T>(Func<T> job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));
        }

        private void ThrowIfJobIsNull<T, K>(Func<T, K> job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));
        }

        private void ThrowIfJobIsNull<T, K, S>(Func<T, K, S> job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));
        }

        private void ThrowIfTimeoutIsNotValid(TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero)
                throw new ArgumentException($"\"{nameof(timeout)}\" cannot be lower or equals than zero", nameof(timeout));
        }
    }
}
