using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Terraria;
using UICustomizer.Common.Systems.Hooks;

namespace UICustomizer.Helpers
{
    /// <summary>
    /// Stores and retrieves named “layouts” of all UI hooks 
    /// (Chat, Hotbar, Map, InfoAccs, ClassicLife, ClassicMana, FancyLife, FancyMana, HorizontalLifeBar).
    /// Each layout is saved as a JSON dictionary: string → Vector2.
    /// </summary>
    public static class LayoutJsonHelper
    {
        private const string FolderName = "UICustomizerLayouts";
        public static string CurrentLayoutName { get; set; } = "Default";

        private static string GetLayoutsFolderPath()
        {
            string modDataPath = Path.Combine(Main.SavePath, FolderName);
            Directory.CreateDirectory(modDataPath);
            // Log.Info("Layouts folder created at: " + modDataPath);
            return modDataPath;
        }

        private static string GetLayoutFilePath(string layoutName)
        {
            string folder = GetLayoutsFolderPath();
            Log.Info($"Layouts folder path: {folder}");
            return Path.Combine(folder, $"{layoutName}.json");
        }

        public static void EnsureDefaultLayoutsExist()
        {
            // 1) DEFAULT layout (all zero)
            string defaultPath = GetLayoutFilePath("Default");
            if (!File.Exists(defaultPath))
            {
                var dict = new Dictionary<string, Vector2>
                {
                    ["ChatOffset"] = Vector2.Zero,
                    ["HotbarOffset"] = Vector2.Zero,
                    ["MapOffset"] = Vector2.Zero,
                    ["InfoAccsOffset"] = Vector2.Zero,
                    ["ClassicLifeOffset"] = Vector2.Zero,
                    ["ClassicManaOffset"] = Vector2.Zero,
                    ["FancyLifeOffset"] = Vector2.Zero,
                    ["FancyManaOffset"] = Vector2.Zero,
                    ["HorizontalLifeBarOffset"] = Vector2.Zero
                };
                WriteLayoutFile("Default", dict);
            }

            // 2) HOTBAR CENTERED layout
            string hbPath = GetLayoutFilePath("HotbarCentered");
            if (!File.Exists(hbPath))
            {
                var dict = new Dictionary<string, Vector2>
                {
                    ["ChatOffset"] = Vector2.Zero,
                    ["HotbarOffset"] = new Vector2(100, 0),
                    ["MapOffset"] = Vector2.Zero,
                    ["InfoAccsOffset"] = Vector2.Zero,
                    ["ClassicLifeOffset"] = Vector2.Zero,
                    ["ClassicManaOffset"] = Vector2.Zero,
                    ["FancyLifeOffset"] = Vector2.Zero,
                    ["FancyManaOffset"] = Vector2.Zero,
                    ["HorizontalLifeBarOffset"] = Vector2.Zero
                };
                WriteLayoutFile("HotbarCentered", dict);
            }

            string lastLayoutPath = GetLastLayoutFilePath();
            if (!File.Exists(lastLayoutPath))
            {
                File.WriteAllText(lastLayoutPath, "Default");
                CurrentLayoutName = "Default";
            }
        }

        public static string LoadLastLayoutName()
        {
            string lastPath = GetLastLayoutFilePath();
            if (!File.Exists(lastPath))
                return "Default";

            string text = File.ReadAllText(lastPath).Trim();
            return string.IsNullOrEmpty(text) ? "Default" : text;
        }

        private static string GetLastLayoutFilePath()
        {
            return Path.Combine(GetLayoutsFolderPath(), "LastLayout.txt");
        }

        public static void SaveLastLayout()
        {
            string lastPath = GetLastLayoutFilePath();
            File.WriteAllText(lastPath, CurrentLayoutName);
        }

        private static void LoadLastLayout()
        {
            string lastPath = GetLastLayoutFilePath();
            if (!File.Exists(lastPath))
            {
                CurrentLayoutName = "Default";
                return;
            }
            string text = File.ReadAllText(lastPath).Trim();
            var candidateJson = Path.Combine(GetLayoutsFolderPath(), text + ".json");
            CurrentLayoutName = File.Exists(candidateJson) ? text : "Default";
        }

        /// <summary>
        /// Writes a new JSON file named {layoutName}.json containing all offsets from the given dictionary.
        /// </summary>
        private static void WriteLayoutFile(string layoutName, Dictionary<string, Vector2> data)
        {
            try
            {
                string path = GetLayoutFilePath(layoutName);
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                Log.Info($"Failed to write layout '{layoutName}': {ex.Message}");
            }
        }

        public static void SaveActiveLayout(string layoutName)
        {
            var dict = new Dictionary<string, Vector2>
            {
                ["ChatOffset"] = new Vector2(ChatHook.OffsetX, ChatHook.OffsetY),
                ["HotbarOffset"] = new Vector2(HotbarHook.OffsetX, HotbarHook.OffsetY),
                ["MapOffset"] = new Vector2(MapHook.OffsetX, MapHook.OffsetY),
                ["InfoAccsOffset"] = new Vector2(InfoAccsHook.OffsetX, InfoAccsHook.OffsetY),
                ["ClassicLifeOffset"] = new Vector2(ClassicLifeHook.OffsetX, ClassicLifeHook.OffsetY),
                ["ClassicManaOffset"] = new Vector2(ClassicManaHook.OffsetX, ClassicManaHook.OffsetY),
                ["FancyLifeOffset"] = new Vector2(FancyLifeHook.OffsetX, FancyLifeHook.OffsetY),
                ["FancyManaOffset"] = new Vector2(FancyManaHook.OffsetX, FancyManaHook.OffsetY),
                ["HorizontalLifeBarOffset"] = new Vector2(HorizontalBarsHook.OffsetX, HorizontalBarsHook.OffsetY)
            };

            WriteLayoutFile(layoutName, dict);
            CurrentLayoutName = layoutName;
            SaveLastLayout();
            Log.Info($"[UICustomizer] Saved layout '{layoutName}'.");
        }

