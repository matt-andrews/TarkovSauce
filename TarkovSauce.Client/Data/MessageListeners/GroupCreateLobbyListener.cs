using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Data.MessageListeners
{
    internal class GroupCreateLobbyListener(IRawLogProvider _rawLogProvider) : BaseMessageListener(_rawLogProvider)
    {
        public override string Match => "Got notification | GroupMatchRaidSettings";

        protected override void OnEventImpl(string str)
        {
        }
    }
}
