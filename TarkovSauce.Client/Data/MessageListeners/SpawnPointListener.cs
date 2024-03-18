using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Data.MessageListeners
{
    internal class SpawnPointListener(IRawLogProvider _rawLogProvider, IScreenshotWatcherProvider _screenshotWatcherProvider) : BaseMessageListener(_rawLogProvider)
    {
        public override string Match => "application|Reason:PacketsQueue";

        protected override void OnEventImpl(string str)
        {
            var pos = str.Split('(')[1].Split(')')[0].Split(',');
            _screenshotWatcherProvider.NewScreenshot(pos);
        }
    }
}
