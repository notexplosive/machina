using Android.App;
using Android.Views;
using Machina.Engine;
using Machina.Engine.Cartridges;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MachinaAndroid
{
    /*
    [Activity(
        Label = "@string/app_name",
        MainLauncher = true,
        Icon = "@drawable/icon",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleTask,
        ScreenOrientation = ScreenOrientation.FullSensor,
        ConfigurationChanges =
            ConfigChanges.Orientation |
            ConfigChanges.Keyboard |
            ConfigChanges.KeyboardHidden |
            ConfigChanges.ScreenSize |
            ConfigChanges.ScreenLayout |
            ConfigChanges.UiMode |
            ConfigChanges.SmallestScreenSize
    )]
    public class Activity1 : AndroidGameActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            MachinaAndroid.AndroidBootstrap.Run(new MyGame(), new Machina.Engine.GameSpecification("MyGameTitle", Array.Empty<string>(), new Machina.Data.GameSettings(new Point(1920,1080))));
        }
    }
    */

    public static class AndroidBootstrap
    {
        public static void Run(GameCartridge cartridge, GameSpecification spec, Activity activity)
        {
            GamePlatform.Set(PlatformType.Android, GetFilesAtContentDirectory_Android, ReadFileInContentDirectory_Android, ReadTextFile_Android);

            // I don't think I need these but they might be useful
            // activity.Window.AddFlags(WindowManagerFlags.Fullscreen);
            // activity.Window.AddFlags(WindowManagerFlags.LayoutInOverscan);

            var game = new MachinaGame(spec, cartridge, new AndroidPlatformContext());
            var view = game.Services.GetService(typeof(View)) as View;
            view.SystemUiVisibility =
                (StatusBarVisibility) (SystemUiFlags.LayoutStable | SystemUiFlags.LayoutHideNavigation | SystemUiFlags.LayoutFullscreen | SystemUiFlags.HideNavigation | SystemUiFlags.Fullscreen | SystemUiFlags.ImmersiveSticky);
            activity.SetContentView(view);
            game.Run();
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

        public static Task<string> ReadFileInContentDirectory_Android(string pathInContent)
        {
            using (var file = new StreamReader(Android.App.Application.Context.Assets.Open(Path.Join("Content", pathInContent))))
            {
                var content = file.ReadToEnd();
                return Task.FromResult(content);
            };
            throw new FileNotFoundException(pathInContent);
        }


        public static Task<string> ReadTextFile_Android(string pathToFile)
        {
            using (var file = new StreamReader(pathToFile))
            {
                var content = file.ReadToEnd();
                return Task.FromResult(content);
            };
            throw new FileNotFoundException(pathToFile);
        }
    }
}
