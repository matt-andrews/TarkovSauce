using TarkovSauce.Client.Data.EventArgs;
using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Data.MessageListeners
{
    internal class GroupCreateLobbyListener(IRawLogProvider _rawLogProvider, ISelectedMapProvider _selectedMapProvider) : BaseMessageListener(_rawLogProvider)
    {
        public override string Match => "Got notification | GroupMatchRaidSettings";

        protected override void OnEventImpl(string str)
        {
            var args = str.Deserialize<GroupCreateLobbyEventArgs>();
            if (args is null) return;
            _selectedMapProvider.SelectMap(args);
        }
    }
}
