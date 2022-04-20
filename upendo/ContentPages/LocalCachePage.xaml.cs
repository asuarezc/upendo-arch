using upendo.Models;
using upendo.ViewModels;

namespace upendo.ContentPages
{
    public partial class LocalCachePage : LocalCachePageXaml
    {
        public LocalCachePage()
        {
            InitializeComponent();
        }
    }

    public class LocalCachePageXaml : BaseContentPage<LocalCachePageViewModel, LocalCachePageModel> { }
}
