using TarkovSauce.MapTools;

namespace TarkovSauce.Client.Data.Providers
{
    public interface IScreenshotWatcherProvider : IProvider
    {
        GameCoord? Position { get; }
        void NewScreenshot(string[] pos);
    }
    internal class ScreenshotWatcherProvider : IScreenshotWatcherProvider
    {
        public Action? OnStateChanged { get; set; }
        public GameCoord? Position { get; private set; }
        public void NewScreenshot(string[] pos)
        {
            _ = float.TryParse(pos[0], out float x);
            _ = float.TryParse(pos[1], out float y);
            _ = float.TryParse(pos[2], out float z);
            Position = new GameCoord(x, y, z);
            OnStateChanged?.Invoke();
        }
    }
}
