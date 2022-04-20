using System;
using System.IO;
using upendo.CrossCutting.Entities;
using upendo.CrossCutting.Entities.Enums;
using Xamarin.Forms;

namespace upendo.Models
{
    public class PickedOrCapturedMedia : BaseModel
    {
        private ImageSource imageSource;
        private readonly Stream stream;
        private readonly string filePath;

        public string FilePath => filePath;

        public ImageSource ImageSource
        {
            get => IsPhoto ? imageSource : null;
            private set
            {
                imageSource = value;
                NotifyPropertyChanged();
            }
        }

        public string Base64 { get; private set; }

        public MediaSource Source { get; private set; }

        public MediaType Type { get; private set; }

        public bool IsPhoto => Type == MediaType.Photo;

        public bool IsVideo => Type == MediaType.Video;

        public PickedOrCapturedMedia(MediaItem mediaItem)
        {
            if (mediaItem == null)
                throw new ArgumentException(nameof(mediaItem));

            filePath = mediaItem.FilePath;
            Source = mediaItem.Source;
            Type = mediaItem.Type;
            stream = mediaItem.Stream;

            Init();
        }

        private void Init()
        {
            Base64 = GetBase64StringFromFile(filePath);

            if (IsPhoto)
                ImageSource = ImageSource.FromStream(() => stream);  
        }

        private string GetBase64StringFromFile(string path)
        {
            byte[] bytes;

            using (FileStream fileStream = new(path, FileMode.Open, FileAccess.Read))
            {
                using MemoryStream memoryStream = new();
                    fileStream.CopyTo(memoryStream);
                    bytes = memoryStream.ToArray();
            }

            return Convert.ToBase64String(bytes);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (ImageSource != null)
                    ImageSource = null;

                stream?.Dispose();

                if (Source == MediaSource.Captured && File.Exists(filePath))
                    File.Delete(filePath);
            }

            if (!string.IsNullOrEmpty(Base64))
                Base64 = null;

            base.Dispose(disposing);
        }
    }
}
