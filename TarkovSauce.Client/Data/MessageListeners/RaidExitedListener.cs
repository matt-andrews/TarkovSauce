using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Data.MessageListeners
{
    internal class RaidExitedListener(IRawLogProvider _rawLogProvider) : BaseMessageListener(_rawLogProvider)
    {
        public override string Match => "Got notification | UserMatchOver";

        protected override void OnEventImpl(string str)
        {
            //ListenerService.OnRaidExited();
        }
    }
}
