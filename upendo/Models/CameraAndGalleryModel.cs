namespace upendo.Models
{
    public class CameraAndGalleryModel : BaseModel
    {
        private PickedOrCapturedMedia mediaElement;
        public PickedOrCapturedMedia MediaElement
        {
            get => mediaElement;
            set
            {
                mediaElement = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(() => IsPhoto);
                NotifyPropertyChanged(() => IsVideo);
            }
        }

        public bool IsPhoto => MediaElement != null && MediaElement.IsPhoto;

        public bool IsVideo => MediaElement != null && MediaElement.IsVideo;
    }
}
