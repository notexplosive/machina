using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachinaDesktop
{
    public class DesktopPlatformContext : IPlatformContext
    {
        public void OnCartridgeSetup(Cartridge cartridge, MachinaWindow window)
        {
            window.GameWindow.TextInput += cartridge.SceneLayers.AddPendingTextInput;
        }
    }
}
