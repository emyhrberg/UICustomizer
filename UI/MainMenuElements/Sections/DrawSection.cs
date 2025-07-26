using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using UICustomizer.Common.Configs;
using UICustomizer.Common.Systems.Hooks.MainMenu;

namespace UICustomizer.UI.MainMenuElements.Sections
{
    public class DrawSection : BaseSection
    {
        private readonly UIElement col1;   // left column
        private readonly UIElement col2;   // right column
        private float yOffset = 15f;

        public DrawSection()
        {
            Height.Set(95, 0);

            Append(new UIText("Draw") { HAlign = 0.5f, Top = { Pixels = 0 } });

            // ─── create columns ───────────────────────────────────────
            col1 = new UIElement
            {
                Width = StyleDimension.FromPixelsAndPercent(-14, 0.5f),   // half panel minus gap
                Height = StyleDimension.FromPixelsAndPercent(0, 1f),
                HAlign = 0f
            };
            col2 = new UIElement
            {
                Width = StyleDimension.FromPixelsAndPercent(-14, 0.5f),
                Height = StyleDimension.FromPixelsAndPercent(0, 1f),
                HAlign = 1f
            };
            Append(col1);
            Append(col2);

            // ───── left column ─────
            MakeToggle("Background: ", col1,
                b => { Conf.C.MainMenuDraw.DrawBackground = b; BackgroundHook.IsDrawing = b; });

            //MakeToggle("Clouds: ", col1,
            //    b => { Conf.C.MainMenuDraw.DrawClouds = b; SkipCloudsHook.IsDrawing = b; });

            MakeToggle("Sun: ", col1,
                b => { Conf.C.MainMenuDraw.DrawSun = b; SkipSunDrawHook.IsDrawing = b; });

            MakeToggle("Sky: ", col1,
                b => { Conf.C.MainMenuDraw.DrawSky = b; SkipSkyDrawHook.IsDrawing = b; });


            // ───── right column ─────
            yOffset = 15; // reset
            MakeToggle("Logo: ", col2,
                b => { Conf.C.MainMenuDraw.DrawLogo = b; LogoHook.IsDrawing = b; });

            //MakeToggle("Text: ", col2,
            //    b =>
            //    {
            //        Conf.C.MainMenuDraw.DrawMainText = b;
            //        MainMenuTextColorHook.IsDrawing = !MainMenuTextColorHook.IsDrawing;
            //    });

            MakeToggle("Stars: ", col2,
                b => { Conf.C.MainMenuDraw.DrawStars = b; SkipStarsHook.IsDrawing = b; });

            MakeToggle("Version: ", col2,
                b =>
                {
                    Conf.C.MainMenuDraw.DrawVersion = b; SkipVersionNumberDrawHook.IsDrawing = b; SkipSocialMediaButtonsHook.IsDrawing = b;
                });

            void MakeToggle(string label, UIElement parent, Action<bool> apply)
            {
                var btn = new OnOffTextButton(label)
                {
                    Top = { Pixels = yOffset }
                };
                yOffset += 18f;
                btn.OnLeftClick += (_, _) => apply(btn.isOn);
                parent.Append(btn);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
