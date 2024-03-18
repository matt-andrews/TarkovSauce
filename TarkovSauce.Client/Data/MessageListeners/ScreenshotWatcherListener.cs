using System.Text.RegularExpressions;
using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Data.MessageListeners
{
    internal partial class ScreenshotWatcherListener(IRawLogProvider _rawLogProvider, IScreenshotWatcherProvider _screenshotWatcherProvider) 
        : BaseMessageListener(_rawLogProvider)
    {
        public override string Match => "Screenshot";
        protected override void OnEventImpl(string str)
        {
            var match = MatchPosFromFileName().Match(str);
            if (!match.Success)
            {
                return;
            }
            var position = MatchXyzFromPos().Match(match.Groups["position"].Value);
            if (!position.Success)
            {
                return;
            }
            _screenshotWatcherProvider.NewScreenshot([position.Groups["x"].Value, position.Groups["y"].Value, position.Groups["z"].Value]);
        }

        [GeneratedRegex(@"\d{4}-\d{2}-\d{2}\[\d{2}-\d{2}\]_(?<position>.+) \(\d\)\.png")]
        private static partial Regex MatchPosFromFileName();
        [GeneratedRegex(@"(?<x>-?[\d.]+), (?<y>-?[\d.]+), (?<z>-?[\d.]+)_.*")]
        private static partial Regex MatchXyzFromPos();
    }
}
