namespace TarkovSauce.Client
{
    public enum MessageType
    {
        PlayerMessage = 1,
        Insurance = 2,
        FleaMarket = 4,
        InsuranceReturn = 8,
        TaskStarted = 10,
        TaskFailed = 11,
        TaskFinished = 12,
        TwitchDrop = 13,
    }
    public enum TaskStatus
    {
        None = 0,
        Started = 10,
        Failed = 11,
        Finished = 12
    }
    public enum RaidType
    {
        Unknown,
        PMC,
        Scav
    }
    public enum GroupInviteType
    {
        Accepted,
        Sent
    }
}
