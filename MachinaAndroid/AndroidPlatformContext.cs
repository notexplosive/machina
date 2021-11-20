using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Machina.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MachinaAndroid
{
    public class MachinaBootstrap
    {
        public static void Run()
        {
            GamePlatform.Set(PlatformType.Android, GetFilesAtContentDirectory_Android, ReadFileInContentDirectory_Android, ReadTextFile_Android);
        }

        private static List<string> GetFilesAtContentDirectory_Android(string contentSubfolder, string extension = "*")
        {
            var list = Android.App.Application.Context.Assets.List(Path.Join("Content", contentSubfolder));
            var result = new List<string>();

            foreach (var fileName in list)
            {
                if (Path.GetExtension(fileName) == "." + extension || extension == "*")
                    result.Add(fileName);
            }

            return result;
        }

        public async static Task<string> ReadFileInContentDirectory_Android(string pathInContent)
        {
            using (var file = new StreamReader(Android.App.Application.Context.Assets.Open(Path.Join("Content", pathInContent))))
            {
                var content = file.ReadToEnd();
                return content;
            };
            throw new FileNotFoundException(pathInContent);
        }


        public async static Task<string> ReadTextFile_Android(string pathToFile)
        {
            using (var file = new StreamReader(pathToFile))
            {
                var content = file.ReadToEnd();
                return content;
            };
            throw new FileNotFoundException(pathToFile);
        }
    }

    public class AndroidPlatformContext : IPlatformContext
    {
        public void OnCartridgeSetup(Cartridge cartridge, MachinaWindow window)
        {
        }
    }
}
