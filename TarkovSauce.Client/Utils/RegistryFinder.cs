using Microsoft.Win32;

namespace TarkovSauce.Client.Utils
{
    internal class RegistryFinder
    {
        public static string GetTarkovLogsLocation()
        {
#if WINDOWS
            string keyPath = "SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\EscapeFromTarkov";

            using RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath)
                ?? throw new Exception("EFT install registry entry not found");

            return Path.Combine(key.GetValue("InstallLocation")?.ToString()
                ?? throw new Exception("InstallLocation registry value not found"), "Logs");
#else
            return "";
#endif
        }
    }
}
