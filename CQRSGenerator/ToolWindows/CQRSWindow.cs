using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CQRSGenerator
{
    public class CQRSWindow : BaseToolWindow<CQRSWindow>
    {
        public override string GetTitle(int toolWindowId) => "CQRS Generator";

        public override Type PaneType => typeof(Pane);

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            return Task.FromResult<FrameworkElement>(new CQRSWindowControl());
        }

        [Guid("ef15f585-e4c2-4de3-93c4-d4ec154ee7d4")] 
        internal class Pane : ToolkitToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.ToolWindow;
            }
        }
    }
}
