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
        private readonly UIElement column1Element;   // left column
        private readonly UIElement column2Element;   // right column

        public DrawSection()
        {
            Height.Set(130, 0);

            Append(new UIText("Draw") { HAlign = 0.5f, Top = { Pixels = 4 } });

            // ─── create columns ───────────────────────────────────────
            column1Element = new UIElement
            {
                Width = StyleDimension.FromPixelsAndPercent(-14, 0.5f),   // half panel minus gap
                Height = StyleDimension.FromPixelsAndPercent(0, 1f),
                HAlign = 0f
            };
            column2Element = new UIElement
            {
                Width = StyleDimension.FromPixelsAndPercent(-14, 0.5f),
                Height = StyleDimension.FromPixelsAndPercent(0, 1f),
                HAlign = 1f
            };
            Append(column1Element);
            Append(column2Element);

            const float firstRowY = 30f;
            const float rowGap = 22f;

            // ───── left column ─────
            MakeToggle("Background: ", column1Element, 0,
                b => { Conf.C.MainMenuDraw.DrawBackground = b; SkipBackgroundDrawHook.IsDrawing = b; });

            MakeToggle("Clouds: ", column1Element, 1,
                b => { Conf.C.MainMenuDraw.DrawClouds = b; SkipCloudsHook.IsDrawing = b; });

            MakeToggle("Sun: ", column1Element, 2,
                b => { Conf.C.MainMenuDraw.DrawSun = b; SkipSunDrawHook.IsDrawing = b; });

            MakeToggle("Sky: ", column1Element, 3,
                b => { Conf.C.MainMenuDraw.DrawSky = b; SkipSkyDrawHook.IsDrawing = b; });

            // ───── right column ─────
            MakeToggle("Logo: ", column2Element, 0,
                b => { Conf.C.MainMenuDraw.DrawLogo = b; LogoHook.IsDrawing = b; });

            MakeToggle("Text: ", column2Element, 1,
                b =>
                {
                    Conf.C.MainMenuDraw.DrawMainText = b;
                    MainMenuTextColorHook.FillColor = new(0, 0, 0, 0);
                });

            MakeToggle("Social: ", column2Element, 2,
                b => { Conf.C.MainMenuDraw.DrawSocial = b; SkipSocialMediaButtonsHook.IsDrawing = b; });

            MakeToggle("Version: ", column2Element, 3,
                b => { Conf.C.MainMenuDraw.DrawVersion = b; SkipVersionNumberDrawHook.IsDrawing = b; });

            // ───────────────── helper ─────────────────
            void MakeToggle(string label, UIElement parent, int row, Action<bool> apply)
            {
                var btn = new OnOffTextButton(label)
                {
                    Top = { Pixels = firstRowY + rowGap * row }   // vertical placement
                };
                btn.OnLeftClick += (_, _) => apply(btn.isOn);
                parent.IgnoresMouseInteraction = false;
                parent.Append(btn);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
