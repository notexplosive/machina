using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Machina.Engine
{
    public enum PlatformType
    {
        Unknown,
        PC,
        Android,
        Ios,
        Mac,
        Linux
    }

    public static class GamePlatform
    {
        private static PlatformType platformType = PlatformType.PC;

        private static Func<string, string, List<string>> GetFilesAtContentDirectoryFunc =
            GamePlatform.GetFilesAtContentDirectory_Desktop;

        private static Func<string, Task<string>> ReadFileInContentDirectoryFunc =
            GamePlatform.ReadFileInContentDirectory_Desktop;

        /// <summary>
        ///     Platform is Desktop (Mac, PC, or Linux). This means we have a mouse cursor
        /// </summary>
        public static bool IsDesktop => GamePlatform.platformType == PlatformType.PC ||
                                        GamePlatform.platformType == PlatformType.Mac ||
                                        GamePlatform.platformType == PlatformType.Linux;

        /// <summary>
        ///     Platform is Mobile (IOS or Android)
        /// </summary>
        public static bool IsMobile => GamePlatform.platformType == PlatformType.Android ||
                                       GamePlatform.platformType == PlatformType.Ios;

        /// <summary>
        ///     Platform is Android
        /// </summary>
        public static bool IsAndroid => GamePlatform.platformType == PlatformType.Android;

        public static void Set(PlatformType platformType, Func<string, string, List<string>> getFilesAtContentDirectory,
            Func<string, Task<string>> readFileInContentDirectory)
        {
            GamePlatform.platformType = platformType;
            GamePlatform.GetFilesAtContentDirectoryFunc = getFilesAtContentDirectory;
            GamePlatform.ReadFileInContentDirectoryFunc = readFileInContentDirectory;
        }

        private static List<string> GetFilesAtContentDirectory_Desktop(string contentSubFolder, string extension = "*")
        {
            var result = new List<string>();

            var contentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content");
            var path = Path.Join(contentPath, contentSubFolder);
            var dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                // You need to add MachinaAssets as a project dependency to your game
                throw new DirectoryNotFoundException(
                    "Content folder missing, most likely missing MachinaAssets\ntried:" + path);
            }

            var files = dir.GetFiles("*." + extension);
            foreach (var file in files)
            {
                result.Add(file.FullName);
            }

            return result;
        }

        private static async Task<string> ReadFileInContentDirectory_Desktop(string pathInContent)
        {
            var local = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content"), pathInContent);
            if (File.Exists(local))
            {
                var result = await File.ReadAllTextAsync(local);
                return result;
            }

            throw new FileNotFoundException();
        }

        public static async Task<string> ReadFileInContentDirectory(string path)
        {
            return await GamePlatform.ReadFileInContentDirectoryFunc(path);
        }

        public static string ReadFileInContentDirectory_Sync(string path)
        {
            return GamePlatform.ReadFileInContentDirectoryFunc(path).Result;
        }

        public static List<string> GetFilesAtContentDirectory(string contentSubFolder, string extension = "*")
        {
            return GamePlatform.GetFilesAtContentDirectoryFunc(contentSubFolder, extension);
        }
    }
}
