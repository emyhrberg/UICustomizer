using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Terraria.ModLoader;
using UICustomizer.Edit.Hooks;
using static UICustomizer.Edit.Helpers.ElementHelper;
using static UICustomizer.Edit.Helpers.MapThemeHelper;
using static UICustomizer.Edit.Helpers.ResourceThemeHelper;

namespace UICustomizer.Edit.Helpers
{
    public static class LayoutHelper
    {
        #region save layouts

        public static void WriteLayoutFile(string layoutName, LayoutData data)
        {
            try
            {
                string path = FileHelper.GetLayoutFilePath(layoutName);
                // Log.Info($"Attempting to write layout '{layoutName}' to: {path}");

                // Check if directory exists
                string directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Log.Info($"Creating directory: {directory}");
                    Directory.CreateDirectory(directory);
                }

                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(path, json);

                // Log.Info($"Successfully wrote layout '{layoutName}' ({json.Length} characters)");

                // Verify file was created
                if (!File.Exists(path))
                {
                    Log.Error($"File was not created: {path}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to write layout '{layoutName}': {ex.Message}");
            }
        }

        public static void SaveCurrentAs(string layoutName, bool setAsActiveInConfig = true)
        {
            if (string.IsNullOrWhiteSpace(layoutName))
                layoutName = "CustomLayout";

            GetActiveResourceTheme(out ResourceTheme currentTheme);
            GetActiveMapTheme(out MapTheme mapTheme);

            var data = new LayoutData
            {
                ResourceTheme = currentTheme,
                MapTheme = mapTheme,
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
                    [Element.Inventory] = new Vector2(InventoryHook.OffsetX, InventoryHook.OffsetY)
                }
            };

            WriteLayoutFile(layoutName, data);

            if (setAsActiveInConfig)
            {
                var cfg = ModContent.GetInstance<Config>();
                cfg.Layout = layoutName;
                Terraria.ModLoader.Config.ConfigManager.Save(cfg);
            }

            Log.Info($"Saved layout '{layoutName}' with resource theme '{currentTheme}' and map theme '{mapTheme}'.");
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

                var layoutData = JsonConvert.DeserializeObject<LayoutData>(json);
                Dictionary<Element, Vector2> positions;
                ResourceTheme resourceTheme = ResourceTheme.Classic;
                MapTheme mapTheme = MapTheme.Default;

                if (layoutData?.Offsets != null)
                {
                    positions = layoutData.Offsets.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    resourceTheme = layoutData.ResourceTheme;
                    mapTheme = layoutData.MapTheme;
                }
                else
                {
                    positions = JsonConvert.DeserializeObject<Dictionary<Element, Vector2>>(json);
                    if (positions == null)
                    {
                        Log.Error($"Failed to parse '{layoutName}.json'.");
                        return;
                    }
                }

                SetResourceTheme(resourceTheme);
                SetMapTheme(mapTheme);
                ApplyPositions(positions);

