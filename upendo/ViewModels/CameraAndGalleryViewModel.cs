using System.Windows.Input;
using upendo.CrossCutting.Entities;
using DryIoc;
using upendo.Models;
using Xamarin.Forms;
using upendo.CrossCutting.Interfaces.Logic;
using System;
using System.Threading.Tasks;

namespace upendo.ViewModels
{
    public class CameraAndGalleryViewModel : BaseViewModel<CameraAndGalleryModel>
    {
        private enum CameraAndGalleryAction
        {
            PickImage,
            CaptureImage,
            PickVideo,
            CaptureVideo
        }

        private ICommand pickImageCommand;
        public ICommand PickImageCommand => pickImageCommand ??= new Command(
            async () => await DoCameraAndGalleryActionAsync(CameraAndGalleryAction.PickImage),
            () => IsNotBusy
        );

        private ICommand captureImageCommand;
        public ICommand CaptureImageCommand => captureImageCommand ??= new Command(
            async () => await DoCameraAndGalleryActionAsync(CameraAndGalleryAction.CaptureImage),
            () => IsNotBusy
        );

        private ICommand pickVideoCommand;
        public ICommand PickVideoCommand => pickVideoCommand ??= new Command(
            async () => await DoCameraAndGalleryActionAsync(CameraAndGalleryAction.PickVideo),
            () => IsNotBusy
        );

        private ICommand captureVideoCommand;
        public ICommand CaptureVideoCommand => captureVideoCommand ??= new Command(
            async () => await DoCameraAndGalleryActionAsync(CameraAndGalleryAction.CaptureVideo),
            () => IsNotBusy
        );

        private async Task DoCameraAndGalleryActionAsync(CameraAndGalleryAction action)
        {
            await RunUnitOfWorkAsync(
                job: async (IResolverContext scope) =>
                {
                    ICameraAndGalleryPageService service = scope.Resolve<ICameraAndGalleryPageService>();
                    MediaItem result = null;

                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        result = action switch
                        {
                            CameraAndGalleryAction.PickImage => await service.PickPhotoAsync(),
                            CameraAndGalleryAction.CaptureImage => await service.CapturePhotoAsync(),
                            CameraAndGalleryAction.PickVideo => await service.PickVideoAsync(),
                            CameraAndGalleryAction.CaptureVideo => await service.CaptureVideoAsync(),
                            _ => throw new InvalidOperationException(),
                        };
                    });

                    return result;
                },
                onSuccess: (MediaItem mediaItem) =>
                {
                    if (mediaItem != null)
                        Model.MediaElement = new PickedOrCapturedMedia(mediaItem);
                }
            );
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Model?.MediaElement?.Dispose();

            base.Dispose(disposing);
        }
    }
}
