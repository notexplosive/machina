using Machina.Engine;

namespace Machina.Data
{
    public class App
    {
        public readonly string appName;

        private readonly bool isSingleton;
        private readonly WindowBuilder windowBuilder;
        private UIWindow latestInstance;

        public App(string appName, bool isSingleton, WindowBuilder windowBuilder)
        {
            this.appName = appName;
            this.isSingleton = isSingleton;
            this.windowBuilder = windowBuilder;
        }

        public UIWindow Launch(Scene parentScene, WindowManager windowManager)
        {
            if (this.isSingleton && this.latestInstance != null && this.latestInstance.IsOpen())
            {
                windowManager.SelectWindow(this.latestInstance);
                return this.latestInstance;
            }

            this.latestInstance = windowManager.CreateWindow(parentScene, this.windowBuilder);
            return this.latestInstance;
        }
    }
}