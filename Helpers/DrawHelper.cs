using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace UICustomizer.Helpers
{
    public static class DrawHelper
    {
        public static void DrawHitboxOutlineAndText(SpriteBatch sb, Rectangle rect, string text, Color color = default)
        {
            if (color == default)
                color = Color.Red * 0.5f;

            Texture2D t = TextureAssets.MagicPixel.Value;
            int thickness = 2;

            sb.Draw(t, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
            sb.Draw(t, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
            sb.Draw(t, new Rectangle(rect.X + rect.Width - thickness, rect.Y, thickness, rect.Height), color);
            sb.Draw(t, new Rectangle(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness), color);

            Vector2 pos = rect.Location.ToVector2();
            pos += new Vector2(0, -20); // place 20 pixels above the pos
            Utils.DrawBorderString(sb, text, pos, Color.White);
        }

        /// <summary>
        /// Draws a texture at the proper scale to fit within the given UI element.
        /// /// </summary>
        public static void DrawProperScale(SpriteBatch spriteBatch, UIElement element, Texture2D tex, float scale = 1.0f, float opacity = 1.0f, bool active = false)
        {
            if (tex == null || element == null)
            {
                Log.SlowInfo("Failed to find texture to draw. Skipping draw.", seconds: 5);
            }

            // Get the UI element's dimensions
            CalculatedStyle dims = element.GetDimensions();

            // Compute a scale that makes it fit within the UI element
            float scaleX = dims.Width / tex.Width;
            float scaleY = dims.Height / tex.Height;
            float drawScale = Math.Min(scaleX, scaleY) * scale;

            // Top-left anchor: just place it at dims.X, dims.Y
            Vector2 drawPosition = new Vector2(dims.X, dims.Y);

            float actualOpacity = opacity;
            if (active)
            {
                actualOpacity = 1f;
            }

            // Draw the texture anchored at top-left with the chosen scale
            spriteBatch.Draw(
                tex,
                drawPosition,
                null,
                Color.White * actualOpacity,
                0f,
                Vector2.Zero,
                drawScale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
