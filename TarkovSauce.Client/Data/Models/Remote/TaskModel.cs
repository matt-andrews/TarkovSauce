using SQLite;

namespace TarkovSauce.Client.Data.Models.Remote
{
    public class TaskModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public int MinPlayerLevel { get; set; }
        public ObjectiveModel[] Objectives { get; set; } = [];
        public TaskRequirementsModel[] TaskRequirements { get; set; } = [];
        public TraderModel Trader { get; set; } = new();
        public TraderRequirementsModel[] TraderRequirements { get; set; } = [];
    }
    public class TaskModelFlat
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public int MinPlayerLevel { get; set; }
        public string Objectives { get; set; } = "";
        public string TaskRequirements { get; set; } = "";
        public string Trader { get; set; } = "";
        public string TraderRequirements { get; set; } = "";
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
    public class TraderModel
    {
        public string Name { get; set; } = "";
        public string ImageLink { get; set; } = "";
    }
    public class TraderRequirementsModel
    {
        public TraderModel Trader { get; set; } = new();
        public string RequirementType { get; set; } = "";
        public string CompareMethod { get; set; } = "";
        public int Value { get; set; }
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

    public static class TaskModelExtensions
    {
        public static IEnumerable<TaskModelFlat> Flatten(this IEnumerable<TaskModel> tasks)
        {
            foreach (var task in tasks)
            {
                yield return new TaskModelFlat()
                {
                    Id = task.Id,
                    Name = task.Name,
                    MinPlayerLevel = task.MinPlayerLevel,
                    Objectives = task.Objectives.Serialize(),
                    TaskRequirements = task.TaskRequirements.Serialize(),
                    Trader = task.Trader.Serialize(),
                    TraderRequirements = task.TraderRequirements.Serialize(),
                };
            }
        }
        public static IEnumerable<TaskModel> Expand(this IEnumerable<TaskModelFlat> tasks)
        {
            foreach(var task in tasks)
            {
                yield return new TaskModel()
                {
                    Id = task.Id,
                    Name = task.Name,
                    MinPlayerLevel = task.MinPlayerLevel,
                    Objectives = task.Objectives.Deserialize<ObjectiveModel[]>() ?? [],
                    TaskRequirements = task.TaskRequirements.Deserialize<TaskRequirementsModel[]>() ?? [],
                    Trader = task.Trader.Deserialize<TraderModel>() ?? new(),
                    TraderRequirements = task.TraderRequirements.Deserialize<TraderRequirementsModel[]>() ?? [],
                };
            }
        }
    }
}
