using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using UICustomizer.Core.MainMenuEditor.Hooks;

namespace UICustomizer.Core.MainMenuEditor.UI.Sections
{
    public class DrawSection : BaseSection
    {
        private ResetButton reset;

        private readonly UIElement col1;   // left column
        private readonly UIElement col2;   // right column
        private float yOffset = 18f;

        public DrawSection()
        {
            Height.Set(105, 0);

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
            BuildEverything();

            reset = new ResetButton { Left = { Pixels = 6 }, Top = { Pixels = 0 } };
            reset.OnLeftMouseDown += (_, _) =>
            {
                Conf.C.MainMenuDraw.DrawText = true;
                Conf.C.MainMenuDraw.DrawBackground = true;
                Conf.C.MainMenuDraw.DrawSun = true;
                Conf.C.MainMenuDraw.DrawSky = true;
                Conf.C.MainMenuDraw.DrawLogo = true;
                Conf.C.MainMenuDraw.DrawStars = true;
                Conf.C.MainMenuDraw.DrawVersion = true;
                BackgroundHook.IsDrawing = true;
                SkipSunDrawHook.IsDrawing = true;
                SkipSkyDrawHook.IsDrawing = true;
                LogoHook.IsDrawing = true;
                MainMenuTextColorHook.IsDrawing = true;
                SkipStarsHook.IsDrawing = true;
                SkipVersionNumberDrawHook.IsDrawing = true;
                Conf.Save();
                col1.RemoveAllChildren();
                col2.RemoveAllChildren();
                yOffset = 18f;
                BuildEverything();
            };
            Append(reset);
        }

        private void BuildEverything()
        {
            // Column 1
            MakeToggle("Background: ", col1,
                () => Conf.C.MainMenuDraw.DrawBackground,
                v => Conf.C.MainMenuDraw.DrawBackground = v,
                v => BackgroundHook.IsDrawing = v);

            MakeToggle("Clouds: ", col1,
                () => Conf.C.MainMenuDraw.DrawClouds,
                v => Conf.C.MainMenuDraw.DrawClouds = v,
                v => SkipCloudsHook.IsDrawing = v);

            MakeToggle("Logo: ", col1,
                () => Conf.C.MainMenuDraw.DrawLogo,
                v => Conf.C.MainMenuDraw.DrawLogo = v,
                v => LogoHook.IsDrawing = v);

            MakeToggle("Sky: ", col1,
                () => Conf.C.MainMenuDraw.DrawSky,
                v => Conf.C.MainMenuDraw.DrawSky = v,
                v => SkipSkyDrawHook.IsDrawing = v);

            yOffset = 18f;
            // Column 2
            MakeToggle("Sun: ", col2,
                () => Conf.C.MainMenuDraw.DrawSun,
                v => Conf.C.MainMenuDraw.DrawSun = v,
                v => SkipSunDrawHook.IsDrawing = v);
            MakeToggle("Stars: ", col2,
                () => Conf.C.MainMenuDraw.DrawStars,
                v => Conf.C.MainMenuDraw.DrawStars = v,
                v => SkipStarsHook.IsDrawing = v);
            MakeToggle("Text: ", col2,
                () => Conf.C.MainMenuDraw.DrawText,
                v => Conf.C.MainMenuDraw.DrawText = v,
                v => MainMenuTextColorHook.IsDrawing = v);
            MakeToggle("Version: ", col2,
                () => Conf.C.MainMenuDraw.DrawVersion,
                v => Conf.C.MainMenuDraw.DrawVersion = v,
                v => SkipVersionNumberDrawHook.IsDrawing = v);
        }

        private void MakeToggle(string label, UIElement parent, Func<bool> getConfig, Action<bool> setConfig, Action<bool> setHook)
        {
            var btn = new OnOffTextButton(label, getConfig()) { Top = { Pixels = yOffset } };
            yOffset += 18f;

            btn.OnLeftClick += (_, _) =>
            {
                bool val = btn.isOn;
                setConfig(val);
                setHook(val);
                Conf.Save();
            };

            parent.Append(btn);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
