using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Data.MessageListeners
{
    internal class RaidEndedListener(IRawLogProvider _rawLogProvider) : BaseMessageListener(_rawLogProvider)
    {
        public override string Match => "application|SelectProfile ProfileId:";

        protected override void OnEventImpl(string str)
        {
            //ListenerService.OnRaidEnded();
        }
    }
}
