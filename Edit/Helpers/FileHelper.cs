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

        #region file operations

        public static void CreateAndOpenNewLayoutFile(string layoutName)
        {
            string basePath = GetLayoutFilePath(layoutName);
            string path = basePath;
            int counter = 1;

            // Generate unique filename if needed
            while (File.Exists(path))
            {
                path = GetLayoutFilePath($"{layoutName}{counter}");
                counter++;
            }

            // Get current theme and positions
            ResourceThemeHelper.GetActiveResourceTheme(out ResourceThemeHelper.ResourceTheme currentTheme);

            var layoutData = new LayoutData
            {
                ResourceTheme = currentTheme,
                Offsets = new Dictionary<Element, Vector2>
                {
                    [Element.Chat] = new Vector2(ChatHook.OffsetX, ChatHook.OffsetY),
                    [Element.Hotbar] = new Vector2(HotbarHook.OffsetX, HotbarHook.OffsetY),
                    [Element.Map] = new Vector2(MapHook.OffsetX, MapHook.OffsetY),
                    [Element.InfoAccs] = new Vector2(InfoAccsHook.OffsetX, InfoAccsHook.OffsetY),
                    [Element.ClassicLife] = new Vector2(ClassicLifeHook.OffsetX, ClassicLifeHook.OffsetY),
                    [Element.ClassicMana] = new Vector2(ClassicManaHook.OffsetX, ClassicManaHook.OffsetY),
                    [Element.FancyLife] = new Vector2(FancyLifeHook.OffsetX, FancyLifeHook.OffsetY),
                    [Element.FancyMana] = new Vector2(FancyManaHook.OffsetX, FancyManaHook.OffsetY),
                    [Element.HorizontalBars] = new Vector2(HorizontalBarsHook.OffsetX, HorizontalBarsHook.OffsetY),
                    [Element.BarLifeText] = new Vector2(BarLifeTextHook.OffsetX, BarLifeTextHook.OffsetY),
                    [Element.BarManaText] = new Vector2(BarManaTextHook.OffsetX, BarManaTextHook.OffsetY),
                    [Element.Buffs] = new Vector2(BuffHook.OffsetX, BuffHook.OffsetY),
                    [Element.Inventory] = new Vector2(InventoryHook.OffsetX, InventoryHook.OffsetY),
                }
            };

            LayoutHelper.WriteLayoutFile(Path.GetFileNameWithoutExtension(path), layoutData);

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
        #endregion
    }
}