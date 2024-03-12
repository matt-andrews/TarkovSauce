namespace TarkovSauce.Client.Data.Models.Remote
{
    public class TokenResponse
    {
        public List<string> Permissions { get; set; } = [];
        public string Token { get; set; } = "";
    }
}
