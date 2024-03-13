using TarkovSauce.Client.Data.EventArgs;
using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Data.MessageListeners
{
    public class ChatMessageReceivedListener(IRawLogProvider _rawLogProvider,
        IFleaSalesProvider _fleaSalesProvider,
        ITarkovTrackerProvider _tarkovTrackerProvider) : BaseMessageListener(_rawLogProvider)
    {
        public override string Match => "Got notification | ChatMessageReceived";

        protected override void OnEventImpl(string str)
        {
            SystemChatMessageEventArgs? args = str.Deserialize<SystemChatMessageEventArgs>();

            if (args is null) return;

            if (args.Message.Type == MessageType.FleaMarket)
            {
                if (args is null)
                {
                    return;
                }
                if (args.Message.TemplateId == "5bdabfb886f7743e152e867e 0")
                {
                    _fleaSalesProvider.AppendSale(str.Deserialize<FleaSoldMessageEventArgs>());
                }
                else if (args.Message.TemplateId == "5bdabfe486f7743e1665df6e 0")
                {
                    _fleaSalesProvider.AppendExpiry(str.Deserialize<FleaExpiredeMessageEventArgs>());
                }
            }

            if (args.Message.Type >= MessageType.TaskStarted && args.Message.Type <= MessageType.TaskFinished)
            {
                var statusArgs = str.Deserialize<TaskStatusMessageEventArgs>();

                if (args.Message.Type == MessageType.TaskStarted)
                {
                    _tarkovTrackerProvider.OnTaskStarted(statusArgs);
                }
                else if (args.Message.Type == MessageType.TaskFailed)
                {
                    _tarkovTrackerProvider.OnTaskFailed(statusArgs);
                }
                else if (args.Message.Type == MessageType.TaskFinished)
                {
                    _tarkovTrackerProvider.OnTaskComplete(statusArgs);
                }
            }
        }
    }
}
