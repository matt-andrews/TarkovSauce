namespace TarkovSauce.Client.Data.EventArgs
{
    public class SystemChatMessageEventArgs : ChatMessageEventArgs
    {
        public new SystemChatMessage Message { get; set; } = new();
    }
}
