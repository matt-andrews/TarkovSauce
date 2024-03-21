using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using TarkovSauce.Watcher.Interfaces;

namespace TarkovSauce.Watcher
{
    internal partial class MessageFactory(List<IMessageListener> listeners, string checkpointPath) : IWatcherEventListener
    {
        public void OnMessage(string name, string message)
        {
            MatchCollection logMessages = LogPatternRegex().Matches(message);
            foreach (Match match in logMessages.Cast<Match>())
            {
                DateTime eventDate = ParseDateTime(match);
                var eventLine = match.Groups["message"].Value;
                var listener = listeners.FirstOrDefault(f => eventLine.Contains(f.Match));
                if (listener is null) continue;
                if (match.Groups["json"].Success)
                {
                    string json = match.Groups["json"].Value;
                    listener.OnEvent(json);
                }
                else
                {
                    listener.OnEvent(eventLine);
                }
            }
            File.WriteAllText(checkpointPath, DateTime.Now.ToString());
        }
        public void OnScreenshot(string filename)
        {
            var listener = listeners.FirstOrDefault(listener => listener.Match == "Screenshot");
            listener?.OnEvent(filename);
        }
        public void OnError(string name, string message, Exception ex)
        {
            throw ex;
        }

        private static DateTime ParseDateTime(Match match)
        {
            string value = match.Groups["date"].Value + " " + match.Groups["time"].Value.Split(" ")[0];
            string format = "yyyy-MM-dd HH:mm:ss.fff";
            _ = DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime eventDate);
            return eventDate;
        }

        [GeneratedRegex(@"(?<date>^\d{4}-\d{2}-\d{2}) (?<time>\d{2}:\d{2}:\d{2}\.\d{3} [+-]\d{2}:\d{2})\|(?<message>.+$)\s*(?<json>^{[\s\S]+?^})?", RegexOptions.Multiline)]
        private static partial Regex LogPatternRegex();
    }
}
