using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.OS;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;
using UICustomizer.Common.Configs;
using UICustomizer.Common.Systems.Hooks.MainMenu;
using static Terraria.GameContent.UI.States.UICharacterCreation;

namespace UICustomizer.UI.MainMenuElements
{
    internal class BoxSectionHSLSlider : UIPanel
    {
        private enum ColorTab { Fill, Outline, Hover }
        private ColorTab _tab = ColorTab.Fill;

        private readonly UIText _header;
        private readonly SmallColoredImageButton _btnF, _btnO, _btnH;
        private readonly UIColoredSlider _hue, _sat, _lum;
        private readonly UIPanel _panel;
        private readonly UIPanel _hexTag;
        private readonly UIText _hexText;
        private readonly ResetButton _reset;
        private Vector3 _hsl; // current HSL

        public BoxSectionHSLSlider()
        {
            Width.Set(-28, 1);
            Height.Set(197, 0);
            HAlign = 0.5f;
            PaddingTop = 4f;
            Left.Set(12, 0);
            OverflowHidden = false;

            // 1) Header
            Append(_header = new UIText("Fill")
            {
                HAlign = 1f,
                Left = { Pixels = -32 },
                Top = { Pixels = 8 }
            });

            // 2) Tabs
            _btnF = MakeTab(Ass.F, ColorTab.Fill, 0, "Fill");
            _btnO = MakeTab(Ass.O, ColorTab.Outline, 32, "Outline");
            _btnH = MakeTab(Ass.H, ColorTab.Hover, 32 + 32, "Hover");

            _reset = new ResetButton();
            _reset.OnLeftMouseDown += (_, _) =>
            {
                // 1) pick the appropriate default
                Color def = _tab switch
                {
                    ColorTab.Fill => Color.Gray,
                    ColorTab.Outline => Color.Black,
                    _ => Main.OurFavoriteColor
                };

                // 2) apply it to the live hooks
                SetHookColor(def);
                _hsl = Main.rgbToHsl(def);
                _hexText.SetText(GetHex());

                // 3) update config
                string hex = $"#{def.R:X2}{def.G:X2}{def.B:X2}";
                switch (_tab)
                {
                    case ColorTab.Fill: Conf.C.FillColor = hex; break;
                    case ColorTab.Outline: Conf.C.OutlineColor = hex; break;
                    default: Conf.C.HoverColor = hex; break;
                }

                // 4) persist to disk
                Conf.Save();
            };
            Append(_reset);

            // 3) HSL Panel (220×104), offset down by 10 px + 30 px header area
            Append(_panel = new UIPanel
            {
                Width = StyleDimension.FromPixelsAndPercent(220f, 0f),
                Height = StyleDimension.FromPixelsAndPercent(104f, 0f),
                HAlign = 0.5f,
                VAlign = 0f,
                Top = StyleDimension.FromPixels(10f + 30f)
            });
            _panel.SetPadding(0);

            // 4) Three sliders, spaced 30 px
            _hue = MakeSlider(HSLSliderId.Hue);
            _sat = MakeSlider(HSLSliderId.Saturation);
            _lum = MakeSlider(HSLSliderId.Luminance);   // “Lightness” in tModLoader enum
            _panel.Append(_hue);
            _panel.Append(_sat);
            _panel.Append(_lum);

            // ─── 5) Row Y position just under the slider panel ───
            float rowY = _panel.Top.Pixels + _panel.Height.Pixels + 8f;
            float halfW = _panel.Width.Pixels * 0.5f;          // 220 px / 2 = 110

            // 6) Copy / Paste / Randomize – anchored to _panel’s left edge
            MakeBtn("Copy", 0f, rowY, CopyHex);
            MakeBtn("Paste", 32f, rowY, PasteHex);
            MakeBtn("Randomize", 32 + 32f, rowY, RandomizeColor);

            // ─── 7) Big hex-value panel, right-aligned to the slider panel ───
            _hexTag = new UIPanel
            {
                Width = StyleDimension.FromPixelsAndPercent(96f, 0f),   // bigger
                Height = StyleDimension.FromPixelsAndPercent(24f, 0f),
                HAlign = 0.5f,                                            // anchor @ centre
                Top = { Pixels = rowY }
            };
            // shift right so its **right edge** matches the slider panel’s right edge
            _hexTag.Left.Set(halfW - _hexTag.Width.Pixels * 0.5f, 0f);

            _hexTag.SetPadding(0f);               // keep UIPanel’s default colours
            _hexTag.Append(_hexText = new UIText(string.Empty)
            {
                HAlign = 0.5f,
                VAlign = 0.5f
            });
            Append(_hexTag);


            // ─── 8) Start on the “Fill” tab ───
            SelectTab(ColorTab.Fill);
        }

