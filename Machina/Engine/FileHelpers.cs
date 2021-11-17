using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Machina.Engine
{
    public static class FileHelpers
    {
        public static async void WriteStringToAppData(string data, string path, bool skipDevPath = false,
            Action onComplete = null)
        {
            var fullPath = Path.Combine(MachinaGame.Current.appDataPath, path);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            await File.WriteAllTextAsync(fullPath, data);

#if DEBUG
            // In development mode we do this wacky thing where we want to write the file to the repo, no other scenario needs to worry about this
            if (GamePlatform.IsDesktop)
            {
                if (!skipDevPath)
                {
                    var devContentPath =
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Content");
                    var fullContentPath = Path.Combine(devContentPath, path);
                    Directory.CreateDirectory(Path.GetDirectoryName(fullContentPath));
                    await File.WriteAllTextAsync(fullContentPath, data);
                    MachinaGame.Print("Saved:", fullContentPath);
                }
            }
#endif
            MachinaGame.Print("Saved:", fullPath);

            onComplete?.Invoke();
        }

        public static string GetAppDataPath()
        {
            return MachinaGame.Current.appDataPath;
        }

        public static IEnumerable<string> GetFilesAt(string path, string suffix)
        {
            var result = new List<string>();
            var foundNames = new HashSet<string>();

            var contentFiles = GamePlatform.GetFilesAtContentDirectory(path);

            foreach (var file in contentFiles)
            {
                if (file.EndsWith(suffix))
                {
                    foundNames.Add(Path.GetFileName(file));
                    result.Add(file);
                }
            }

            foreach (var file in Directory.EnumerateFiles(Path.Combine(MachinaGame.Current.appDataPath, path), "*"))
            {
                if (file.EndsWith(suffix) && !foundNames.Contains(Path.GetFileName(file)))
                {
                    result.Add(file);
                }
            }

            return result;
        }

        /// <summary>
        ///     Checks to see if path exists in AppData. If it's there, read it.
        ///     If it's not there, check the local directory instead.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<string> ReadTextAppDataThenLocal(string path)
        {
            var appData = Path.Combine(MachinaGame.Current.appDataPath, path);
            if (File.Exists(appData))
            {
                var result = await File.ReadAllTextAsync(appData);
                return result;
            }

            return await GamePlatform.ReadFileInContentDirectory(path);

            throw new FileNotFoundException();
        }
    }
}