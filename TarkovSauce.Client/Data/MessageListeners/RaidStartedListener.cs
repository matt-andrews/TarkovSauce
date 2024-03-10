using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Data.MessageListeners
{
    internal class RaidStartedListener(IRawLogProvider _rawLogProvider) : BaseMessageListener(_rawLogProvider)
    {
        public override string Match => "application|GameStarted";

        protected override void OnEventImpl(string str)
        {
            //ListenerService.OnRaidStarted();
        }
    }
}
