using Machina.Data;
using Machina.Engine.Cartridges;

namespace Machina.Engine
{
    /// <summary>
    /// A runtime that can exist inside the context of a "parent" runtime
    /// </summary>
    public class SubRuntime : IMachinaRuntime
    {
        private readonly IMachinaRuntime parent;
        private readonly UIWindow window;
        private readonly GameCartridge cartridge;

        public SubRuntime(IMachinaRuntime parent, UIWindow window, GameCartridge cartridge)
        {
            this.parent = parent;
            this.window = window;
            this.cartridge = cartridge;
        }

        public Painter Painter => this.parent.Painter;

        public IWindow WindowInterface => this.window;

        public DebugLevel DebugLevel
        {
            get => this.parent.DebugLevel;

            set => this.parent.DebugLevel = value;
        }

        public Cartridge CurrentCartridge => this.cartridge;

        public void Quit()
        {
            this.window.Close();
        }

        public void RunDemo(string demoPath)
        {
            MachinaClient.Print("Run demo in subruntime (not implemented)");
        }
    }
}