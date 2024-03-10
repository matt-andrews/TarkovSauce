using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Data.MessageListeners
{
    internal class RaidStartingListener(IRawLogProvider _rawLogProvider) : BaseMessageListener(_rawLogProvider)
    {
        public override string Match => "application|GameStarting";

        protected override void OnEventImpl(string str)
        {
            //ListenerService.OnRaidStarting();
        }
    }
}