        /// <summary>
        /// Reads {layoutName}.json, deserializes into a Dictionary, and applies each offset to the corresponding hook.
        /// If the file does not exist, logs an error and does nothing.
        /// </summary>
        public static void ApplyLayout(string layoutName)
        {
            string path = GetLayoutFilePath(layoutName);
            if (!File.Exists(path))
            {
                Log.Info($"Layout file '{layoutName}.json' not found.");
                return;
            }

            try
            {
                string json = File.ReadAllText(path);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, Vector2>>(json);
                if (dict == null)
                {
                    Log.Info($"Failed to parse '{layoutName}.json'.");
                    return;
                }

                // Apply each offset; if a key is missing, skip it
                if (dict.TryGetValue("ChatOffset", out Vector2 chat))
                { ChatHook.OffsetX = chat.X; ChatHook.OffsetY = chat.Y; }
                if (dict.TryGetValue("HotbarOffset", out Vector2 hb))
                { HotbarHook.OffsetX = hb.X; HotbarHook.OffsetY = hb.Y; }
                if (dict.TryGetValue("MapOffset", out Vector2 map))
                { MapHook.OffsetX = map.X; MapHook.OffsetY = map.Y; }
                if (dict.TryGetValue("InfoAccsOffset", out Vector2 ia))
                { InfoAccsHook.OffsetX = ia.X; InfoAccsHook.OffsetY = ia.Y; }
                if (dict.TryGetValue("ClassicLifeOffset", out Vector2 cl))
                { ClassicLifeHook.OffsetX = cl.X; ClassicLifeHook.OffsetY = cl.Y; }
                if (dict.TryGetValue("ClassicManaOffset", out Vector2 cm))
                { ClassicManaHook.OffsetX = cm.X; ClassicManaHook.OffsetY = cm.Y; }
                if (dict.TryGetValue("FancyLifeOffset", out Vector2 fl))
                { FancyLifeHook.OffsetX = fl.X; FancyLifeHook.OffsetY = fl.Y; }
                if (dict.TryGetValue("FancyManaOffset", out Vector2 fm))
                { FancyManaHook.OffsetX = fm.X; FancyManaHook.OffsetY = fm.Y; }
                if (dict.TryGetValue("HorizontalLifeBarOffset", out Vector2 hl))
                { HorizontalBarsHook.OffsetX = hl.X; HorizontalBarsHook.OffsetY = hl.Y; }

                CurrentLayoutName = layoutName;
                Log.Info($"Applied layout '{layoutName}'.");

            }
            catch (Exception ex)
            {
                Log.Info($"Error applying layout '{layoutName}': {ex.Message}");
            }
        }

        public static void OpenLayoutFolder()
        {
            string folder = GetLayoutsFolderPath();
            if (Directory.Exists(folder))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = folder,
                    UseShellExecute = true
                });
            }
            else
            {
                Log.Info($"Layouts folder does not exist: {folder}");
            }
        }

        public static void OpenNewLayoutFile(string layoutName)
        {
            string basePath = GetLayoutFilePath(layoutName);
            string path = basePath;
            int counter = 1;

            // Check if the file exists and generate a new name if necessary
            while (File.Exists(path))
            {
                path = GetLayoutFilePath($"{layoutName}{counter}");
                counter++;
            }

            // Create the new layout file
            var dict = new Dictionary<string, Vector2>
            {
                ["ChatOffset"] = Vector2.Zero,
                ["HotbarOffset"] = Vector2.Zero,
                ["MapOffset"] = Vector2.Zero,
                ["InfoAccsOffset"] = Vector2.Zero,
                ["ClassicLifeOffset"] = Vector2.Zero,
                ["ClassicManaOffset"] = Vector2.Zero,
                ["FancyLifeOffset"] = Vector2.Zero,
                ["FancyManaOffset"] = Vector2.Zero,
                ["HorizontalLifeBarOffset"] = Vector2.Zero
            };
            WriteLayoutFile(Path.GetFileNameWithoutExtension(path), dict);

            // Open the newly created file
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }

        public static List<string> GetLayouts()
        {
            string folder = GetLayoutsFolderPath();
            if (!Directory.Exists(folder))
            {
                Log.Info($"Layouts folder does not exist: {folder}");
                return [];
            }

            return Directory
                .GetFiles(folder, "*.json")
                .Select(path => Path.GetFileNameWithoutExtension(path))
                .ToList();
        }

        // delete all layouts
        public static void DeleteAllLayouts()
        {
            string folder = GetLayoutsFolderPath();
            if (!Directory.Exists(folder))
            {
                Log.Info($"Layouts folder does not exist: {folder}");
                return;
            }

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
                Log.Info($"Error deleting layouts: {ex.Message}");
            }
        }
    }
}
