using System.IO;
using upendo.CrossCutting.Entities.Enums;

namespace upendo.CrossCutting.Entities
{
    public class MediaItem
    {
        public MediaSource Source { get; set; }

        public MediaType Type { get; set; }

        public string FilePath { get; set; }

        public Stream Stream { get; set; }
    }
}
