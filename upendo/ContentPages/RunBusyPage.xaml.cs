using upendo.Models;
using upendo.ViewModels;

namespace upendo.ContentPages
{
    public partial class RunBusyPage : RunBusyPageXaml
    {
        public RunBusyPage()
        {
            InitializeComponent();
        }
    }

    public class RunBusyPageXaml : BaseContentPage<RunBusyPageViewModel, RunBusyPageModel> { }
}
