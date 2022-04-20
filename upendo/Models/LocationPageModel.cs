using System;
using upendo.CrossCutting.Entities;

namespace upendo.Models
{
    public class LocationPageModel : BaseModel
    {
        private string addressLineOne;
        public string AddressLineOne
        {
            get => addressLineOne;
            set
            {
                addressLineOne = value;
                NotifyPropertyChanged();
            }
        }

        private string addressLineTwo;
        public string AddressLineTwo
        {
            get => addressLineTwo;
            set
            {
                addressLineTwo = value;
                NotifyPropertyChanged();
            }
        }

        public LocationPageModel() { }

        public LocationPageModel(Location location)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            AddressLineOne = location.AddressLineOne;
            AddressLineTwo = location.AddressLineTwo;
        }
    }
}
