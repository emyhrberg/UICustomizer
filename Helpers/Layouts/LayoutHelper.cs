using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UICustomizer.Common.Systems.Hooks;
using static UICustomizer.Helpers.ResourceThemeHelper;

namespace UICustomizer.Helpers.Layouts
{
    public static class LayoutHelper
    {
        public static string CurrentLayoutName { get; set; } = "Default";

        #region save layouts

        public static void SaveActiveLayout()
        {
            string layoutName = "Active";

            ResourceThemeHelper.GetActiveTheme(out ResourceTheme currentTheme);

            var layoutData = new LayoutData
            {
                Theme = currentTheme,
                Positions = new Dictionary<string, Vector2>
                {
                    ["ChatOffset"] = new Vector2(ChatHook.OffsetX, ChatHook.OffsetY),
                    ["HotbarOffset"] = new Vector2(HotbarHook.OffsetX, HotbarHook.OffsetY),
                    ["MapOffset"] = new Vector2(MapHook.OffsetX, MapHook.OffsetY),
                    ["InfoAccsOffset"] = new Vector2(InfoAccsHook.OffsetX, InfoAccsHook.OffsetY),
                    ["ClassicLifeOffset"] = new Vector2(ClassicLifeHook.OffsetX, ClassicLifeHook.OffsetY),
                    ["ClassicManaOffset"] = new Vector2(ClassicManaHook.OffsetX, ClassicManaHook.OffsetY),
                    ["FancyLifeOffset"] = new Vector2(FancyLifeHook.OffsetX, FancyLifeHook.OffsetY),
                    ["FancyManaOffset"] = new Vector2(FancyManaHook.OffsetX, FancyManaHook.OffsetY),
                    ["HorizontalBarsOffset"] = new Vector2(HorizontalBarsHook.OffsetX, HorizontalBarsHook.OffsetY),
                    ["BarLifeTextOffset"] = new Vector2(BarLifeTextHook.OffsetX, BarLifeTextHook.OffsetY),
                    ["BarManaTextOffset"] = new Vector2(BarManaTextHook.OffsetX, BarManaTextHook.OffsetY),
                    ["BuffOffset"] = new Vector2(BuffHook.OffsetX, BuffHook.OffsetY)
                }
            };

            WriteLayoutFile(layoutName, layoutData);
            CurrentLayoutName = layoutName;
            SaveLastLayout();
            Log.Info($"Saved layout '{layoutName}' with theme '{currentTheme}'.");
        }

        public static void WriteLayoutFile(string layoutName, LayoutData data)
        {
            try
            {
                string path = FileHelper.GetLayoutFilePath(layoutName);
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to write layout '{layoutName}': {ex.Message}");
            }
        }

        public static void SaveLastLayout()
        {
            string lastPath = FileHelper.GetLastLayoutFilePath();
            File.WriteAllText(lastPath, CurrentLayoutName);
        }

        #endregion

        #region load layouts
        public static void ApplyLayout(string layoutName)
        {
            string path = FileHelper.GetLayoutFilePath(layoutName);
            if (!File.Exists(path))
            {
                Log.Warn($"Layout file '{layoutName}.json' not found.");
                return;
            }

            try
            {
                string json = File.ReadAllText(path);

                // Try to deserialize as new format first
                var layoutData = JsonConvert.DeserializeObject<LayoutData>(json);
                Dictionary<string, Vector2> positions;
                ResourceTheme theme = ResourceTheme.Classic;

                if (layoutData?.Positions != null)
                {
                    // New format with theme
                    positions = layoutData.Positions;
                    theme = layoutData.Theme;
                }
                else
                {
                    // Legacy format - just positions
                    positions = JsonConvert.DeserializeObject<Dictionary<string, Vector2>>(json);
                    if (positions == null)
                    {
                        Log.Error($"Failed to parse '{layoutName}.json'.");
                        return;
                    }
                }

                // Apply the resource theme first
                ResourceThemeHelper.SetResourceTheme(theme);

                // Apply positions
                ApplyPositions(positions);

                CurrentLayoutName = layoutName;
                Log.Info($"Applied layout '{layoutName}' with theme '{theme}'.");
            }
            catch (Exception ex)
            {
                Log.Error($"Error applying layout '{layoutName}': {ex.Message}");
            }
        }

        private static void ApplyPositions(Dictionary<string, Vector2> positions)
        {
            if (positions.TryGetValue("ChatOffset", out Vector2 chat))
            { ChatHook.OffsetX = chat.X; ChatHook.OffsetY = chat.Y; }
            if (positions.TryGetValue("HotbarOffset", out Vector2 hb))
            { HotbarHook.OffsetX = hb.X; HotbarHook.OffsetY = hb.Y; }
            if (positions.TryGetValue("MapOffset", out Vector2 map))
            { MapHook.OffsetX = map.X; MapHook.OffsetY = map.Y; }
            if (positions.TryGetValue("InfoAccsOffset", out Vector2 ia))
            { InfoAccsHook.OffsetX = ia.X; InfoAccsHook.OffsetY = ia.Y; }
            if (positions.TryGetValue("ClassicLifeOffset", out Vector2 cl))
            { ClassicLifeHook.OffsetX = cl.X; ClassicLifeHook.OffsetY = cl.Y; }
            if (positions.TryGetValue("ClassicManaOffset", out Vector2 cm))
            { ClassicManaHook.OffsetX = cm.X; ClassicManaHook.OffsetY = cm.Y; }
            if (positions.TryGetValue("FancyLifeOffset", out Vector2 fl))
            { FancyLifeHook.OffsetX = fl.X; FancyLifeHook.OffsetY = fl.Y; }
            if (positions.TryGetValue("FancyManaOffset", out Vector2 fm))
            { FancyManaHook.OffsetX = fm.X; FancyManaHook.OffsetY = fm.Y; }
            if (positions.TryGetValue("HorizontalBarsOffset", out Vector2 hl))
            { HorizontalBarsHook.OffsetX = hl.X; HorizontalBarsHook.OffsetY = hl.Y; }
            if (positions.TryGetValue("BarLifeTextOffset", out Vector2 blt))
            { BarLifeTextHook.OffsetX = blt.X; BarLifeTextHook.OffsetY = blt.Y; }
            if (positions.TryGetValue("BarManaTextOffset", out Vector2 bmt))
            { BarManaTextHook.OffsetX = bmt.X; BarManaTextHook.OffsetY = bmt.Y; }
            if (positions.TryGetValue("BuffOffset", out Vector2 buff))
            { BuffHook.OffsetX = buff.X; BuffHook.OffsetY = buff.Y; }
        }

        public static void LoadLastLayout()
        {
            string lastLayoutName = FileHelper.LoadLastLayoutName();
            ApplyLayout(lastLayoutName);
        }
        #endregion
    }
}