namespace TarkovSauce.Client.Data.Models.Remote
{
    public class TaskStatusBody
    {
        public string? Id { get; set; }
        public string State { get; private set; }
        private TaskStatusBody(string newState)
        {
            State = newState;
        }
        public static TaskStatusBody Completed => new("completed");
        public static TaskStatusBody Uncompleted => new("uncompleted");
        public static TaskStatusBody Failed => new("failed");
        public static TaskStatusBody From(TaskStatus code)
        {
            if (code == TaskStatus.Finished)
            {
                return TaskStatusBody.Completed;
            }
            if (code == TaskStatus.Failed)
            {
                return TaskStatusBody.Failed;
            }
            return TaskStatusBody.Uncompleted;
        }
        public static TaskStatusBody From(MessageType messageType)
        {
            return TaskStatusBody.From((TaskStatus)messageType);
        }
    }
}
