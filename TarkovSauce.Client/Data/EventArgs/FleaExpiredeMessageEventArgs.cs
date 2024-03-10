namespace TarkovSauce.Client.Data.EventArgs
{
    public class FleaExpiredeMessageEventArgs : JsonEventArgs
    {
        public string ItemId => Message.Items.Data[0].Id;
        public string ItemIdMask { get; set; } = "";
        public int ItemCount => Message.Items.Data[0].Upd?.StackObjectsCount ?? 1;
        public SystemChatMessageWithItems Message { get; set; } = new();
    }
}
