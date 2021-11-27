using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Machina.Engine
{
    public class MachinaFileSystem
    {
        public MachinaFileSystem(string gameTitle, string contentDirectory = "")
        {
            this.AppDataPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "NotExplosive", gameTitle);

            this.devPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", contentDirectory, "Content"));
            MachinaClient.Print(this.devPath, Directory.Exists(this.devPath));
        }

        public async void WriteStringToAppData(string data, string path, bool skipDevPath = false,
            Action onComplete = null)
        {
            var fullPath = Path.Combine(this.AppDataPath, path);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            await File.WriteAllTextAsync(fullPath, data);

#if DEBUG
            // In development mode we do this wacky thing where we want to write the file to the repo, no other scenario needs to worry about this
            if (GamePlatform.IsDesktop)
            {
                if (!skipDevPath)
                {
                    var fullContentPath = Path.Combine(this.devPath, path);
                    Directory.CreateDirectory(Path.GetDirectoryName(fullContentPath));
                    await File.WriteAllTextAsync(fullContentPath, data);
                    MachinaClient.Print("Saved:", fullContentPath);
                }
            }
#endif
            MachinaClient.Print("Saved:", fullPath);

            onComplete?.Invoke();
        }

        /// <summary>
        /// Path to users AppData folder (or platform equivalent)
        /// </summary>
        public string AppDataPath { get; }

        private readonly string devPath;

        public IEnumerable<string> GetFilesAt(string path, string suffix)
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

            foreach (var file in Directory.EnumerateFiles(Path.Join(this.AppDataPath, path), "*"))
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
        public async Task<string> ReadTextAppDataThenLocal(string path)
        {
            var appData = Path.Join(this.AppDataPath, path);
            if (File.Exists(appData))
            {
                Console.WriteLine($"Attempting to read: {appData}");
                var result = await GamePlatform.ReadTextFile(appData);
                Console.WriteLine($"Finished reading {appData}, found: {result}.");
                return result;
            }

            return await GamePlatform.ReadFileInContentDirectory(path);

            throw new FileNotFoundException();
        }
    }
}