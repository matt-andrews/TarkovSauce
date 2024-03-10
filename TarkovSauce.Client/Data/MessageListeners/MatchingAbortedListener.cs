using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Data.MessageListeners
{
    internal class MatchingAbortedListener(IRawLogProvider _rawLogProvider) : BaseMessageListener(_rawLogProvider)
    {
        public override string Match => "application|Network game matching aborted";

        protected override void OnEventImpl(string str)
        {
            //ListenerService.OnMatchingAborted();
        }
    }
}
