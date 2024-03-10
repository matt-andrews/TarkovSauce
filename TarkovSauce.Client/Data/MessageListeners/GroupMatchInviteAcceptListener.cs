using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Data.MessageListeners
{
    public class GroupMatchInviteAcceptListener(IRawLogProvider _rawLogProvider) : BaseMessageListener(_rawLogProvider)
    {
        public override string Match => "Got notification | GroupMatchInviteAccept";

        protected override void OnEventImpl(string str)
        {
        }
    }
}
