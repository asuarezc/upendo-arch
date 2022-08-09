namespace upendo.Models
{
    public class RunBusyPageModel : BaseModel
    {
        private double currentTimeoutSeconds;
        public double CurrentTimeoutSeconds
        {
            get => currentTimeoutSeconds;
            set
            {
                currentTimeoutSeconds = value;
                NotifyPropertyChanged();
            }
        }

        public RunBusyPageModel()
        {
            CurrentTimeoutSeconds = 5d;
        }
    }
}
