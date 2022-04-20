using System.Threading.Tasks;
using upendo.CrossCutting.Entities;

namespace upendo.CrossCutting.Interfaces.Data.CameraAndDeviceGallery
{
    public interface IMediaService
    {
        Task<MediaItem> CapturePhotoAsync();
        Task<MediaItem> CaptureVideoAsync();
        Task<MediaItem> PickPhotoAsync();
        Task<MediaItem> PickVideoAsync();
    }
}