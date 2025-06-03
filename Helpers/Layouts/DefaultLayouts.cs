using System.Collections.Generic;
using System.IO;
using static UICustomizer.Helpers.ResourceThemeHelper;

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
            CreateLastLayoutFile();
        }

        private static void CreateDefaultLayout()
        {
            string defaultPath = FileHelper.GetLayoutFilePath("Default");
            if (!File.Exists(defaultPath))
            {
                var layoutData = new LayoutData
                {
                    Theme = ResourceTheme.Classic,
                    Positions = new Dictionary<string, Vector2>
                    {
                        ["ChatOffset"] = Vector2.Zero,
                        ["HotbarOffset"] = Vector2.Zero,
                        ["MapOffset"] = Vector2.Zero,
                        ["InfoAccsOffset"] = Vector2.Zero,
                        ["ClassicLifeOffset"] = Vector2.Zero,
                        ["ClassicManaOffset"] = Vector2.Zero,
                        ["FancyLifeOffset"] = Vector2.Zero,
                        ["FancyManaOffset"] = Vector2.Zero,
                        ["HorizontalBarsOffset"] = Vector2.Zero,
                        ["BarLifeTextOffset"] = Vector2.Zero,
                        ["BarManaTextOffset"] = Vector2.Zero,
                        ["BuffOffset"] = Vector2.Zero
                    }
                };
                LayoutHelper.WriteLayoutFile("Default", layoutData);
            }
        }

        private static void CreateHotbarCenteredLayout()
        {
            string hbPath = FileHelper.GetLayoutFilePath("HotbarCentered");
            if (!File.Exists(hbPath))
            {
                var layoutData = new LayoutData
                {
                    Theme = ResourceTheme.Classic,
                    Positions = new Dictionary<string, Vector2>
                    {
                        ["ChatOffset"] = Vector2.Zero,
                        ["HotbarOffset"] = new Vector2(120, 0),
                        ["MapOffset"] = Vector2.Zero,
                        ["InfoAccsOffset"] = Vector2.Zero,
                        ["ClassicLifeOffset"] = Vector2.Zero,
                        ["ClassicManaOffset"] = Vector2.Zero,
                        ["FancyLifeOffset"] = Vector2.Zero,
                        ["FancyManaOffset"] = Vector2.Zero,
                        ["HorizontalBarsOffset"] = Vector2.Zero,
                        ["BarLifeTextOffset"] = Vector2.Zero,
                        ["BarManaTextOffset"] = Vector2.Zero,
                        ["BuffOffset"] = Vector2.Zero
                    }
                };
                LayoutHelper.WriteLayoutFile("HotbarCentered", layoutData);
            }
        }

        private static void CreateMapLeftLayout()
        {
            string mapLeftPath = FileHelper.GetLayoutFilePath("MapLeft");
            if (!File.Exists(mapLeftPath))
            {
                var layoutData = new LayoutData
                {
                    Theme = ResourceTheme.Fancy2,
                    Positions = new Dictionary<string, Vector2>
                    {
                        ["ChatOffset"] = Vector2.Zero,
                        ["HotbarOffset"] = new Vector2(230, -4),
                        ["MapOffset"] = new Vector2(-658, -71),
                        ["InfoAccsOffset"] = new Vector2(475, 286),
                        ["ClassicLifeOffset"] = Vector2.Zero,
                        ["ClassicManaOffset"] = Vector2.Zero,
                        ["FancyLifeOffset"] = new Vector2(34, 273),
                        ["FancyManaOffset"] = new Vector2(-258, 7),
                        ["HorizontalBarsOffset"] = new Vector2(38, -3),
                        ["BarLifeTextOffset"] = Vector2.Zero,
                        ["BarManaTextOffset"] = Vector2.Zero,
                        ["BuffOffset"] = new Vector2(0, 0)
                    }
                };
                LayoutHelper.WriteLayoutFile("MapLeft", layoutData);
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