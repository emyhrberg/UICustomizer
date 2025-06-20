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
            rRatio = Conf.C.MainMenuTextColor.R / 255f;
            gRatio = Conf.C.MainMenuTextColor.G / 255f;
            bRatio = Conf.C.MainMenuTextColor.B / 255f;
        }
        public override void Unload()
        {
            On_Main.DrawVersionNumber -= DrawMenuOptions;
        }

        // Variables
        public bool isOpen = false; // whether the collapse menu is open or not
        bool isManuallyPaused = false;

        // Store the state for each slider
        public float rRatio = 0.0f; // black
        public float gRatio = 0.0f; // black
        public float bRatio = 0.0f; // black
        public float rRatio2 = 255f / 255f;
        public float gRatio2 = 239f / 255f;
        public float bRatio2 = 69f / 255f;
        public float timeRatio = 0;

        // Positions
        int x = 260;
        int y = 40;

        public override void OnModLoad()
        {
            rRatio = Conf.C.MainMenuTextColor.R / 255f;
            gRatio = Conf.C.MainMenuTextColor.G / 255f;
            bRatio = Conf.C.MainMenuTextColor.B / 255f;
            Log.Info("r ratio: " + rRatio);
        }

        private void DrawMenuOptions(On_Main.orig_DrawVersionNumber orig, Color menuColor, float upbump)
        {
            orig(menuColor, upbump);

            //Log.SlowInfo(Main.time.ToString());
            //Log.Info("r ratio draw menu: " + rRatio);


            if (!Conf.C.EditMainMenu) return;
            if (Main.menuMode != 0) return; // Only draw on main menu

            if (isOpen)
            {
                SpriteBatch sb = Main.spriteBatch;
                DrawPanel(sb);

                // Text color sliders
                Utils.DrawBorderString(Main.spriteBatch, "Change text color", new Vector2(Main.screenWidth - x + 50, y + 10), Color.White);
                rRatio = SimpleSlider.Draw(sb, Main.screenWidth - x, y + 10 + 30, "R", rRatio);
                gRatio = SimpleSlider.Draw(sb, Main.screenWidth - x, y + 10 + 30 * 2, "G", gRatio);
                bRatio = SimpleSlider.Draw(sb, Main.screenWidth - x, y + 10 + 30 * 3, "B", bRatio);

                Vector3 updatedColor = new(rRatio, gRatio, bRatio);
                MainMenuTextColorHook.MainMenuTextColor = new Color(updatedColor);

                // Conf.C.MainMenuTextColor = MainMenuTextColorHook.MainMenuTextColor;
                // Conf.Save();

                Rectangle prev1 = new(Main.screenWidth - x + 50 + 150, y + 10, 20, 20);
                Color col1 = new(rRatio, gRatio, bRatio);
                sb.Draw(TextureAssets.MagicPixel.Value, prev1, col1);

                // Hover color sliders
                Utils.DrawBorderString(Main.spriteBatch, "Hover text color", new Vector2(Main.screenWidth - x + 50, y + 10 + 30 * 5), Color.White);
                rRatio2 = SimpleSlider.Draw(sb, Main.screenWidth - x, y + 10 + 30 * 6, "R", rRatio2);
                gRatio2 = SimpleSlider.Draw(sb, Main.screenWidth - x, y + 10 + 30 * 7, "G", gRatio2);
                bRatio2 = SimpleSlider.Draw(sb, Main.screenWidth - x, y + 10 + 30 * 8, "B", bRatio2);

                Rectangle prev2 = new(Main.screenWidth - x + 50 + 150, y + 10 + 30 * 5, 20, 20);
                Color col2 = new(rRatio2, gRatio2, bRatio2);
                sb.Draw(TextureAssets.MagicPixel.Value, prev2, col2);

                MainMenuHoverTextColorHook.R = rRatio2;
                MainMenuHoverTextColorHook.G = gRatio2;
                MainMenuHoverTextColorHook.B = bRatio2;

                Utils.DrawBorderString(Main.spriteBatch, $"Time: {GetFormattedTime()}", new Vector2(Main.screenWidth - x + 50, y + 10 + 30 * 10), Color.White);

                // Time slider
                float previousTimeRatio = timeRatio;
                timeRatio = SimpleSlider.DrawTimeSlider(sb, Main.screenWidth - x, y + 10 + 30 * 12, timeRatio);

                if (timeRatio != previousTimeRatio)
                {
                    SetTime(timeRatio);
                }

                timeRatio = GetRatioFromTime();
                SetTime(timeRatio);

                // --- Pause button ---
                string pauseText = $"Pause: {(isManuallyPaused ? "On" : "Off")}";
                Vector2 pauseSize = FontAssets.MouseText.Value.MeasureString(pauseText);
                var pauseRect = new Rectangle(Main.screenWidth - x + 50, y + 10 + 30 * 11, (int)pauseSize.X, (int)pauseSize.Y);
                bool isHoveringPause = Main.MouseScreen.Between(pauseRect.TopLeft(), pauseRect.BottomRight());

                Utils.DrawBorderString(sb, pauseText, pauseRect.TopLeft(), isHoveringPause ? Color.Yellow : Color.White);

                if (isHoveringPause && Main.mouseLeft && Main.mouseLeftRelease)
                {
                    isManuallyPaused = !isManuallyPaused;
                    MainMenuPauseSystem.TimeIsPausedBySlider = !MainMenuPauseSystem.TimeIsPausedBySlider;
                    Main.mouseLeftRelease = false;
                }
            }
            DrawCollapseButton();
        }
        private void SetTime(float ratio)
        {
            const float dayDuration = 54000f;
            const float totalCycleDuration = 86400f; // Day (54000) + Night (32400)
            float currentTickInCycle = ratio * totalCycleDuration;

            if (currentTickInCycle < dayDuration)
            {
                Main.dayTime = true;
                Main.time = currentTickInCycle;
            }
            else
            {
                Main.dayTime = false;
                Main.time = currentTickInCycle - dayDuration;
            }
        }

        private float GetRatioFromTime()
        {
            const float dayDuration = 54000f;
            const float totalCycleDuration = 86400f;

            double currentTimeInCycle = Main.time;
            if (!Main.dayTime)
            {
                currentTimeInCycle += dayDuration;
            }

            return (float)(currentTimeInCycle / totalCycleDuration);
        }

        private string GetFormattedTime() => $"{((int)((Main.time + (Main.dayTime ? 0 : 54000)) / 3600 + 4.5) % 12 == 0 ? 12 : (int)((Main.time + (Main.dayTime ? 0 : 54000)) / 3600 + 4.5) % 12)}:{(int)((Main.time + (Main.dayTime ? 0 : 54000)) % 3600 / 60):D2} {((Main.time + (Main.dayTime ? 0 : 54000)) / 3600 + 4.5 >= 12 ? "PM" : "AM")}";

        private void DrawPanel(SpriteBatch sb)
        {
            int w = 250;
            int h = Main.screenHeight - 100;
            Rectangle rect = new(Main.screenWidth - x, y, w, h);
            Utils.DrawInvBG(sb, rect, UICommon.DefaultUIBlue * 0.7f);
        }

        private void DrawCollapseButton()
        {
            // Draw collapse button
            SpriteBatch sb = Main.spriteBatch;
            Asset<Texture2D> tex = isOpen ? Ass.Minus : Ass.Plus;


            sb.Draw(tex.Value, position: new Vector2(Main.screenWidth - x, y: 0), null, Color.White, 0f, Vector2.Zero, scale: 1f, SpriteEffects.None, 0f);

            // Draw collapse button debug bounds
            //sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(Main.screenWidth - x, y-10, tex.Value.Width, tex.Value.Height), Color.Red * 0.3f);

            // Left click
            bool IsMouseHovering = Main.MouseScreen.Between(minimum: new Vector2(Main.screenWidth - x, y: 0), maximum: new Vector2(Main.screenWidth - x, y: 0) + tex.Value.Size());

            if (IsMouseHovering)
            {
                // Show hover glow and hover text
                Asset<Texture2D> hoverTex = isOpen ? Ass.MinusHover : Ass.PlusHover;
                sb.Draw(hoverTex.Value, position: new Vector2(Main.screenWidth - x, y: 0), null, Color.White, 0f, Vector2.Zero, scale: 1f, SpriteEffects.None, 0f);

                //string tooltip = !isOpen ? "Open text color menu" : "Close text color menu";
                string tooltip = !isOpen ? "Main menu" : "";
                Utils.DrawBorderString(sb, tooltip, new Vector2(Main.screenWidth - x + 30, 6), Color.White);

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
