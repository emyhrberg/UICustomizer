using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UICustomizer.Common.Systems.Hooks;
using static UICustomizer.Helpers.Layouts.MapThemeHelper;
using static UICustomizer.Helpers.Layouts.OffsetHelper;
using static UICustomizer.Helpers.Layouts.ResourceThemeHelper;

namespace UICustomizer.Helpers.Layouts
{
    public static class LayoutHelper
    {
        public static string CurrentLayoutName { get; set; } = "Default";

        #region save layouts

        public static void SaveActiveLayout()
        {
            string layoutName = "Active";

            GetActiveResourceTheme(out ResourceTheme currentTheme);
            GetActiveMapTheme(out MapTheme mapTheme);

            var layoutData = new LayoutData
            {
                ResourceTheme = currentTheme,
                MapTheme = mapTheme,
                Offsets = new Dictionary<Offset, Vector2>
                {
                    [Offset.Chat] = new(ChatHook.OffsetX, ChatHook.OffsetY),
                    [Offset.Hotbar] = new(HotbarHook.OffsetX, HotbarHook.OffsetY),
                    [Offset.Map] = new(MapHook.OffsetX, MapHook.OffsetY),
                    [Offset.InfoAccs] = new(InfoAccsHook.OffsetX, InfoAccsHook.OffsetY),
                    [Offset.ClassicLife] = new(ClassicLifeHook.OffsetX, ClassicLifeHook.OffsetY),
                    [Offset.ClassicMana] = new(ClassicManaHook.OffsetX, ClassicManaHook.OffsetY),
                    [Offset.FancyLife] = new(FancyLifeHook.OffsetX, FancyLifeHook.OffsetY),
                    [Offset.FancyMana] = new(FancyManaHook.OffsetX, FancyManaHook.OffsetY),
                    [Offset.HorizontalBars] = new(HorizontalBarsHook.OffsetX, HorizontalBarsHook.OffsetY),
                    [Offset.BarLifeText] = new(BarLifeTextHook.OffsetX, BarLifeTextHook.OffsetY),
                    [Offset.BarManaText] = new(BarManaTextHook.OffsetX, BarManaTextHook.OffsetY),
                    [Offset.Buffs] = new(BuffHook.OffsetX, BuffHook.OffsetY),
                    [Offset.Inventory] = new(InventoryHook.OffsetX, InventoryHook.OffsetY),
                }
            };

            WriteLayoutFile(layoutName, layoutData);
            CurrentLayoutName = layoutName;
            SaveLastLayout();
            Log.Info($"Saved layout '{layoutName}' with life theme '{currentTheme}'.");
        }

