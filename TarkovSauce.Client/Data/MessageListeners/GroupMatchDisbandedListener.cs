using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Data.MessageListeners
{
    internal class GroupMatchDisbandedListener(IRawLogProvider _rawLogProvider) : BaseMessageListener(_rawLogProvider)
    {
        public override string Match => "Got notification | GroupMatchWasRemoved";

        protected override void OnEventImpl(string str)
        {
        }
    }
}
