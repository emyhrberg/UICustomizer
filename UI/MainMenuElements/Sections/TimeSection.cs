using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using UICustomizer.Common.Systems.Hooks.MainMenu;
using UICustomizer.Common.Systems.MainMenu;

namespace UICustomizer.UI.MainMenuElements.Sections
{
    internal class TimeSection : BaseSection
    {
        private enum TimeTab { Time, Speed, Parallax }
        private TimeTab tab = TimeTab.Time;

        private readonly UIText header;
        private readonly SmallColoredImageButton btnT, btnS, btnP;
        private readonly ZoeSlider timeSlider, speedSlider, parallaxSlider;
        private readonly ResetButton resetButton;
        private readonly PlayPause playPause;

        public TimeSection()
        {
            Height.Set(82, 0);

            // 1) Header
            Append(header = new UIText("Time: ")
            {
                HAlign = 1f,
                Left = { Pixels = -32 },
                Top = { Pixels = 8 }
            });

            // 2) Tabs
            btnT = MakeTab(Ass.T, TimeTab.Time, 0);
            btnS = MakeTab(Ass.S, TimeTab.Speed, 32);
            btnP = MakeTab(Ass.P, TimeTab.Parallax, 32 + 32);

            // ─── Common controls ──────────────────────────────────────────
            playPause = new PlayPause
            {
                Left = { Pixels = 6 },
                Top = { Pixels = 6 }
            };
            playPause.OnLeftMouseDown += (_, _) =>
                MainMenuPauseSystem.IsPaused = !MainMenuPauseSystem.IsPaused;
            Append(playPause);

            resetButton = new ResetButton
            {
                Left = { Pixels = 6 },
                Top = { Pixels = 6 }
            };
            resetButton.OnLeftMouseDown += (_, _) =>
            {
                if (tab == TimeTab.Speed)
                {
                    TimeSpeedHook.Speed = 1f;
                    speedSlider.Ratio = ColorHelper.InverseLerp(0f, 100f, 1f);
                }
                else if (tab == TimeTab.Parallax)
                {
                    ParallaxSpeedHook.Speed = 5f;
                    parallaxSlider.Ratio = ColorHelper.InverseLerp(0f, 100f, 5f);
                }
            };
            Append(resetButton);

            // ─── Time slider ──────────────────────────────────────────────
            timeSlider = new ZoeSlider
            {
                Top = { Pixels = 45 },
                Ratio = WorldTimeHelper.GetRatioFromTime(),
                InnerTexture = Ass.SliderTime
                //InnerTexture = Ass.SliderTime
            };
            timeSlider.OnDrag += WorldTimeHelper.SetTime;
            Append(timeSlider);

            // ─── IsDrawing slider ─────────────────────────────────────────────
            speedSlider = new ZoeSlider
            {
                Top = { Pixels = 45 },
                Ratio = ColorHelper.InverseLerp(0f, 100f, TimeSpeedHook.Speed)
            };
            speedSlider.OnDrag += v =>
                TimeSpeedHook.Speed = MathHelper.Lerp(0f, 100f, v);
            Append(speedSlider);

            // ─── Parallax slider ─────────────────────────────────────────
            parallaxSlider = new ZoeSlider
            {
                Top = { Pixels = 45 },
                Ratio = ColorHelper.InverseLerp(0f, 100f, ParallaxSpeedHook.Speed)
            };
            parallaxSlider.OnDrag += v =>
                ParallaxSpeedHook.Speed = MathHelper.Lerp(0f, 100f, v);
            Append(parallaxSlider);

            SelectTab(TimeTab.Time);
        }

        private SmallColoredImageButton MakeTab(Asset<Texture2D> tex, TimeTab tab, float left)
        {
            const float iconSize = 22f;

            var btn = new SmallColoredImageButton(tex, tab.ToString());
            btn.Left.Set(left, 0f);
            btn.Top.Set(4f, 0f);
            btn.Width.Set(iconSize, 0f);
            btn.Height.Set(iconSize, 0f);

            btn.OnLeftMouseDown += (_, _) => SelectTab(tab);
            Append(btn);
            return btn;
        }
        private void SelectTab(TimeTab tab)
        {
            this.tab = tab;

            // ─── Tab button highlights ───
            btnT.SetSelected(tab == TimeTab.Time);
            btnS.SetSelected(tab == TimeTab.Speed);
            btnP.SetSelected(tab == TimeTab.Parallax);

            // ─── Purge all dynamic children ───
            playPause.Remove();
            timeSlider.Remove();
            resetButton.Remove();
            speedSlider.Remove();
            parallaxSlider.Remove();

            // ─── Re-add what this tab needs ───
            switch (tab)
            {
                case TimeTab.Time:
                    header.SetText("Time: " + WorldTimeHelper.GetFormattedTime());
                    Append(playPause);
                    Append(timeSlider);
                    break;

                case TimeTab.Speed:
                    header.SetText("Speed: " + $"{TimeSpeedHook.Speed:F2}");
                    Append(resetButton);
                    Append(speedSlider);
                    break;

                case TimeTab.Parallax:
                    header.SetText("Parallax: " + $"{ParallaxSpeedHook.Speed:F2}");
                    Append(resetButton);
                    Append(parallaxSlider);
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (tab)
            {
                case TimeTab.Time:
                    header.SetText("Time: " + WorldTimeHelper.GetFormattedTime());
                    timeSlider.Ratio = WorldTimeHelper.GetRatioFromTime();
                    break;

                case TimeTab.Speed:
                    header.SetText($"Speed: {TimeSpeedHook.Speed:F2}");
                    speedSlider.Ratio = ColorHelper.InverseLerp(0f, 100f, TimeSpeedHook.Speed);
                    break;

                case TimeTab.Parallax:
                    header.SetText($"Parallax: {ParallaxSpeedHook.Speed:F2}");
                    parallaxSlider.Ratio = ColorHelper.InverseLerp(0f, 100f, ParallaxSpeedHook.Speed);
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
