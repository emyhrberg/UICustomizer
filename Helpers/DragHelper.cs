using Terraria.GameContent;
using UICustomizer.Common.Systems.Hooks;

namespace UICustomizer.Helpers
{
    public static class DragHelper
    {
        public static bool MouseInBounds(Rectangle bounds)
        {
            Point point = Main.MouseScreen.ToPoint();

            return bounds.Contains(point);
        }

        public static Rectangle ChatBounds()
        {
            // vanilla: centre horizontally, a bit above the bottom toolbar
            int w = TextureAssets.TextBack.Width() + 120; // not accurate, its much wider in fullscreen
            int h = TextureAssets.TextBack.Height();
            int x = (int)(78 + ChatHook.OffsetX);
            int y = (int)(Main.screenHeight-86 + ChatHook.OffsetY);
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle HotbarBounds()
        {
            int slot = (int)(52f * Main.inventoryScale);    // vanilla slot size
            int w = slot * 20 - 180;
            int h = slot * 2 + 10;
            int x = (int)(20 + HotbarHook.OffsetX);
            int y = (int)(HotbarHook.OffsetY);
            return new Rectangle(x, y, w, h);
        }
    }
}