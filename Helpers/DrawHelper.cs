using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using UICustomizer.Common.Systems;
using UICustomizer.Helpers.Layouts;
using static UICustomizer.Helpers.Layouts.ElementHelper;

namespace UICustomizer.Helpers
{
    public static class DrawHelper
    {
        public static void DrawHitboxOutlineAndText(SpriteBatch sb, Rectangle rect, Element element, int x = 0, int y = 0, Color color = default)
        {
            if (color == default)
                color = Color.Red;

            var sys = ModContent.GetInstance<EditorSystem>();
            if (sys == null || sys.state == null || sys.state.editorPanel == null) return;

            // Draw hitboxes
            if (EditorTabSettings.ShowHitboxes)
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

                sb.Draw(TextureAssets.MagicPixel.Value, fillRect, color * EditorTabSettings.Opacity);

                // Draw outline around the full-size rect
                DrawSlices(sb, rect, color, fill: false, fillOpacity: 0f);
            }

            // Draw eye toggle
            // Try to get the interface layer name from the mapping
            if (!ElementHelper.ElementInterfaceLayerMapping.TryGetValue(element, out string interfaceLayerName))
            {
                // Still draw the name if ShowNames is enabled, using the enum's string representation
                if (EditorTabSettings.ShowNames)
                {
                    Vector2 pos = rect.Location.ToVector2();
                    Utils.DrawBorderString(sb, element.ToString(), pos, Color.White);
                }
                return; // Exit early for this element if no mapping for eye toggle
            }

            // Draw eye toggle (only if mapping was found)
            if (EditorTabSettings.ShowEyeToggle)
            {
                if (LayerSystem.LayerStates == null)
                {
                    return;
                }

                bool isCurrentlyVisible = LayerSystem.LayerStates.TryGetValue(interfaceLayerName, out bool currentState) ? currentState : true;
                Rectangle eyeRect = new(rect.X - Ass.EyeOpen.Width(), rect.Y, Ass.EyeOpen.Width(), Ass.EyeOpen.Height());

                if (eyeRect.Contains(Main.mouseX, Main.mouseY))
                {
                    sb.Draw(isCurrentlyVisible ? Ass.EyeOpenHover.Value : Ass.EyeClosedHover.Value, eyeRect, Color.White); // Draw hover icon
                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        isCurrentlyVisible = !isCurrentlyVisible;
                        // Use interfaceLayerName (from mapping) as the key for LayerSystem.LayerStates
                        LayerSystem.LayerStates[interfaceLayerName] = isCurrentlyVisible;
                        Main.mouseLeftRelease = false;
                    }
                }
                else // Only draw the normal eye if not hovering
                {
                    sb.Draw(isCurrentlyVisible ? Ass.EyeOpen.Value : Ass.EyeClosed.Value, eyeRect, Color.White);
                }
            }

            // Draw names of the UI elements
            // Use interfaceLayerName if available and ShowNames is true, otherwise use element.ToString()
            if (EditorTabSettings.ShowNames)
            {
                Vector2 pos = rect.Location.ToVector2();
                // Display the mapped interfaceLayerName if available, otherwise the enum name.
                string displayName = !string.IsNullOrEmpty(interfaceLayerName) ? interfaceLayerName : element.ToString();
                if (EditorTabSettings.ShowEyeToggle) displayName = interfaceLayerName;
                else displayName = element.ToString();
                Utils.DrawBorderString(sb, displayName, pos, Color.White);
            }
        }

        /// <summary>
        /// A 30x30 pixel made in photoshop
        /// We grab the 5x5 corner edges to create a rounded edges look.
        /// </summary>
        private static void DrawSlices(SpriteBatch sb, Rectangle t, Color col, bool fill = true, float fillOpacity = 0.3f)
        {
            var tex = Ass.Hitbox.Value;
            int c = EditorTabSettings.Stroke;                         // 5-px corners / edge thickness
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
        public static int NewText(Rectangle location, Color color, string text, bool dramatic = false, bool dot = false)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return 100;
            }

            for (int i = 0; i < 100; i++)
            {
                if (Main.combatText[i].active)
                {
                    continue;
                }

                int num = 0;

                Vector2 vector = FontAssets.CombatText[num].Value.MeasureString(text);
                Main.combatText[i].alpha = 1f;
                Main.combatText[i].alphaDir = -1;
                Main.combatText[i].active = true;
                Main.combatText[i].scale = 0f;
                Main.combatText[i].rotation = 0f;
                Main.combatText[i].position.X = (float)location.X + (float)location.Width * 0.5f - vector.X * 0.5f;
                Main.combatText[i].position.Y = (float)location.Y + (float)location.Height * 0.25f - vector.Y * 0.5f;
                Main.combatText[i].position.X += Main.rand.Next(-(int)((double)location.Width * 0.5), (int)((double)location.Width * 0.5) + 1);
                Main.combatText[i].position.Y += Main.rand.Next(-(int)((double)location.Height * 0.5), (int)((double)location.Height * 0.5) + 1);
                Main.combatText[i].color = color;
                Main.combatText[i].text = text;
                Main.combatText[i].velocity.Y = -7f;
                if (Main.player[Main.myPlayer].gravDir == -1f)
                {
                    Main.combatText[i].velocity.Y *= -1f;
                    Main.combatText[i].position.Y = (float)location.Y + (float)location.Height * 0.75f + vector.Y * 0.5f;
                }

                Main.combatText[i].lifeTime = 60;
                Main.combatText[i].crit = dramatic;
                Main.combatText[i].dot = dot;
                if (dramatic)
                {
                    Main.combatText[i].text = text;
                    Main.combatText[i].lifeTime *= 2;
                    Main.combatText[i].velocity.Y *= 2f;
                    Main.combatText[i].velocity.X = (float)Main.rand.Next(-25, 26) * 0.05f;
                    Main.combatText[i].rotation = (float)(Main.combatText[i].lifeTime / 2) * 0.002f;
                    if (Main.combatText[i].velocity.X < 0f)
                    {
                        Main.combatText[i].rotation *= -1f;
                    }
                }

                if (dot)
                {
                    Main.combatText[i].velocity.Y = -4f;
                    Main.combatText[i].lifeTime = 40;
                }

                return i;
            }

            return 100;
        }
    }
}