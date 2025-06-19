using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using UICustomizer.Common.Configs;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    internal class MainMenuDraw : ModSystem
    {
        public override void Load()
        {
            On_Main.DrawVersionNumber += DrawMenuOptions;
        }
        public override void Unload()
        {
            On_Main.DrawVersionNumber -= DrawMenuOptions;
        }

        // Variables
        public bool isOpen = false; // whether the collapse menu is open or not

        // Store the state for each slider
        float rRatio = 0.5f; // gray
        float gRatio = 0.5f; // gray
        float bRatio = 0.5f; // gray
        float rRatio2 = 255f / 255f;
        float gRatio2 = 239f / 255f;
        float bRatio2 = 69f / 255f;

        // Positions
        int x = 530;
        int y = 10;

        private void DrawMenuOptions(On_Main.orig_DrawVersionNumber orig, Color menuColor, float upbump)
        {
            orig(menuColor, upbump);

            if (!Conf.C.EditMainMenu) return;
            if (Main.menuMode != 0) return; // Only draw on main menu

            if (isOpen)
            {
                SpriteBatch sb = Main.spriteBatch;
                DrawPanel(sb);

                // Text color sliders
                Utils.DrawBorderString(Main.spriteBatch, "Change text color", new Vector2(x+50, y+10), Color.White);
                rRatio = SimpleSlider.Draw(sb, x, y+10+30, "R", rRatio);
                gRatio = SimpleSlider.Draw(sb, x, y+10+30*2, "G", gRatio);
                bRatio = SimpleSlider.Draw(sb, x, y+10+30*3, "B", bRatio);

                Vector3 updatedColor = new(rRatio, gRatio, bRatio);
                MainMenuTextColorHook.MainMenuTextColor = new Color(updatedColor);

                Rectangle prev1 = new(x + 50+150, y + 10,20, 20);
                Color col1 = new(rRatio, gRatio, bRatio);
                sb.Draw(TextureAssets.MagicPixel.Value, prev1, col1);

                // Hover color sliders
                Utils.DrawBorderString(Main.spriteBatch, "Hover text color", new Vector2(x + 50, y + 10+30*5), Color.White);
                rRatio2 = SimpleSlider.Draw(sb, x, y + 10 + 30*6, "R", rRatio2);
                gRatio2 = SimpleSlider.Draw(sb, x, y + 10 + 30 * 7, "G", gRatio2);
                bRatio2 = SimpleSlider.Draw(sb, x, y + 10 + 30 * 8, "B", bRatio2);

                Rectangle prev2 = new(x + 50 + 150, y + 10+30*5, 20, 20);
                Color col2 = new(rRatio2, gRatio2, bRatio2);
                sb.Draw(TextureAssets.MagicPixel.Value, prev2, col2);

                MainMenuHoverTextColorHook.R = rRatio2;
                MainMenuHoverTextColorHook.G = gRatio2;
                MainMenuHoverTextColorHook.B = bRatio2;
            }
            DrawCollapseButton();
        }

        private void DrawPanel(SpriteBatch sb)
        {
            int w = 250;
            int h = 300;
            Rectangle rect = new(x, y, w, h);
            Utils.DrawInvBG(sb, rect, UICommon.DefaultUIBlue);
        }

        private void DrawCollapseButton()
        {
            // Draw collapse button
            SpriteBatch sb = Main.spriteBatch;
            Asset<Texture2D> tex = isOpen ? Ass.Minus : Ass.Plus;
            sb.Draw(tex.Value, position: new Vector2(x, y-10), null, Color.White, 0f, Vector2.Zero, scale: 1f, SpriteEffects.None, 0f);

            // Draw collapse button debug bounds
            //sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(x, y-10, tex.Value.Width, tex.Value.Height), Color.Red * 0.3f);

            // Left click
            bool IsMouseHovering = Main.MouseScreen.Between(minimum: new Vector2(x, y), maximum: new Vector2(x, y) + tex.Value.Size());

            if (IsMouseHovering)
            {
                // Show hover glow and hover text
                Asset<Texture2D> hoverTex = isOpen ? Ass.MinusHover : Ass.PlusHover;
                sb.Draw(hoverTex.Value, position: new Vector2(x, y-10), null, Color.White, 0f, Vector2.Zero, scale: 1f, SpriteEffects.None, 0f);

                //string tooltip = !isOpen ? "Open text color menu" : "Close text color menu";
                string tooltip = !isOpen ? "Change text color" : "";
                Utils.DrawBorderString(sb, tooltip, new Vector2(x+30, y-6), Color.White);

                // Toggle on left mouse click
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    isOpen = !isOpen;
                    Main.mouseLeftRelease = false;
                }
            }
        }
    }
}
