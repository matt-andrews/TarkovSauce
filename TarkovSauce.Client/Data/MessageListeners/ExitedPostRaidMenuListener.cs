using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Data.MessageListeners
{
    internal class ExitedPostRaidMenuListener(IRawLogProvider _rawLogProvider) : BaseMessageListener(_rawLogProvider)
    {
        public override string Match => "application|Init: pstrGameVersion: ";

        protected override void OnEventImpl(string str)
        {
        }
    }
}