        // Emojis.
        public static void WriteLayoutFile(string layoutName, LayoutData data)
        {
            try
            {
                string path = FileHelper.GetLayoutFilePath(layoutName);
                Log.Info($"Attempting to write layout '{layoutName}' to: {path}");

                // Check if directory exists
                string directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Log.Info($"Creating directory: {directory}");
                    Directory.CreateDirectory(directory);
                }

                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(path, json);

                Log.Info($"✅ Successfully wrote layout '{layoutName}' ({json.Length} characters)");

                // Verify file was created
                if (File.Exists(path))
                {
                    Log.Info($"✅ Verified file exists: {path}");
                }
                else
                {
                    Log.Error($"❌ File was not created: {path}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"❌ Failed to write layout '{layoutName}': {ex.Message}");
                Log.Error($"Stack trace: {ex.StackTrace}");
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
                Dictionary<Offset, Vector2> positions;
                ResourceTheme resourceTheme = ResourceTheme.Classic; // Default fallback
                MapTheme mapTheme = MapTheme.Default; // Default fallback

                if (layoutData?.Offsets != null)
                {
                    // New format - extract positions and themes from layout data
                    positions = layoutData.Offsets.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    // Use the SAVED themes from the layout, not current themes
                    resourceTheme = layoutData.ResourceTheme;
                    mapTheme = layoutData.MapTheme;
                }
                else
                {
                    // Legacy format - just positions, use defaults for themes
                    positions = JsonConvert.DeserializeObject<Dictionary<Offset, Vector2>>(json);
                    if (positions == null)
                    {
                        Log.Error($"Failed to parse '{layoutName}.json'.");
                        return;
                    }
                    // Keep default themes for legacy layouts
                }

                // Apply the SAVED themes from the layout
                SetResourceTheme(resourceTheme);
                SetMapTheme(mapTheme);

                // Apply positions
                ApplyPositions(positions);

                CurrentLayoutName = layoutName;
                Log.Info($"Applied layout '{layoutName}' with resource theme '{resourceTheme}' and map theme '{mapTheme}'.");
                Main.NewText($"Applied layout '{layoutName}' with resource theme '{resourceTheme}' and map theme '{mapTheme}'.", Color.LightGreen);
            }
            catch (Exception ex)
            {
                Log.Error($"Error applying layout '{layoutName}': {ex.Message}");
            }
        }

        private static void ApplyPositions(Dictionary<Offset, Vector2> positions)
        {
            if (positions.TryGetValue(Offset.Chat, out Vector2 chat))
            { ChatHook.OffsetX = chat.X; ChatHook.OffsetY = chat.Y; }
            if (positions.TryGetValue(Offset.Hotbar, out Vector2 hb))
            { HotbarHook.OffsetX = hb.X; HotbarHook.OffsetY = hb.Y; }
            if (positions.TryGetValue(Offset.Map, out Vector2 map))
            { MapHook.OffsetX = map.X; MapHook.OffsetY = map.Y; }
            if (positions.TryGetValue(Offset.InfoAccs, out Vector2 ia))
            { InfoAccsHook.OffsetX = ia.X; InfoAccsHook.OffsetY = ia.Y; }
            if (positions.TryGetValue(Offset.ClassicLife, out Vector2 cl))
            { ClassicLifeHook.OffsetX = cl.X; ClassicLifeHook.OffsetY = cl.Y; }
            if (positions.TryGetValue(Offset.ClassicMana, out Vector2 cm))
            { ClassicManaHook.OffsetX = cm.X; ClassicManaHook.OffsetY = cm.Y; }
            if (positions.TryGetValue(Offset.FancyLife, out Vector2 fl))
            { FancyLifeHook.OffsetX = fl.X; FancyLifeHook.OffsetY = fl.Y; }
            if (positions.TryGetValue(Offset.FancyMana, out Vector2 fm))
            { FancyManaHook.OffsetX = fm.X; FancyManaHook.OffsetY = fm.Y; }
            if (positions.TryGetValue(Offset.HorizontalBars, out Vector2 hl))
            { HorizontalBarsHook.OffsetX = hl.X; HorizontalBarsHook.OffsetY = hl.Y; }
            if (positions.TryGetValue(Offset.BarLifeText, out Vector2 blt))
            { BarLifeTextHook.OffsetX = blt.X; BarLifeTextHook.OffsetY = blt.Y; }
            if (positions.TryGetValue(Offset.BarManaText, out Vector2 bmt))
            { BarManaTextHook.OffsetX = bmt.X; BarManaTextHook.OffsetY = bmt.Y; }
            if (positions.TryGetValue(Offset.Buffs, out Vector2 buff))
            { BuffHook.OffsetX = buff.X; BuffHook.OffsetY = buff.Y; }
            if (positions.TryGetValue(Offset.Inventory, out Vector2 inv))
            { InventoryHook.OffsetX = (int)inv.X; InventoryHook.OffsetY = (int)inv.Y; }
        }

        public static void LoadLastLayout()
        {
            string lastLayoutName = FileHelper.LoadLastLayoutName();
            ApplyLayout(lastLayoutName);
        }
        #endregion
    }
}