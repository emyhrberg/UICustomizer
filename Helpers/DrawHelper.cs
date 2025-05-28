using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace UICustomizer.Helpers
{
    public static class DrawHelper
    {
        public static void DrawDebugHitbox(UIElement element, Color color = default, float scale = 1f)
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

        public static void DrawDebugHitbox(Rectangle rect, Color color = default)
        {
            if (color == default)
                color = Color.Red * 0.5f;

            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, color);
        }

        public static void DrawDebugText(SpriteBatch sb, Vector2 pos, string text, int yOffset = 20)
        {
            Color color = Color.White;
            pos += new Vector2(0, -20); // place 20 pixels above the pos
            Utils.DrawBorderString(sb, text, pos, color);
        }

        public static void DrawDebugHitboxOutline(SpriteBatch sb, Rectangle rect, Color color = default)
        {
            if (color == default)
                color = Color.Red * 0.5f;

            Texture2D t = TextureAssets.MagicPixel.Value;
            int thickness = 2;

            sb.Draw(t, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
            sb.Draw(t, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
            sb.Draw(t, new Rectangle(rect.X + rect.Width - thickness, rect.Y, thickness, rect.Height), color);
            sb.Draw(t, new Rectangle(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness), color);
        }
    }
}
