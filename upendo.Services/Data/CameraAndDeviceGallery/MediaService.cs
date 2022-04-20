using System.Threading.Tasks;
using upendo.CrossCutting.Entities;
using upendo.CrossCutting.Entities.Enums;
using upendo.CrossCutting.Interfaces.Data.CameraAndDeviceGallery;
using Xamarin.Essentials;

namespace upendo.Services.Data.CameraAndDeviceGallery
{
    public class MediaService : IMediaService
    {
        public async Task<MediaItem> PickPhotoAsync()
        {
            FileResult photo = await MediaPicker.PickPhotoAsync();

            if (photo == null || string.IsNullOrEmpty(photo.FullPath))
                return null;

            return new MediaItem
            {
                FilePath = photo.FullPath,
                Source = MediaSource.Picked,
                Type = MediaType.Photo,
                Stream = await photo.OpenReadAsync()
            };
        }

        public async Task<MediaItem> CapturePhotoAsync()
        {
            FileResult photo = await MediaPicker.CapturePhotoAsync();

            if (photo == null || string.IsNullOrEmpty(photo.FullPath))
                return null;

            return new MediaItem
            {
                FilePath = photo.FullPath,
                Source = MediaSource.Captured,
                Type = MediaType.Photo,
                Stream = await photo.OpenReadAsync()
            };
        }

        public async Task<MediaItem> PickVideoAsync()
        {
            FileResult video = await MediaPicker.PickVideoAsync();

            if (video == null || string.IsNullOrEmpty(video.FullPath))
                return null;

            return new MediaItem
            {
                FilePath = video.FullPath,
                Source = MediaSource.Picked,
                Type = MediaType.Video,
                Stream = await video.OpenReadAsync()
            };
        }

        public async Task<MediaItem> CaptureVideoAsync()
        {
            FileResult video = await MediaPicker.CaptureVideoAsync();

            if (video == null || string.IsNullOrEmpty(video.FullPath))
                return null;

            return new MediaItem
            {
                FilePath = video.FullPath,
                Source = MediaSource.Captured,
                Type = MediaType.Video,
                Stream = await video.OpenReadAsync()
            };
        }
    }
}
