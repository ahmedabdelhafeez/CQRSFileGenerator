namespace CQRSGenerator
{
    [Command(PackageIds.CQRSWindow)]
    internal sealed class OpenCQRSWindowCommand : BaseCommand<OpenCQRSWindowCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {

            await CQRSWindow.ShowAsync();
        }
    }
}
