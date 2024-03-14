using System.Text.RegularExpressions;

namespace TarkovSauce.Watcher
{
    internal partial class FolderFinder
    {
        private readonly Dictionary<DateTime, string> _folders;
        private readonly string _path;

        public FolderFinder(string path)
        {
            _path = path;
            _folders = GetLogFolders();
        }

        public string GetLatestLogFolder() => _folders.OrderByDescending(o => o.Key).FirstOrDefault().Value;
        public List<string> GetLogFoldersNewerThan(DateTime dt) 
            => _folders.Where(folder => folder.Key.Date >= dt.Date).Select(s => s.Value).ToList();
        private Dictionary<DateTime, string> GetLogFolders()
        {
            Dictionary<DateTime, string> folderDictionary = [];
            if (string.IsNullOrEmpty(_path))
            {
                return folderDictionary;
            }

            var logFolders = Directory.GetDirectories(_path);
            foreach (string folderName in logFolders)
            {
                var dateTimeString = FolderNameRegex().Match(folderName).Groups["timestamp"].Value;
                DateTime folderDate = DateTime.ParseExact(dateTimeString, "yyyy.MM.dd_H-mm-ss", System.Globalization.CultureInfo.InvariantCulture);
                folderDictionary.Add(folderDate, folderName);
            }
            return folderDictionary.OrderBy(key => key.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        [GeneratedRegex(@"log_(?<timestamp>\d+\.\d+\.\d+_\d+-\d+-\d+)")]
        private static partial Regex FolderNameRegex();
    }
}
