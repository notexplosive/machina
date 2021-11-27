using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachinaDesktop
{
    public class DesktopPlatformContext : IPlatformContext
    {
        public void OnCartridgeSetup(Cartridge cartridge, OSWindow window)
        {
            window.GameWindow.TextInput += cartridge.SceneLayers.AddPendingTextInput;
        }

        public void OnGameConstructed(MachinaGame machinaGame)
        {
        }
    }
}
