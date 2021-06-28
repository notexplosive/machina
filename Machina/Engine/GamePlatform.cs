﻿using System;
using System.Collections.Generic;
using System.IO;
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

        public static void Set(PlatformType platformType, Func<string, string, List<string>> getFilesAtContentDirectory)
        {
            GamePlatform.platformType = platformType;
            GetFilesAtContentDirectoryFunc = getFilesAtContentDirectory;
        }

        /// <summary>
        /// Platform is Desktop (Mac, PC, or Linux)
        /// </summary>
        public static bool IsDesktop => platformType == PlatformType.PC || platformType == PlatformType.Mac || platformType == PlatformType.Linux;
        /// <summary>
        /// Platform is Mobile (IOS or Android)
        /// </summary>
        public static bool IsMobile => platformType == PlatformType.Android || platformType == PlatformType.Ios;
        /// <summary>
        /// Platform is Android
        /// </summary>
        public static bool IsAndroid => platformType == PlatformType.Android;

        private static Func<string, string, List<string>> GetFilesAtContentDirectoryFunc = GetFilesAtContentDirectory_Desktop;

        private static List<string> GetFilesAtContentDirectory_Desktop(string contentSubFolder, string extension = "*")
        {
            var result = new List<string>();

            var contentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content");
            var path = Path.Join(contentPath, contentSubFolder);
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                // You need to add MachinaAssets as a project dependency to your game
                throw new DirectoryNotFoundException("Content folder missing, most likely missing MachinaAssets\ntried:" + path);
            }

            FileInfo[] files = dir.GetFiles("*." + extension);
            foreach (FileInfo file in files)
            {
                result.Add(Path.GetFileNameWithoutExtension(file.FullName));
            }
            return result;
        }

        public static List<string> GetFilesAtContentDirectory(string contentSubFolder, string extension = "*")
        {
            return GetFilesAtContentDirectoryFunc(contentSubFolder, extension);
        }
    }
}
