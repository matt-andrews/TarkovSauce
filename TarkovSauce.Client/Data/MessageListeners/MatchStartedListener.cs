using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Data.MessageListeners
{
    internal class MatchStartedListener(IRawLogProvider _rawLogProvider, ISelectedMapProvider _selectedMapProvider) : BaseMessageListener(_rawLogProvider)
    {
        public override string Match => "application|TRACE-NetworkGameCreate profileStatus";

        protected override void OnEventImpl(string str)
        {
            var map = str.Split("Location:", StringSplitOptions.None)[1].Split(',')[0].Trim();
            _selectedMapProvider.SelectMap(map);
        }
    }
}
