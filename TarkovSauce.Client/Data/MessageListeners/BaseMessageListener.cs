using TarkovSauce.Client.Data.Providers;
using TarkovSauce.Watcher.Interfaces;

namespace TarkovSauce.Client.Data.MessageListeners
{
    public abstract class BaseMessageListener(IRawLogProvider _rawLogProvider) : IMessageListener
    {
        public abstract string Match { get; }
        protected abstract void OnEventImpl(string str);

        public void OnEvent(string str)
        {
            _rawLogProvider.AppendLog(str);
            OnEventImpl(str);
        }
    }
}
