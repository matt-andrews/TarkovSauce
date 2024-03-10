using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Data.MessageListeners
{
    internal class GroupMatchMemberReadyListener(IRawLogProvider _rawLogProvider) : BaseMessageListener(_rawLogProvider)
    {
        public override string Match => "Got notification | GroupMatchRaidReady";

        protected override void OnEventImpl(string str)
        {
        }
    }
}
