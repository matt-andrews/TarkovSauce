namespace TarkovSauce.Client.Data.Models.Remote
{
    public class TaskModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public int MinPlayerLevel { get; set; }
        public ObjectiveModel[] Objectives { get; set; } = [];
        public TaskRequirementsModel[] TaskRequirements { get; set; } = [];
    }
    public class TaskRequirementsModel
    {
        public TaskRequirementTaskModel Task { get; set; } = new();
        public string[] Status { get; set; } = [];
    }
    public class TaskRequirementTaskModel
    {
        public string Id { get; set; } = "";
    }
    public class ObjectiveModel
    {
        public string Id { get; set; } = "";
        public string Type { get; set; } = "";
        public string Description { get; set; } = "";
        public MapModel[] Maps { get; set; } = [];
        public ItemModel Item { get; set; } = new();
        public ItemModel[] Items { get; set; } = [];
        public int Count { get; set; }
        public bool FoundInRaid { get; set; }
    }
    public class MapModel
    {
        public string NormalizedName { get; set; } = "";
    }
    public class TasksWrapper
    {
        public TaskModel[]? Tasks { get; set; }
    }
    public class TasksGraphQLWrapper
    {
        public TasksWrapper? Data { get; set; }
    }
}
