using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Data.MessageListeners
{
    internal class MatchStartedListener(IRawLogProvider _rawLogProvider) : BaseMessageListener(_rawLogProvider)
    {
        public override string Match => "application|TRACE-NetworkGameCreate profileStatus";

        protected override void OnEventImpl(string str)
        {
            //ListenerService.OnMatchStarted();
        }
    }
}
