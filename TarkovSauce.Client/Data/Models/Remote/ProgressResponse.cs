namespace TarkovSauce.Client.Data.Models.Remote
{
    public class ProgressResponse
    {
        public ProgressResponseData Data { get; set; } = new();
        public ProgressResponseMeta Meta { get; set; } = new();
    }
    public class ProgressResponseData
    {
        public List<ProgressResponseTask> TasksProgress { get; set; } = [];
        public List<ProgressResponseHideoutModules> HideoutModulesProgress { get; set; } = [];
        public string? DisplayName { get; set; }
        public string UserId { get; set; } = "";
        public int PlayerLevel { get; set; }
        public int GameEdition { get; set; }
        public string PmcFaction { get; set; } = "";
    }
    public class ProgressResponseTask
    {
        public string Id { get; set; } = "";
        public bool Complete { get; set; }
        public bool Invalid { get; set; }
        public bool Failed { get; set; }
    }
    public class ProgressResponseHideoutModules
    {
        public string Id { get; set; } = "";
        public bool Complete { get; set; }
    }
    public class ProgressResponseMeta
    {
        public string Self { get; set; } = "";
    }
}
