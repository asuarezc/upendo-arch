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
            }
        }
    }
}
