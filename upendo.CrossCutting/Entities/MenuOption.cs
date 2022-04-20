using upendo.CrossCutting.Entities.Enums;

namespace upendo.CrossCutting.Entities
{
    public class MenuOption
    {
        public string Description { get; set; }

        public string TopBarBackgroundHexColor { get; set; }

        public string OptionBackgroundHexColor { get; set; }

        public string OptionImage { get; set; }

        public string GoImage { get; set; }

        public AvaliablePageFromMenu NavigationPage { get; set; }
    }
}
