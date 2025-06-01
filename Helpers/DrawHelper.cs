using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using UICustomizer.Common.Systems;
using UICustomizer.UI;

namespace UICustomizer.Helpers
{
    public static class DrawHelper
    {
        public enum TextPosition
        {
            Top,
            Left,
            Right,
            Bottom
        }

        public static void DrawHitboxOutlineAndText(SpriteBatch sb, Rectangle rect, string text, TextPosition textPos = TextPosition.Top, int x =0)
        {
            //if (color == default)
            Color color = Color.Red * 0.5f;

            Texture2D t = TextureAssets.MagicPixel.Value;
            int thickness = 2;
            var sys = ModContent.GetInstance<UICustomizerSystem>();
            if (sys == null || sys.state == null || sys.state.panel == null)
            {
                Log.SlowInfo("UICustomizerSystem or editorPanel is not initialized. Skipping draw.", seconds: 5);
                return;
            }

            // Draw outline (outline around the rectangle)
            if (sys.state.panel.editorTab.CheckboxOutline.state == CheckboxState.Checked)
            {
                sb.Draw(t, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
                sb.Draw(t, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
                sb.Draw(t, new Rectangle(rect.X + rect.Width - thickness, rect.Y, thickness, rect.Height), color);
                sb.Draw(t, new Rectangle(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness), color);
            }

            // Draw fill with 0.3 opacity
            if (sys.state.panel.editorTab.CheckboxFill.state == CheckboxState.Checked)
            {
                sb.Draw(t, rect, color * 0.3f);
            }

            // Draw names (of the UI elements)
            if (sys.state.panel.editorTab.CheckboxNames.state == CheckboxState.Checked)
            {
                Vector2 pos = rect.Location.ToVector2();

                // Custom offsets?
                if (sys.state.panel.editorTab.CheckboxTextPos?.state == CheckboxState.Checked)
                {
                    pos += new Vector2(x, 0);
                    if (textPos == TextPosition.Left)
                    {
                        pos += new Vector2(-60, 0);
                    }
                    else if (textPos == TextPosition.Right)
                    {
                        pos += new Vector2(rect.Width, 0);
                    }
                    else if (textPos == TextPosition.Top)
                    {
                        pos += new Vector2(0, -20); // place 20 pixels above the pos
                    }
                    else if (textPos == TextPosition.Bottom)
                    {
                        pos += new Vector2(-8, rect.Height); // place 20 pixels below the pos
                    }
                }
                    
                Utils.DrawBorderString(sb, text, pos, Color.White);
            }
        }

        /// <summary>
        /// Draws a texture at the proper scale to fit within the given UI element.
        /// </summary>
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
