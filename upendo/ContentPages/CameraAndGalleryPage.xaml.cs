using upendo.Models;
using upendo.ViewModels;

namespace upendo.ContentPages
{
    public partial class CameraAndGalleryPage : CameraAndGalleryPageXaml
    {
        public CameraAndGalleryPage()
        {
            InitializeComponent();
        }
    }

    public class CameraAndGalleryPageXaml : BaseContentPage<CameraAndGalleryViewModel, CameraAndGalleryModel> { }
}
