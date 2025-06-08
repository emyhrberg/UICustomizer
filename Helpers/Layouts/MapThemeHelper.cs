using System.Linq;
using Terraria;

namespace UICustomizer.Helpers.Layouts
{
    public static class MapThemeHelper
    {
        public enum MapTheme
        {
            Default,
            Golden,
            Remix,
            Sticks,
            StoneGold,
            TwigLeaf,
            Leaf,
            Retro,
            Valkyrie,
        }

        public static void SetMapTheme(MapTheme theme)
        {
            var options = Main.MinimapFrameManagerInstance.Options.Keys.ToList();
            if ((int)theme >= 0 && (int)theme < options.Count)
            {
                Main.MinimapFrameManagerInstance.SetActiveFrame(options[(int)theme]);
            }
        }

        public static void GetActiveMapTheme(out MapTheme theme)
        {
            var options = Main.MinimapFrameManagerInstance.Options.Keys.ToList();
            string currentKey = Main.MinimapFrameManagerInstance.ActiveSelectionKeyName;
            int activeIndex = options.IndexOf(currentKey);

            if (activeIndex >= 0 && activeIndex < options.Count)
            {
                theme = (MapTheme)activeIndex;
            }
            else
            {
                theme = MapTheme.Default; // Fallback to Default if the active index is invalid
            }
        }
    }
}
