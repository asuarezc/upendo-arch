using upendo.Models;
using upendo.ViewModels;

namespace upendo.ContentPages
{
    public partial class RestPage : RestPageXaml
    {
        public RestPage()
        {
            InitializeComponent();
        }
    }

    public class RestPageXaml : BaseContentPage<RestPageViewModel, RestPageModel> { }
}
