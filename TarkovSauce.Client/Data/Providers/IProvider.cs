namespace TarkovSauce.Client.Data.Providers
{
    public interface IProvider
    {
        Action? OnStateChanged { get; set; }
    }
}
