using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Data.MessageListeners
{
    internal class MapLoadedListener(IRawLogProvider _rawLogProvider) : BaseMessageListener(_rawLogProvider)
    {
        public override string Match => "application|LocationLoaded";

        protected override void OnEventImpl(string str)
        {
            //float.Parse(Regex.Match(eventLine,
            //@"LocationLoaded:[0-9.,]+ real:(?<loadTime>[0-9.,]+)")
            //.Groups["loadTime"].Value.Replace(",", "."), CultureInfo.InvariantCulture)
            //ListenerService.OnMapLoaded();
        }
    }
}
