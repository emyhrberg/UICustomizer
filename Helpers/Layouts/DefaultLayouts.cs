using System.Collections.Generic;
using System.IO;
using static UICustomizer.Helpers.Layouts.ElementHelper;
using static UICustomizer.Helpers.Layouts.MapThemeHelper;
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
            CreateMirrorLayout();
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
                    Offsets = new Dictionary<Element, Vector2>
                    {
                        [Element.Chat] = Vector2.Zero,
                        [Element.Hotbar] = Vector2.Zero,
                        [Element.Map] = Vector2.Zero,
                        [Element.InfoAccs] = Vector2.Zero,
                        [Element.ClassicLife] = Vector2.Zero,
                        [Element.ClassicMana] = Vector2.Zero,
                        [Element.FancyLife] = Vector2.Zero,
                        [Element.FancyMana] = Vector2.Zero,
                        [Element.HorizontalBars] = Vector2.Zero,
                        [Element.BarLifeText] = Vector2.Zero,
                        [Element.BarManaText] = Vector2.Zero,
                        [Element.Buffs] = Vector2.Zero,
                        [Element.Inventory] = Vector2.Zero,
                        [Element.Crafting] = Vector2.Zero,
                        [Element.Accessories] = Vector2.Zero
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
                    Offsets = new Dictionary<Element, Vector2>
                    {
                        [Element.Chat] = Vector2.Zero,
                        [Element.Hotbar] = new Vector2(650, 0),
                        [Element.Map] = Vector2.Zero,
                        [Element.InfoAccs] = Vector2.Zero,
                        [Element.ClassicLife] = Vector2.Zero,
                        [Element.ClassicMana] = Vector2.Zero,
                        [Element.FancyLife] = Vector2.Zero,
                        [Element.FancyMana] = Vector2.Zero,
                        [Element.HorizontalBars] = Vector2.Zero,
                        [Element.BarLifeText] = Vector2.Zero,
                        [Element.BarManaText] = Vector2.Zero,
                        [Element.Buffs] = Vector2.Zero,
                        [Element.Inventory] = Vector2.Zero,
                        [Element.Crafting] = Vector2.Zero,
                        [Element.Accessories] = Vector2.Zero
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
                    Offsets = new Dictionary<Element, Vector2>
                    {
                        [Element.Hotbar] = new Vector2(230, -4),
                        [Element.Map] = new Vector2(-658, -71),
                        [Element.InfoAccs] = new Vector2(475, 286),
                        [Element.FancyLife] = new Vector2(34, 273),
                        [Element.FancyMana] = new Vector2(-258, 7),
                        [Element.HorizontalBars] = new Vector2(38, -3),
                        [Element.BarLifeText] = new Vector2(0, 0),
                        [Element.BarManaText] = new Vector2(0, 0),
                        [Element.Buffs] = new Vector2(0, 0),
                        [Element.Inventory] = new Vector2(0, 0),
                        [Element.Chat] = new Vector2(0, 0),
                        [Element.ClassicLife] = new Vector2(0, 0),
                        [Element.ClassicMana] = new Vector2(0, 0),
                        [Element.Crafting] = new Vector2(0, 0),
                        [Element.Accessories] = new Vector2(0, 0)
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
                    Offsets = new Dictionary<Element, Vector2>
                    {
                        [Element.Chat] = new Vector2(1111, 21),
                        [Element.Hotbar] = new Vector2(788, 936),
                        [Element.Map] = new Vector2(5, -77),
                        [Element.InfoAccs] = new Vector2(9, -85),
                        [Element.FancyLife] = new Vector2(-736, 853),
                        [Element.FancyMana] = new Vector2(3, -9),
                        [Element.HorizontalBars] = new Vector2(0, 0),
                        [Element.BarLifeText] = new Vector2(0, 0),
                        [Element.BarManaText] = new Vector2(0, 0),
                        [Element.Buffs] = new Vector2(0, 0),
                        [Element.Inventory] = new Vector2(0, 0),
                        [Element.ClassicLife] = new Vector2(0, 0),
                        [Element.ClassicMana] = new Vector2(0, 0),
                        [Element.Crafting] = new Vector2(0, 0),
                        [Element.Accessories] = new Vector2(0, 0)
                    }
                };
                Log.Info($"Creating layout: {layoutName}");
                LayoutHelper.WriteLayoutFile(layoutName, layoutData);
                Log.Info($"Created layout: {layoutName}");
            }
        }

        private static void CreateMirrorLayout()
        {
            const string layoutName = "Mirror";
            string minecraftPath = FileHelper.GetLayoutFilePath(layoutName);
            if (!File.Exists(minecraftPath))
            {
                var layoutData = new LayoutData
                {
                    ResourceTheme = ResourceTheme.Fancy2,
                    MapTheme = MapTheme.TwigLeaf,
                    Offsets = new Dictionary<Element, Vector2>
                    {
                        [Element.Chat] = new Vector2(196, -29),
                        [Element.Hotbar] = new Vector2(1457, 6),
                        [Element.Map] = new Vector2(-1578, 0),
                        [Element.InfoAccs] = new Vector2(-1578, -85),
                        [Element.FancyLife] = new Vector2(-1573, -1),
                        [Element.FancyMana] = new Vector2(-1872, -6),
                        [Element.HorizontalBars] = new Vector2(0, 0),
                        [Element.BarLifeText] = new Vector2(0, 0),
                        [Element.BarManaText] = new Vector2(0, 0),
                        [Element.Buffs] = new Vector2(1450, 9),
                        [Element.Inventory] = new Vector2(1348, 54),
                        [Element.ClassicLife] = new Vector2(0, 0),
                        [Element.ClassicMana] = new Vector2(0, 0),
                        [Element.Crafting] = new Vector2(0, 0),
                        [Element.Accessories] = new Vector2(0, 0)
                    }
                };
                Log.Info($"Creating layout: {layoutName}");
                LayoutHelper.WriteLayoutFile(layoutName, layoutData);
                Log.Info($"Created layout: {layoutName}");
            }
        }

        private static void CreateBottomLayout()
        {
            const string layoutName = "Bottom";
            string bottomPath = FileHelper.GetLayoutFilePath(layoutName);
            if (!File.Exists(bottomPath))
            {
                var layoutData = new LayoutData
                {
                    ResourceTheme = ResourceTheme.Fancy,
                    MapTheme = MapTheme.Default,
                    Offsets = new Dictionary<Element, Vector2>
                    {
                        [Element.Chat] = new Vector2(-10, 20),
                        [Element.Hotbar] = new Vector2(736, 993),
                        [Element.Map] = new Vector2(-25, 664),
                        [Element.InfoAccs] = new Vector2(96, 221),
                        [Element.ClassicLife] = new Vector2(0, 0),
                        [Element.ClassicMana] = new Vector2(0, 0),
                        [Element.FancyLife] = new Vector2(-30, 989),
                        [Element.FancyMana] = new Vector2(-6, 811),
                        [Element.HorizontalBars] = new Vector2(0, 0),
                        [Element.BarLifeText] = new Vector2(0, 0),
                        [Element.BarManaText] = new Vector2(0, 0),
                        [Element.Buffs] = new Vector2(0, 0),
                        [Element.Inventory] = new Vector2(106, 673),
                        [Element.Crafting] = new Vector2(0, 0),
                        [Element.Accessories] = new Vector2(0, 0)
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