                Log.Info($"Applied layout '{layoutName}' with resource theme '{resourceTheme}' and map theme '{mapTheme}'.");
            }
            catch (Exception ex)
            {
                Log.Error($"Error applying layout '{layoutName}': {ex.Message}");
            }
        }

        public static void TryApplyLayoutFromConfig()
        {
            var cfg = ModContent.GetInstance<Config>();
            string name = cfg.Layout;

            if (string.IsNullOrWhiteSpace(name))
                return;

            string path = FileHelper.GetLayoutFilePath(name);
            if (!File.Exists(path))
            {
                Log.Warn($"Configured layout '{name}' not found.");
                return;
            }

            ApplyLayout(name);
        }

        #endregion

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
            GetActiveResourceTheme(out ResourceTheme currentTheme);

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

            FileHelper.OpenFileAtPath(path);
        }

        private static void ApplyPositions(Dictionary<Element, Vector2> positions)
        {
            if (positions.TryGetValue(Element.Chat, out Vector2 chat))
            { ChatHook.OffsetX = chat.X; ChatHook.OffsetY = chat.Y; }
            if (positions.TryGetValue(Element.Hotbar, out Vector2 hb))
            { HotbarHook.OffsetX = hb.X; HotbarHook.OffsetY = hb.Y; }
            if (positions.TryGetValue(Element.Map, out Vector2 map))
            { MapHook.OffsetX = map.X; MapHook.OffsetY = map.Y; }
            if (positions.TryGetValue(Element.InfoAccs, out Vector2 ia))
            { InfoAccsHook.OffsetX = ia.X; InfoAccsHook.OffsetY = ia.Y; }
            if (positions.TryGetValue(Element.ClassicLife, out Vector2 cl))
            { ClassicLifeHook.OffsetX = cl.X; ClassicLifeHook.OffsetY = cl.Y; }
            if (positions.TryGetValue(Element.ClassicMana, out Vector2 cm))
            { ClassicManaHook.OffsetX = cm.X; ClassicManaHook.OffsetY = cm.Y; }
            if (positions.TryGetValue(Element.FancyLife, out Vector2 fl))
            { FancyLifeHook.OffsetX = fl.X; FancyLifeHook.OffsetY = fl.Y; }
            if (positions.TryGetValue(Element.FancyMana, out Vector2 fm))
            { FancyManaHook.OffsetX = fm.X; FancyManaHook.OffsetY = fm.Y; }
            if (positions.TryGetValue(Element.HorizontalBars, out Vector2 hl))
            { HorizontalBarsHook.OffsetX = hl.X; HorizontalBarsHook.OffsetY = hl.Y; }
            if (positions.TryGetValue(Element.BarLifeText, out Vector2 blt))
            { BarLifeTextHook.OffsetX = blt.X; BarLifeTextHook.OffsetY = blt.Y; }
            if (positions.TryGetValue(Element.BarManaText, out Vector2 bmt))
            { BarManaTextHook.OffsetX = bmt.X; BarManaTextHook.OffsetY = bmt.Y; }
            if (positions.TryGetValue(Element.Buffs, out Vector2 buff))
            { BuffHook.OffsetX = buff.X; BuffHook.OffsetY = buff.Y; }
            if (positions.TryGetValue(Element.Inventory, out Vector2 inv))
            { InventoryHook.OffsetX = (int)inv.X; InventoryHook.OffsetY = (int)inv.Y; }
        }

        public static void ResetAllOffsets()
        {
            ChatHook.OffsetX = ChatHook.OffsetY = 0f;
            HotbarHook.OffsetX = HotbarHook.OffsetY = 0f;
            MapHook.OffsetX = MapHook.OffsetY = 0f;
            ClassicLifeHook.OffsetX = ClassicLifeHook.OffsetY = 0f;
            ClassicManaHook.OffsetX = ClassicManaHook.OffsetY = 0f;
            FancyLifeHook.OffsetX = FancyLifeHook.OffsetY = 0f;
            FancyManaHook.OffsetX = FancyManaHook.OffsetY = 0f;
            FancyLifeTextHook.OffsetX = FancyLifeTextHook.OffsetY = 0f;
            HorizontalBarsHook.OffsetX = HorizontalBarsHook.OffsetY = 0f;
            InfoAccsHook.OffsetX = InfoAccsHook.OffsetY = 0f;
            BuffHook.OffsetX = BuffHook.OffsetY = 0f;
            BarLifeTextHook.OffsetX = BarLifeTextHook.OffsetY = 0f;
            BarManaTextHook.OffsetX = BarManaTextHook.OffsetY = 0f;
            InventoryHook.OffsetX = InventoryHook.OffsetY = 0f;
            CraftingHook.OffsetX = CraftingHook.OffsetY = 0f;
            AccessoriesHook.OffsetX = AccessoriesHook.OffsetY = 0f;
            CraftWindowHook.OffsetX = CraftWindowHook.OffsetY = 0f;
        }
    }
}