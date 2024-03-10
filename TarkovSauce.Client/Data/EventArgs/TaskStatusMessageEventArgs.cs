namespace TarkovSauce.Client.Data.EventArgs
{
    public class TaskStatusMessageEventArgs : ChatMessageEventArgs
    {
        public string TaskId => Message.TemplateId.Split(' ')[0];
        public TaskStatus Status => (TaskStatus)Message.Type;
        public new SystemChatMessage Message { get; set; } = new();
    }
}