        private SmallColoredImageButton MakeTab(Asset<Texture2D> tex, ColorTab tab, float left, string tooltip)
        {
            const float iconSize = 22f;

            var btn = new SmallColoredImageButton(tex, tooltip);
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
                HSLSliderId.Hue => _hsl.X,
                HSLSliderId.Saturation => _hsl.Y,
                _ => _hsl.Z
            };

            // 2) Push edits through the common routine
            Action<float> setter = v => ApplyHSLValue(id, v);

            // 3) Per-slider preview gradient
            Func<float, Color> gradient = x => id switch
            {
                HSLSliderId.Hue => Main.hslToRgb(x, 1f, 0.5f),
                HSLSliderId.Saturation => Main.hslToRgb(_hsl.X, x, _hsl.Z),
                _ => Main.hslToRgb(_hsl.X, _hsl.Y, x)
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
            _tab = tab;
            //_header.SetText(tab + " Color");
            _header.SetText(tab.ToString());
            _btnF.SetSelected(tab == ColorTab.Fill);
            _btnO.SetSelected(tab == ColorTab.Outline);
            _btnH.SetSelected(tab == ColorTab.Hover);

            _hsl = Main.rgbToHsl(GetHookColor());
            _hexText.SetText(GetHex());
        }

        private void PasteHex()
        {
            var s = Platform.Get<IClipboard>().Value.TrimStart('#');
            if (s.Length == 6 && uint.TryParse(s, NumberStyles.HexNumber, null, out var u))
            {
                var c = new Color(
                    r: (byte)((u >> 16) & 0xFF),
                    g: (byte)((u >> 8) & 0xFF),
                    b: (byte)(u & 0xFF));

                _hsl = Main.rgbToHsl(c);
                SetHookColor(c);
                _hexText.SetText(GetHex());
            }
        }

        private void RandomizeColor()
        {
            _hsl = new Vector3(Main.rand.NextFloat(), Main.rand.NextFloat(), Main.rand.NextFloat());
            SetHookColor(Main.hslToRgb(_hsl.X, _hsl.Y, _hsl.Z * 0.85f + 0.15f));
            _hexText.SetText(GetHex());
        }

        private void ApplyHSLValue(HSLSliderId id, float value)
        {
            if (id == HSLSliderId.Hue) _hsl.X = value;
            else if (id == HSLSliderId.Saturation) _hsl.Y = value;
            else _hsl.Z = value;

            var colour = Main.hslToRgb(_hsl.X, _hsl.Y, _hsl.Z * 0.85f + 0.15f);
            SetHookColor(colour);
            _hexText.SetText($"#{colour.R:X2}{colour.G:X2}{colour.B:X2}");
            StoreColor(colour); // store the color in the config
        }

        private string GetHex()
        {
            var c = GetHookColor();
            return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
        }

        private void CopyHex() => Platform.Get<IClipboard>().Value = GetHex();

        private Color GetHookColor() => _tab switch
        {
            ColorTab.Fill => MainMenuTextColorHook.FillColor,
            ColorTab.Outline => MainMenuTextColorHook.OutlineColor,
            _ => MainMenuTextColorHook.HoverColor
        };

        private void SetHookColor(Color color)
        {
            switch (_tab)
            {
                case ColorTab.Fill: MainMenuTextColorHook.FillColor = color; break;
                case ColorTab.Outline: MainMenuTextColorHook.OutlineColor = color; break;
                default: MainMenuTextColorHook.HoverColor = color; break;
            }
            StoreColor(color); // store the color in the config
        }

        private void StoreColor(Color c)
        {
            string hex = $"#{c.R:X2}{c.G:X2}{c.B:X2}";

            switch (_tab)
            {
                case ColorTab.Fill: Conf.C.FillColor = hex; break;
                case ColorTab.Outline: Conf.C.OutlineColor = hex; break;
                case ColorTab.Hover: Conf.C.HoverColor = hex; break;
            }
            Conf.Save();                          // persists immediately
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
