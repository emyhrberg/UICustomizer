using Terraria.GameContent;

namespace UICustomizer.Helpers
{
    internal static class DrawHelper
    {
        internal static void DrawDebugHitbox(UIElement element, Color color = default, float scale=1f)
        {
            if (element == null)
            {
                Log.SlowInfo($"Oop. Failed to find element: {element} to draw. Skipping draw.", seconds: 5);
                return;
            }

            CalculatedStyle dims = element.GetDimensions();

            // centre of the element
            float cx = dims.X + dims.Width * 0.5f;
            float cy = dims.Y + dims.Height * 0.5f;

            // scaled size
            float w = dims.Width * scale;
            float h = dims.Height * scale;

            // top-left corner after scaling about the centre
            int left = (int)(cx - w * 0.5f);
            int top = (int)(cy - h * 0.5f);

            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left, top, (int)w, (int)h), color);
        }
    }
}
