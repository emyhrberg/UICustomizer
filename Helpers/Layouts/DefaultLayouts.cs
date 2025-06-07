using System.Collections.Generic;
using System.IO;
using static UICustomizer.Helpers.Layouts.MapThemeHelper;
using static UICustomizer.Helpers.Layouts.OffsetHelper;
using static UICustomizer.Helpers.Layouts.ResourceThemeHelper;

namespace UICustomizer.Helpers.Layouts
{
    public static class DefaultLayouts
    {
        public static void CreateAllDefaultLayouts()
        {
            // Ensure the layouts folder exists
            string layoutsFolder = FileHelper.GetLayoutsFolderPath();
            if (!Directory.Exists(layoutsFolder))
            {
                Directory.CreateDirectory(layoutsFolder);
            }

            CreateDefaultLayout();
            CreateHotbarCenteredLayout();
            CreateMapLeftLayout();
            CreateMinecraftLayout();
            CreateLastLayoutFile();
        }

        private static void CreateDefaultLayout()
        {
            const string layoutName = "Default";

            string defaultPath = FileHelper.GetLayoutFilePath(layoutName);
            if (!File.Exists(defaultPath))
            {
                var layoutData = new LayoutData
                {
                    ResourceTheme = ResourceTheme.Classic,
                    MapTheme = MapTheme.Default,
                    Offsets = new Dictionary<Offset, Vector2>
                    {
                        [Offset.Chat] = Vector2.Zero,
                        [Offset.Hotbar] = Vector2.Zero,
                        [Offset.Map] = Vector2.Zero,
                        [Offset.InfoAccs] = Vector2.Zero,
                        [Offset.ClassicLife] = Vector2.Zero,
                        [Offset.ClassicMana] = Vector2.Zero,
                        [Offset.FancyLife] = Vector2.Zero,
                        [Offset.FancyMana] = Vector2.Zero,
                        [Offset.HorizontalBars] = Vector2.Zero,
                        [Offset.BarLifeText] = Vector2.Zero,
                        [Offset.BarManaText] = Vector2.Zero,
                        [Offset.Buffs] = Vector2.Zero,
                        [Offset.Inventory] = Vector2.Zero,
                        [Offset.Crafting] = Vector2.Zero,
                        [Offset.Accessories] = Vector2.Zero
                    }
                };
                LayoutHelper.WriteLayoutFile(layoutName, layoutData);
            }
        }

        private static void CreateHotbarCenteredLayout()
        {
            const string layoutName = "Hotbar Center";
            string hbPath = FileHelper.GetLayoutFilePath(layoutName);
            if (!File.Exists(hbPath))
            {
                var layoutData = new LayoutData
                {
                    ResourceTheme = ResourceTheme.Bars,
                    MapTheme = MapTheme.Valkyrie,
                    Offsets = new Dictionary<Offset, Vector2>
                    {
                        [Offset.Chat] = Vector2.Zero,
                        [Offset.Hotbar] = new Vector2(160, 0),
                        [Offset.Map] = Vector2.Zero,
                        [Offset.InfoAccs] = Vector2.Zero,
                        [Offset.ClassicLife] = Vector2.Zero,
                        [Offset.ClassicMana] = Vector2.Zero,
                        [Offset.FancyLife] = Vector2.Zero,
                        [Offset.FancyMana] = Vector2.Zero,
                        [Offset.HorizontalBars] = Vector2.Zero,
                        [Offset.BarLifeText] = Vector2.Zero,
                        [Offset.BarManaText] = Vector2.Zero,
                        [Offset.Buffs] = Vector2.Zero,
                        [Offset.Inventory] = Vector2.Zero,
                        [Offset.Crafting] = Vector2.Zero,
                        [Offset.Accessories] = Vector2.Zero
                    }
                };
                LayoutHelper.WriteLayoutFile(layoutName, layoutData);
            }
        }

        private static void CreateMapLeftLayout()
        {
            const string layoutName = "Map Left";
            string mapLeftPath = FileHelper.GetLayoutFilePath(layoutName);
            if (!File.Exists(mapLeftPath))
            {
                var layoutData = new LayoutData
                {
                    ResourceTheme = ResourceTheme.Fancy2,
                    MapTheme = MapTheme.Golden,
                    Offsets = new Dictionary<Offset, Vector2>
                    {
                        [Offset.Hotbar] = new Vector2(230, -4),
                        [Offset.Map] = new Vector2(-658, -71),
                        [Offset.InfoAccs] = new Vector2(475, 286),
                        [Offset.FancyLife] = new Vector2(34, 273),
                        [Offset.FancyMana] = new Vector2(-258, 7),
                        [Offset.HorizontalBars] = new Vector2(38, -3),
                        [Offset.BarLifeText] = new Vector2(0, 0),
                        [Offset.BarManaText] = new Vector2(0, 0),
                        [Offset.Buffs] = new Vector2(0, 0),
                        [Offset.Inventory] = new Vector2(0, 0),
                        [Offset.Chat] = new Vector2(0, 0),
                        [Offset.ClassicLife] = new Vector2(0, 0),
                        [Offset.ClassicMana] = new Vector2(0, 0),
                        [Offset.Crafting] = new Vector2(0, 0),
                        [Offset.Accessories] = new Vector2(0, 0)
                    }
                };
                LayoutHelper.WriteLayoutFile(layoutName, layoutData);
            }
        }

        private static void CreateMinecraftLayout()
        {
            const string layoutName = "Minecraft";
            string minecraftPath = FileHelper.GetLayoutFilePath(layoutName);
            if (!File.Exists(minecraftPath))
            {
                var layoutData = new LayoutData
                {
                    ResourceTheme = ResourceTheme.Fancy2,
                    MapTheme = MapTheme.TwigLeaf,
                    Offsets = new Dictionary<Offset, Vector2>
                    {
                        [Offset.Chat] = new Vector2(1111, 21),
                        [Offset.Hotbar] = new Vector2(788, 936),
                        [Offset.Map] = new Vector2(5, -77),
                        [Offset.InfoAccs] = new Vector2(9, -85),
                        [Offset.FancyLife] = new Vector2(-736, 853),
                        [Offset.FancyMana] = new Vector2(3, -9),
                        [Offset.HorizontalBars] = new Vector2(0, 0),
                        [Offset.BarLifeText] = new Vector2(0, 0),
                        [Offset.BarManaText] = new Vector2(0, 0),
                        [Offset.Buffs] = new Vector2(0, 0),
                        [Offset.Inventory] = new Vector2(0, 0),
                        [Offset.ClassicLife] = new Vector2(0, 0),
                        [Offset.ClassicMana] = new Vector2(0, 0),
                        [Offset.Crafting] = new Vector2(0, 0),
                        [Offset.Accessories] = new Vector2(0, 0)
                    }
                };
                Log.Info($"Creating layout: {layoutName}");
                LayoutHelper.WriteLayoutFile(layoutName, layoutData);
                Log.Info($"Created layout: {layoutName}");
            }
        }

        private static void CreateLastLayoutFile()
        {
            string directory = FileHelper.GetLayoutsFolderPath();
            string lastLayoutPath = Path.Combine(directory, "LastLayout.txt");

            // If it doesn't exist, create it with a default value
            if (!File.Exists(lastLayoutPath))
            {
                File.WriteAllText(lastLayoutPath, "Default");
                LayoutHelper.CurrentLayoutName = "Default";
            }
        }
    }
}