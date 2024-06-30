namespace CQRSGenerator
{
    [Command(PackageIds.GenerateCommand)]
    internal sealed class GenerateCommand : BaseCommand<GenerateCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await VS.MessageBox.ShowWarningAsync("CQRSGenerator", "Button clicked");
        }
    }
}
