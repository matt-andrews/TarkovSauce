using System.Text.Json.Serialization;

namespace TarkovSauce.Client.Data.Models.Remote
{
    public class TaskStatusBody
    {
        [JsonPropertyName("id")]
        public string Id { get; private set; }
        [JsonPropertyName("state")]
        public string State { get; private set; }
        private TaskStatusBody(string id, string newState)
        {
            Id = id;
            State = newState;
        }
        public static TaskStatusBody From(string id, TaskStatus code)
        {
            if (code == TaskStatus.Finished)
            {
                return new TaskStatusBody(id, "completed");
            }
            if (code == TaskStatus.Failed)
            {
                return new TaskStatusBody(id, "failed");
            }
            return new TaskStatusBody(id, "uncompleted");
        }
    }
}
