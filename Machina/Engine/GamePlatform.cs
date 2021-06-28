using System;
using System.Collections.Generic;
using System.Text;

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
        private static PlatformType platformType;

        public static void Set(PlatformType platformType)
        {
            GamePlatform.platformType = platformType;
        }

        public static bool IsDesktop => platformType == PlatformType.PC || platformType == PlatformType.Mac || platformType == PlatformType.Linux;
        public static bool IsMobile => platformType == PlatformType.Android || platformType == PlatformType.Ios;
        public static bool IsAndroid => platformType == PlatformType.Android;
    }
}
