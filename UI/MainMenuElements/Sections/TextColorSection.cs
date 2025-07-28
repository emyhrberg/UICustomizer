using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;
using UICustomizer.Common.Systems.Hooks.MainMenu;
using static Terraria.GameContent.UI.States.UICharacterCreation;

namespace UICustomizer.UI.MainMenuElements.Sections
{
    internal class TextColorSection : BaseSection
    {
        private enum ColorTab { Fill, Outline, Hover, Scale, Position }
        private ColorTab tab = ColorTab.Fill;

        private readonly UIText header;
        private readonly TabButton fillTab, outlineTab, hueTab, scaleTab, positionTab;
        private readonly UIColoredSlider hue, sat, lum;
        private readonly UIPanel panel;
        private readonly UIPanel hexTag;
        private readonly UIText hexText;
        private readonly ResetButton reset;
        private Vector3 hsl; // current HSL

        // Extra sliders
        private ZoeSlider scaleSlider, xPosSlider, yPosSlider;

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
            fillTab = MakeTab(Ass.F, ColorTab.Fill, 0);
            outlineTab = MakeTab(Ass.O, ColorTab.Outline, 32);
            hueTab = MakeTab(Ass.H, ColorTab.Hover, 32 + 32);
            scaleTab = MakeTab(Ass.S, ColorTab.Scale, 32 + 32 + 32);
            positionTab = MakeTab(Ass.P, ColorTab.Position, 32 + 32 + 32 + 32);

