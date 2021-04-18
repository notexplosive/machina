using Machina.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Machina.Engine
{
    static class FileHelpers
    {
        public static async void WriteStringToAppData(string data, string path, bool skipDevPath = false, Action onComplete = null)
        {
            var fullPath = Path.Combine(MachinaGame.Current.appDataPath, path);
            await File.WriteAllTextAsync(fullPath, data);

#if DEBUG
            if (!skipDevPath)
            {
                fullPath = Path.Combine(MachinaGame.Current.devContentPath, path);
                await File.WriteAllTextAsync(fullPath, data);
            }
#endif

            MachinaGame.Print("Saved:", fullPath);
            MachinaGame.Print(data);

            onComplete?.Invoke();
        }

        public static IEnumerable<string> GetFilesAt(string path, string searchPattern)
        {
            var result = new List<string>();
            var foundNames = new HashSet<string>();

            foreach (var file in Directory.EnumerateFiles(Path.Combine(MachinaGame.Current.localContentPath, path), searchPattern))
            {
                foundNames.Add(Path.GetFileName(file));
                result.Add(file);
            }

            foreach (var file in Directory.EnumerateFiles(Path.Combine(MachinaGame.Current.appDataPath, path), searchPattern))
            {
                if (!foundNames.Contains(Path.GetFileName(file)))
                {
                    result.Add(file);
                }
            }

            return result;
        }

        public static async Task<string> ReadTextLocalThenAppData(string path)
        {
            var local = Path.Combine(MachinaGame.Current.localContentPath, path);
            if (File.Exists(local))
            {
                var result = await File.ReadAllTextAsync(local);
                return result;
            }

            var appData = Path.Combine(MachinaGame.Current.appDataPath, path);
            if (File.Exists(appData))
            {
                var result = await File.ReadAllTextAsync(appData);
                return result;
            }

            throw new FileNotFoundException();
        }
    }
}
