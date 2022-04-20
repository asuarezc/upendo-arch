using System;
using upendo.CrossCutting.Entities;

namespace upendo.Models
{
    public class UserModel : BaseModel
    {
        private string firstName;
        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                NotifyPropertyChanged();
            }
        }

        private string lastName;
        public string LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                NotifyPropertyChanged();
            }
        }

        private string email;
        public string Email
        {
            get => email;
            set
            {
                email = value;
                NotifyPropertyChanged();
            }
        }

        private Uri avatarUri;
        public Uri AvatarUri
        {
            get => avatarUri;
            set
            {
                avatarUri = value;
                NotifyPropertyChanged();
            }
        }

        public UserModel(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            AvatarUri = !string.IsNullOrEmpty(user.AvatarUrl)
                ? new Uri(user.AvatarUrl)
                : null;
        }
    }
}
