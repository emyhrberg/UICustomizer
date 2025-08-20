using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Terraria;
using UICustomizer.Edit.Hooks;
using static UICustomizer.Edit.Helpers.ElementHelper;

namespace UICustomizer.Edit.Helpers
{
    /// <summary>
    /// Handles file I/O operations for UICustomizer layouts.
    /// </summary>
    public static class FileHelper
    {
        private const string FolderName = "UICustomizerLayouts";

        public static string GetLayoutsFolderPath()
        {
            string modDataPath = Path.Combine(Main.SavePath, FolderName);
            Directory.CreateDirectory(modDataPath);
            return modDataPath;
        }

        public static string GetLayoutFilePath(string layoutName)
        {
            string folder = GetLayoutsFolderPath();
            return Path.Combine(folder, $"{layoutName}.json");
        }

        public static List<string> GetLayouts()
        {
            string folder = GetLayoutsFolderPath();
            if (!Directory.Exists(folder))
            {
                Log.Warn($"Layouts folder does not exist: {folder}");
                return [];
            }

            return Directory
                .GetFiles(folder, "*.json")
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();
        }

        public static void OpenFileAtPath(string path)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to open layout file: {ex.Message}");
            }
        }

        public static void OpenLayoutFolder()
        {
            string folder = GetLayoutsFolderPath();
            if (Directory.Exists(folder))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = folder,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to open layouts folder: {ex.Message}");
                }
            }
            else
            {
                Log.Warn($"Layouts folder does not exist: {folder}");
            }
        }
    }
}