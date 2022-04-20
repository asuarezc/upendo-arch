using System;
using System.Threading.Tasks;
using upendo.CrossCutting.Entities;
using upendo.CrossCutting.Interfaces.Data.CameraAndDeviceGallery;
using upendo.CrossCutting.Interfaces.Logic;

namespace upendo.Services.Logic
{
    public class CameraAndGalleryPageService : ICameraAndGalleryPageService
    {
        private readonly IMediaService mediaService;

        public CameraAndGalleryPageService(IMediaService mediaService)
        {
            this.mediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));
        }

        public async Task<MediaItem> PickPhotoAsync()
        {
            return await mediaService.PickPhotoAsync();
        }

        public async Task<MediaItem> CapturePhotoAsync()
        {
            return await mediaService.CapturePhotoAsync();
        }

        public async Task<MediaItem> PickVideoAsync()
        {
            return await mediaService.PickVideoAsync();
        }

        public async Task<MediaItem> CaptureVideoAsync()
        {
            return await mediaService.CaptureVideoAsync();
        }
    }
}
