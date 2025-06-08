using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using UICustomizer.Common.Systems;

namespace UICustomizer.Helpers
{
    public static class DrawHelper
    {
        public static void DrawHitboxOutlineAndText(SpriteBatch sb, Rectangle rect, string elementName, int x = 0, int y = 0, Color color = default)
        {
            if (color == default)
                color = Color.Red;

            var sys = ModContent.GetInstance<EditorSystem>();
            if (sys == null || sys.state == null || sys.state.editorPanel == null) return;

            // Draw hitboxes
            if (UIEditorSettings.ShowHitboxes)
            {
                const float fillScaleX = 0.985f;
                const float fillScaleY = 0.97f;

                int padX = (int)((1f - fillScaleX) * rect.Width / 2f); // will be 0
                int padY = (int)((1f - fillScaleY) * rect.Height / 2f);

                var fillRect = new Rectangle(
                    rect.X + padX,
                    rect.Y + padY,
                    (int)(rect.Width * fillScaleX),
                    (int)(rect.Height * fillScaleY)
                );

                sb.Draw(TextureAssets.MagicPixel.Value, fillRect, color * UIEditorSettings.Opacity);

                // Draw outline around the full-size rect
                DrawSlices(sb, rect, color, fill: false, fillOpacity: 0f);
            }

            // Draw names of the UI elements
            if (UIEditorSettings.ShowNames)
            {
                Vector2 pos = rect.Location.ToVector2();

                // Use custom offsets if the user wants them
                //if (UIEditorSettings.OffsetText)
                //{
                    //pos += new Vector2(x, y);
                //}

                Utils.DrawBorderString(sb, elementName, pos, Color.White);
            }
        }

        /// <summary>
        /// A 30x30 pixel made in photoshop
        /// We grab the 5x5 corner edges to create a rounded edges look.
        /// </summary>
        private static void DrawSlices(SpriteBatch sb, Rectangle t, Color col, bool fill = true, float fillOpacity = 0.3f)
        {
            var tex = Ass.Hitbox.Value;
            int c = UIEditorSettings.Stroke;                         // 5-px corners / edge thickness
            Rectangle sc = new(0, 0, c, c),
                      eh = new(c, 0, 30 - 2 * c, c),
                      ev = new(0, c, c, 30 - 2 * c),
                      ce = new(c, c, 30 - 2 * c, 30 - 2 * c);

            if (fill)
                sb.Draw(tex, new Rectangle(t.X + c, t.Y + c, t.Width - 2 * c, t.Height - 2 * c), ce, col * fillOpacity);

            sb.Draw(tex, new Rectangle(t.X + c, t.Y, t.Width - 2 * c, c), eh, col);                                       // top
            sb.Draw(tex, new Rectangle(t.X + c, t.Bottom - c, t.Width - 2 * c, c), eh, col, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0); // bottom
            sb.Draw(tex, new Rectangle(t.X, t.Y + c, c, t.Height - 2 * c), ev, col);                                       // left
            sb.Draw(tex, new Rectangle(t.Right - c, t.Y + c, c, t.Height - 2 * c), ev, col, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0); // right

            sb.Draw(tex, new Rectangle(t.X, t.Y, c, c), sc, col);                                                          // TL
            sb.Draw(tex, new Rectangle(t.Right - c, t.Y, c, c), sc, col, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0); // TR
            sb.Draw(tex, new Rectangle(t.Right - c, t.Bottom - c, c, c), sc, col, 0, Vector2.Zero, SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally, 0); // BR
            sb.Draw(tex, new Rectangle(t.X, t.Bottom - c, c, c), sc, col, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0); // BL
        }

        public static void DrawInfoText(SpriteBatch sb, string text, Vector2 position, Color color)
        {
            if (string.IsNullOrEmpty(text)) return;

            // Draw the info text at the specified position
            Utils.DrawBorderString(sb, text, position, color);
        }
    }
}