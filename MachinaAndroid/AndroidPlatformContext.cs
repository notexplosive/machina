﻿using Android.Content.PM;
using Android.OS;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Text;

namespace MachinaAndroid
{

    public class AndroidPlatformContext : IPlatformContext
    {
        public void OnCartridgeSetup(Cartridge cartridge, MachinaWindow window)
        {
        }

        public void OnGameConstructed(MachinaGame machinaGame)
        {
            MachinaClient.Runtime.Graphics.IsFullScreen = true;
        }
    }
}
