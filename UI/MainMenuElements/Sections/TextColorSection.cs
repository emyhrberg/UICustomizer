using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;
using UICustomizer.Common.Configs;
using UICustomizer.Common.Systems.Hooks.MainMenu;
using static Terraria.GameContent.UI.States.UICharacterCreation;
using static Terraria.NPC.NPCNameFakeLanguageCategoryPassthrough;

namespace UICustomizer.UI.MainMenuElements.Sections
{
    internal class TextColorSection : BaseSection
    {
        private enum ColorTab { Fill, Outline, Hover }
        private ColorTab tab = ColorTab.Fill;

        private readonly UIText header;
        private readonly SmallColoredImageButton btnF, btnO, btnH;
        private readonly UIColoredSlider hue, sat, lum;
        private readonly UIPanel panel;
        private readonly UIPanel hexTag;
        private readonly UIText hexText;
        private readonly ResetButton reset;
        private Vector3 hsl; // current HSL

        public TextColorSection()
        {
            Height.Set(200, 0);

            // 1) Header
            Append(header = new UIText("Fill")
            {
                HAlign = 1f,
                Left = { Pixels = -32 },
                Top = { Pixels = 8 }
            });

            // 2) Tabs
            btnF = MakeTab(Ass.F, ColorTab.Fill, 0);
            btnO = MakeTab(Ass.O, ColorTab.Outline, 32);
            btnH = MakeTab(Ass.H, ColorTab.Hover, 32 + 32);

            reset = new ResetButton();
            reset.OnLeftMouseDown += (_, _) =>
            {
                // 1) pick the appropriate default
                Color def = tab switch
                {
                    ColorTab.Fill => Color.Gray,
                    ColorTab.Outline => Color.Black,
                    _ => Main.OurFavoriteColor
                };
                hsl = Main.rgbToHsl(def);

                // 3) update config
                string hex = $"#{def.R:X2}{def.G:X2}{def.B:X2}";
                switch (tab)
                {
                    case ColorTab.Fill: Conf.C.FillColor = hex; break;
                    case ColorTab.Outline: Conf.C.OutlineColor = hex; break;
                    default: Conf.C.HoverColor = hex; break;
                }

                // 4) persist to disk
                Conf.Save();
            };
            Append(reset);

            // 3) HSL Panel (220×104), offset down by 10 px + 30 px header area
            Append(panel = new UIPanel
            {
                Width = StyleDimension.FromPixelsAndPercent(220f, 0f),
                Height = StyleDimension.FromPixelsAndPercent(104f, 0f),
                HAlign = 0.5f,
                VAlign = 0f,
                Top = StyleDimension.FromPixels(10f + 30f)
            });
            panel.SetPadding(0);

            // 4) Three sliders, spaced 30 px
            hue = MakeSlider(HSLSliderId.Hue);
            sat = MakeSlider(HSLSliderId.Saturation);
            lum = MakeSlider(HSLSliderId.Luminance); 
            panel.Append(hue);
            panel.Append(sat);
            panel.Append(lum);
            float rowY = panel.Top.Pixels + panel.Height.Pixels + 8f;

            // 6) Copy / Paste / Randomize
            MakeBtn("Copy", 0, rowY, () => ColorHelper.CopyHex(GetColorAccessors().get));
            MakeBtn("Paste", 32, rowY, () => ColorHelper.PasteHex(ref hsl, GetColorAccessors().set, hexText));
            MakeBtn("Randomize", 64, rowY, () => ColorHelper.RandomizeColor(ref hsl, GetColorAccessors().set, hexText));

            // ─── 7) Big hex-value colorPanel, right-aligned to the slider colorPanel ───
            hexTag = new UIPanel
            {
                Width = StyleDimension.FromPixelsAndPercent(96f, 0f),
                Height = StyleDimension.FromPixelsAndPercent(24f, 0f),
                HAlign = 0.5f,                                            // anchor @ centre
                Top = { Pixels = rowY }
            };
            hexTag.Left.Set(panel.Width.Pixels * 0.5f - hexTag.Width.Pixels * 0.5f, 0f);
            hexTag.Append(hexText = new UIText(string.Empty)
            {
                HAlign = 0.5f,
                VAlign = 0.5f
            });
            Append(hexTag);

            SelectTab(ColorTab.Fill);
        }