            reset = new ResetButton();
            reset.OnLeftMouseDown += (_, _) =>
            {
                // 1) pick the appropriate default
                Color def = tab switch
                {
                    ColorTab.Fill => MainMenuTextColorHook.DefaultMenuColours.Fill,
                    ColorTab.Outline => MainMenuTextColorHook.DefaultMenuColours.Outline,
                    ColorTab.Hover => MainMenuTextColorHook.DefaultMenuColours.Hover,
                    _ => Color.White // default case to white
                };
                hsl = Main.rgbToHsl(def);

                string hex = $"#{def.R:X2}{def.G:X2}{def.B:X2}";
                switch (tab)
                {
                    case ColorTab.Fill: 
                        Conf.C.MainMenuTextColor.FillColor = hex;
                        fillTab.SetColor(def);
                        break;
                    case ColorTab.Outline: 
                        Conf.C.MainMenuTextColor.OutlineColor = hex;
                        outlineTab.SetColor(def);
                        break;
                    case ColorTab.Hover: 
                        Conf.C.MainMenuTextColor.HoverColor = hex;
                        hueTab.SetColor(def);
                        Log.Info("color set to Hover: " + def);
                        break;
                    case ColorTab.Scale:
                        MainMenuTextColorHook.Scale = 1;
                        scaleSlider.Ratio = 0;
                        break;
                    case ColorTab.Position:
                        MainMenuTextColorHook.OffsetX = MainMenuTextColorHook.OffsetY = 0;
                        xPosSlider.Ratio = yPosSlider.Ratio = 0.5f;
                        break;
                }
                Log.Info("color set to def: " + def);

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

            // 2 extra sliders
            scaleSlider = new ZoeSlider { Top = { Pixels = 45 + 32 } };
            scaleSlider.OnDrag += v => MainMenuTextColorHook.Scale = MathHelper.Lerp(0, 10, v);

            xPosSlider = new ZoeSlider { Top = { Pixels = 45 + 32 } };
            xPosSlider.OnDrag += v =>
            {
                MainMenuTextColorHook.OffsetX = MathHelper.Lerp(-Main.screenWidth * 0.5f,
                                                       +Main.screenWidth * 0.5f, v); 
                Log.Info("x pos:" + MainMenuTextColorHook.OffsetX);
            };

            yPosSlider = new ZoeSlider { Top = { Pixels = 75 + 32 } };
            yPosSlider.OnDrag += v =>
                MainMenuTextColorHook.OffsetY = MathHelper.Lerp(-Main.screenHeight * 0.5f,
                                                       +Main.screenHeight * 0.5f, v);
            xPosSlider.Ratio = yPosSlider.Ratio = 0.5f;

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

        private TabButton MakeTab(Asset<Texture2D> tex, ColorTab tab, float left)
        {
            const float iconSize = 22f;

            string tooltip = tab.ToString() + " Text Color";

            TabButton btn = new(tooltip);
            btn.Left.Set(left, 0f);
            btn.Top.Set(4f, 0f);
            btn.Width.Set(iconSize, 0f);
            btn.Height.Set(iconSize, 0f);

            btn.SetColor(tab switch
            {
                ColorTab.Fill => MainMenuTextColorHook.FillColor,
                ColorTab.Outline => MainMenuTextColorHook.OutlineColor,
                ColorTab.Hover => MainMenuTextColorHook.HoverColor,
                _ => Color.White
            });

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
                var (_, dynSet) = GetColorAccessors();           // fetch *current* tab
                ColorHelper.ApplyHslValue(ref hsl, id, v, dynSet, hexText);

                // Save to config
                switch (tab)
                {
                    // 1) Colour tabs ────────────────────────────────────────────
                    case ColorTab.Fill:
                        Conf.C.MainMenuTextColor.FillColor = ColorHelper.ToHex(Main.hslToRgb(hsl));
                        break;
                    case ColorTab.Outline:
                        Conf.C.MainMenuTextColor.OutlineColor = ColorHelper.ToHex(Main.hslToRgb(hsl));
                        break;
                    case ColorTab.Hover:
                        Conf.C.MainMenuTextColor.HoverColor = ColorHelper.ToHex(Main.hslToRgb(hsl));
                        break;
                }
                Conf.Save();
            };

            // 3) Per-slider preview gradient
            Func<float, Color> gradient = x => id switch
            {
                HSLSliderId.Hue => Main.hslToRgb(x, 1f, 0.5f),
                HSLSliderId.Saturation => Main.hslToRgb(hsl.X, x, hsl.Z),
                HSLSliderId.Luminance => Main.hslToRgb(hsl.X, hsl.Y, x)
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

            // Clear all
            panel.Remove();
            hexTag.Remove();
            scaleSlider.Remove();
            xPosSlider.Remove();
            yPosSlider.Remove();

            // Rebuild all
            switch (tab)
            {
                // 1) Colour tabs ────────────────────────────────────────────
                case ColorTab.Fill:
                case ColorTab.Outline:
                case ColorTab.Hover:
                    {
                        hsl = Main.rgbToHsl(GetColorAccessors().get());
                        hexText.SetText(ColorHelper.ToHex(GetColorAccessors().get()));
                        Append(panel);      // HSL sliders
                        Append(hexTag);     // big HEX label
                        break;
                    }
                case ColorTab.Scale:
                    {
                        Append(scaleSlider);
                        break;
                    }
                case ColorTab.Position:
                    {
                        Append(xPosSlider);
                        Append(yPosSlider);
                        break;
                    }
            }

            string text = tab.ToString();
            if (text.Length <= 7)
            {
                header.SetText(text, 1.0f, false);
            }
            else if (text.Length > 7 && text.Length <= 10)
            {
                header.SetText(text, 0.91f, false);
            }
            else
            {
                header.SetText(text, 0.7f, false);
            }

            fillTab.SetSelected(tab == ColorTab.Fill);
            outlineTab.SetSelected(tab == ColorTab.Outline);
            hueTab.SetSelected(tab == ColorTab.Hover);
            scaleTab.SetSelected(tab == ColorTab.Scale);
            positionTab.SetSelected(tab == ColorTab.Position);

            hsl = Main.rgbToHsl(GetColorAccessors().get());
            hexText.SetText(ColorHelper.ToHex(GetColorAccessors().get()));

            if (tab == ColorTab.Scale || tab == ColorTab.Position)
            {
                // big text below the tabs
                header.Top.Set(8 + 32, 0);
                header.HAlign = 0.5f;
                header.Left.Set(0, 0);
            }
            else
            {
                // top right-aligned color
                header.Top.Set(8, 0);
                header.HAlign = 1f;
                header.Left.Set(-32, 0);
            }
        }

        private (Func<Color> get, Action<Color> set) GetColorAccessors()
        {
            return tab switch
            {
                ColorTab.Fill => (() => MainMenuTextColorHook.FillColor, c => MainMenuTextColorHook.FillColor = c),
                ColorTab.Outline => (() => MainMenuTextColorHook.OutlineColor, c => MainMenuTextColorHook.OutlineColor = c),
                ColorTab.Hover => (() => MainMenuTextColorHook.HoverColor, c => MainMenuTextColorHook.HoverColor = c),
                _ => (() => Color.White,
                _  => { })
            };
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            Color current = Main.hslToRgb(hsl);

            switch (tab)
            {
                case ColorTab.Fill:
                    fillTab.SetColor(current);
                    break;
                case ColorTab.Outline:
                    outlineTab.SetColor(current);
                    break;
                case ColorTab.Hover:
                    hueTab.SetColor(current);
                    break;
                case ColorTab.Scale:
                    header.SetText($"Text Scale: {MainMenuTextColorHook.Scale:F2}");
                    fillTab.SetColor(current);
                    break;
                case ColorTab.Position:
                    header.SetText($"Text Position: {MainMenuTextColorHook.OffsetX:F2}, {MainMenuTextColorHook.OffsetY:F2}");
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
