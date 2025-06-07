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

        public static void DrawHitboxOutlineAndText(SpriteBatch sb, Rectangle rect, string text, TextPosition textPos = TextPosition.Top, int x = 0, Color color = default)
        {
            if (color == default)
                color = Color.Red * 0.5f;
            else
                color *= 0.5f; // Reduce opacity

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

            // Draw names of the UI elements
            if (sys.state.panel.editorTab.CheckboxNames.state == CheckboxState.Checked)
            {
                Vector2 pos = rect.Location.ToVector2();

                // Use custom offsets if the user wants them
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
    }
}
