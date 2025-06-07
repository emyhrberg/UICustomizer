using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UICustomizer.Common.Systems.Hooks;
using static UICustomizer.Helpers.Layouts.OffsetHelper;
using static UICustomizer.Helpers.Layouts.ResourceThemeHelper;

namespace UICustomizer.Helpers.Layouts
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

        public static string GetLastLayoutFilePath()
        {
            return Path.Combine(GetLayoutsFolderPath(), "LastLayout.txt");
        }

        public static string LoadLastLayoutName()
        {
            string lastPath = GetLastLayoutFilePath();
            if (!File.Exists(lastPath))
                return "Default";

            string text = File.ReadAllText(lastPath).Trim();
            return string.IsNullOrEmpty(text) ? "Default" : text;
        }

        #region file operations

        public static void CreateAndOpenNewLayoutFile(string layoutName)
        {
            string basePath = FileHelper.GetLayoutFilePath(layoutName);
            string path = basePath;
            int counter = 1;

            // Generate unique filename if needed
            while (File.Exists(path))
            {
                path = FileHelper.GetLayoutFilePath($"{layoutName}{counter}");
                counter++;
            }

            // Get current theme and positions
            ResourceThemeHelper.GetActiveResourceTheme(out ResourceTheme currentTheme);

            var layoutData = new LayoutData
            {
                ResourceTheme = currentTheme,
                Offsets = new Dictionary<Offset, Vector2>
                {
                    [Offset.Chat] = new Vector2(ChatHook.OffsetX, ChatHook.OffsetY),
                    [Offset.Hotbar] = new Vector2(HotbarHook.OffsetX, HotbarHook.OffsetY),
                    [Offset.Map] = new Vector2(MapHook.OffsetX, MapHook.OffsetY),
                    [Offset.InfoAccs] = new Vector2(InfoAccsHook.OffsetX, InfoAccsHook.OffsetY),
                    [Offset.ClassicLife] = new Vector2(ClassicLifeHook.OffsetX, ClassicLifeHook.OffsetY),
                    [Offset.ClassicMana] = new Vector2(ClassicManaHook.OffsetX, ClassicManaHook.OffsetY),
                    [Offset.FancyLife] = new Vector2(FancyLifeHook.OffsetX, FancyLifeHook.OffsetY),
                    [Offset.FancyMana] = new Vector2(FancyManaHook.OffsetX, FancyManaHook.OffsetY),
                    [Offset.HorizontalBars] = new Vector2(HorizontalBarsHook.OffsetX, HorizontalBarsHook.OffsetY),
                    [Offset.BarLifeText] = new Vector2(BarLifeTextHook.OffsetX, BarLifeTextHook.OffsetY),
                    [Offset.BarManaText] = new Vector2(BarManaTextHook.OffsetX, BarManaTextHook.OffsetY),
                    [Offset.Buffs] = new Vector2(BuffHook.OffsetX, BuffHook.OffsetY),
                    [Offset.Inventory] = new Vector2(InventoryHook.OffsetX, InventoryHook.OffsetY),
                }
            };

            LayoutHelper.WriteLayoutFile(Path.GetFileNameWithoutExtension(path), layoutData);

            // Open the newly created file
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

        public static List<string> GetLayouts()
        {
            string folder = FileHelper.GetLayoutsFolderPath();
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

        public static void OpenLayoutFile(string layoutName)
        {
            string path = GetLayoutFilePath(layoutName);
            if (!File.Exists(path))
            {
                Log.Warn($"Layout file '{layoutName}.json' not found.");
                return;
            }

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

        public static void DeleteAllLayouts()
        {
            string folder = GetLayoutsFolderPath();
            if (!Directory.Exists(folder))
                return;

            try
            {
                foreach (var file in Directory.GetFiles(folder, "*.json"))
                {
                    File.Delete(file);
                }
                Log.Info("All layouts deleted successfully.");
            }
            catch (Exception ex)
            {
                Log.Error($"Error deleting layouts: {ex.Message}");
            }
        }

        #endregion
    }
}