        private SmallColoredImageButton MakeTab(Asset<Texture2D> tex, ColorTab tab, float left)
        {
            const float iconSize = 22f;

            string tooltip = tab.ToString() + " Text";
            var btn = new SmallColoredImageButton(tex, tooltip: tooltip);
            btn.Left.Set(left, 0f);
            btn.Top.Set(4f, 0f);
            btn.Width.Set(iconSize, 0f);
            btn.Height.Set(iconSize, 0f);

            btn.OnLeftMouseDown += (_, _) => SelectTab(tab);
            Append(btn);
            return btn;
        }
        private UIColoredSlider MakeSlider(HSLSliderId id)
        {
            // 1) Current component value
            Func<float> getter = () => id switch
            {
                HSLSliderId.Hue => hsl.X,
                HSLSliderId.Saturation => hsl.Y,
                _ => hsl.Z
            };

            // 2) Push edits through the common routine
            var (get, set) = GetColorAccessors();
            Action<float> setter = v =>   
            {
                var(_, dynSet) = GetColorAccessors();           // fetch *current* tab
                ColorHelper.ApplyHslValue(ref hsl, id, v, dynSet, hexText);
            };

            // 3) Per-slider preview gradient
            Func<float, Color> gradient = x => id switch
            {
                HSLSliderId.Hue => Main.hslToRgb(x, 1f, 0.5f),
                HSLSliderId.Saturation => Main.hslToRgb(hsl.X, x, hsl.Z),
                _ => Main.hslToRgb(hsl.X, hsl.Y, x)
            };

            var slider = new UIColoredSlider(
                LocalizedText.Empty,      // no label
                getter,
                setter,
                () => { },                // no extra on-change callback
                gradient,
                Color.Transparent)
            {
                VAlign = 0f,
                HAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(-10f, 1f)
            };

            // vertical placement: 0, 30, 60 px
            slider.Top.Set(30 * (int)id, 0f);
            return slider;
        }
        private SmallColoredImageButton MakeBtn(string assetName, float left, float top, Action onClick)
        {
            var btn = new SmallColoredImageButton(
                Main.Assets.Request<Texture2D>($"Images/UI/CharCreation/{assetName}"), tooltip: assetName);

            // same local co-ordinate system as the F/O/H buttons
            btn.Left.Set(left, 0f);
            btn.Top.Set(top, 0f);
            btn.Width.Set(22f, 0f);   // explicit, matches SmallPanel size
            btn.Height.Set(22f, 0f);

            btn.OnLeftMouseDown += (_, _) => onClick();
            Append(btn);
            return btn;
        }
        private void SelectTab(ColorTab tab)
        {
            this.tab = tab;
            string text = tab.ToString() + " Text";
            header.SetText(text);
            btnF.SetSelected(tab == ColorTab.Fill);
            btnO.SetSelected(tab == ColorTab.Outline);
            btnH.SetSelected(tab == ColorTab.Hover);

            hsl = Main.rgbToHsl(GetColorAccessors().get());
            hexText.SetText(ColorHelper.ToHex(GetColorAccessors().get()));
        }

        private (Func<Color> get, Action<Color> set) GetColorAccessors()
        {
            return tab switch
            {
                ColorTab.Fill => (() => MainMenuTextColorHook.FillColor, c => MainMenuTextColorHook.FillColor = c),
                ColorTab.Outline => (() => MainMenuTextColorHook.OutlineColor, c => MainMenuTextColorHook.OutlineColor = c),
                _ => (() => MainMenuTextColorHook.HoverColor, c => MainMenuTextColorHook.HoverColor = c)
            };
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
