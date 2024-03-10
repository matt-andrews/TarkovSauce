using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Data.MessageListeners
{
    internal class MapLoadingListener(IRawLogProvider _rawLogProvider) : BaseMessageListener(_rawLogProvider)
    {
        public override string Match => "application|Matching with group id";

        protected override void OnEventImpl(string str)
        {
            //ListenerService.OnMapLoading();
        }
    }
}
