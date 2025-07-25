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
    internal sealed class LogoSection : BaseSection
    {
        private enum LogoTab { Scale, Rotation, Color, Position }
        private LogoTab tab = LogoTab.Scale;   // start on Scale

        // ─── UI elements ────────────────────────────────────────────────
        private readonly UIText header;

        private readonly SmallColoredImageButton btnScale, btnRot, btnCol, btnPos;

        private readonly ZoeSlider scaleSlider, rotationSlider;
        private readonly ZoeSlider xPosSlider, yPosSlider;

        private readonly UIPanel colorPanel;
        private readonly UIColoredSlider hue, sat, lum;
        private readonly SmallColoredImageButton copyBtn, pasteBtn, randBtn;
        private readonly UIPanel hexTag;
        private readonly UIText hexText;

        private readonly ResetButton resetBtn;

        private readonly OnOffTextButton fileChoose;
        private readonly UIText fileText;

        // working HSL
        private Vector3 hsl;

        public LogoSection()
        {
            Height.Set(200, 0);

            // ─── 1. Header ──────────────────────────────────────────────
            header = new UIText(string.Empty)
            {
                HAlign = 0.5f,
                //Left = { Pixels = -32 },
                Top = { Pixels = 8 + 32 }
            };
            Append(header);

            // ─── 2. Tab buttons ────────────────────────────────────────
            btnScale = MakeTab(Ass.S, LogoTab.Scale, 0);
            btnRot = MakeTab(Ass.R, LogoTab.Rotation, 32);
            btnPos = MakeTab(Ass.P, LogoTab.Position, 64);
            btnCol = MakeTab(Ass.C, LogoTab.Color, 94);

            // ─── 3. Reset ──────────────────────────────────────────────
            resetBtn = new ResetButton { Left = { Pixels = 6 }, Top = { Pixels = 6 } };
            resetBtn.OnLeftMouseDown += (_, _) => ResetCurrentTab();

            // ─── 4. Sliders ────────────────────────────────────────────
            scaleSlider = new ZoeSlider { Top = { Pixels = 45+32 } };
            scaleSlider.OnDrag += v => LogoHook.LogoScale = MathHelper.Lerp(0, 10, v);

            rotationSlider = new ZoeSlider { Top = { Pixels = 45+32 } };
            rotationSlider.OnDrag += v => LogoHook.LogoRotation = MathHelper.Lerp(0, 6, v);

            xPosSlider = new ZoeSlider { Top = { Pixels = 45 + 32 } };
            xPosSlider.OnDrag += v => 
                LogoHook.LogoOffsetX = MathHelper.Lerp(-Main.screenWidth * 0.5f,
                                                       +Main.screenWidth * 0.5f, v);

            yPosSlider = new ZoeSlider { Top = { Pixels = 75 + 32 } };
            yPosSlider.OnDrag += v =>
                LogoHook.LogoOffsetX = MathHelper.Lerp(-Main.screenWidth * 0.5f,
                                                       +Main.screenWidth * 0.5f, v);

            // ─── 5. Colour panel ───────────────────────────────────────
            colorPanel = new UIPanel
            {
                Width = StyleDimension.FromPixelsAndPercent(220, 0),
                Height = StyleDimension.FromPixelsAndPercent(104, 0),
                HAlign = 0.5f,
                Top = StyleDimension.FromPixels(40)   // 10 padding + 30 header
            };
            colorPanel.SetPadding(0);

            hue = MakeHslSlider(HSLSliderId.Hue);
            sat = MakeHslSlider(HSLSliderId.Saturation);
            lum = MakeHslSlider(HSLSliderId.Luminance);

            colorPanel.Append(hue);
            colorPanel.Append(sat);
            colorPanel.Append(lum);

            // helper buttons & hex tag
            float rowY = colorPanel.Top.Pixels + colorPanel.Height.Pixels + 8;

            copyBtn = MakeBtn("Copy", 0, rowY, () => ColorHelper.CopyHex(() => LogoHook.LogoColor));
            pasteBtn = MakeBtn("Paste", 32, rowY, () => ColorHelper.PasteHex(ref hsl, c => LogoHook.LogoColor = c, hexText));
            randBtn = MakeBtn("Randomize", 64, rowY, () => ColorHelper.RandomizeColor(ref hsl, c => LogoHook.LogoColor = c, hexText));

            hexTag = new UIPanel
            {
                Width = StyleDimension.FromPixelsAndPercent(96f, 0f),
                Height = StyleDimension.FromPixelsAndPercent(24f, 0f),
                HAlign = 0.5f,                                            // anchor @ centre
                Top = { Pixels = rowY }
            };
            hexTag.Left.Set(colorPanel.Width.Pixels * 0.5f - hexTag.Width.Pixels * 0.5f, 0f);
            hexTag.Append(hexText = new UIText(string.Empty)
            {
                HAlign = 0.5f,
                VAlign = 0.5f
            });
            Append(hexTag);

            // ─── 6. File chooser (always at the bottom, hidden on Color) ─
            fileChoose = new OnOffTextButton("Choose File", NoOnOff: true)
            {
                HAlign = 0,
                Left = { Pixels = 10 },
                Top = { Pixels = 160 }
            };
            fileText = new UIText("No file chosen", 0.9f)
            {
                HAlign = 0.5f,
                Left = { Pixels = 50 },
                Top = { Pixels = 160 }
            };
            fileChoose.OnLeftClick += (_, _) =>
            {
                var f = LogoFileHelper.UploadFile();
                if (!string.IsNullOrEmpty(f)) fileText.SetText(f);
            };

            // ─── initialise state ───────────────────────────────────────
            hsl = Main.rgbToHsl(LogoHook.LogoColor);
            SelectTab(LogoTab.Scale);          // show initial controls
        }

        // ───────────────────────────────────────────────────────────────
        //  Helper builders
        // ───────────────────────────────────────────────────────────────
        private SmallColoredImageButton MakeTab(Asset<Texture2D> tex, LogoTab t, float left)
        {
            var b = new SmallColoredImageButton(tex, t.ToString())
            {
                Left = { Pixels = left },
                Top = { Pixels = 4 },
                Width = { Pixels = 22 },
                Height = { Pixels = 22 }
            };
            b.OnLeftMouseDown += (_, _) => SelectTab(t);
            Append(b);
            return b;
        }

        private SmallColoredImageButton MakeBtn(string asset, float x, float y, Action onClick)
        {
            var b = new SmallColoredImageButton(
                Main.Assets.Request<Texture2D>($"Images/UI/CharCreation/{asset}"), tooltip: asset)
            {
                Left = { Pixels = x },
                Top = { Pixels = y },
                Width = { Pixels = 22 },
                Height = { Pixels = 22 }
            };
            b.OnLeftMouseDown += (_, _) => onClick();
            return b;           // note: NOT appended here
        }

        private UIColoredSlider MakeHslSlider(HSLSliderId id)
        {
            Func<float> get = () => id switch
            {
                HSLSliderId.Hue => hsl.X,
                HSLSliderId.Saturation => hsl.Y,
                _ => hsl.Z
            };

            Action<float> set = v =>
                ColorHelper.ApplyHslValue(ref hsl, id, v, c => LogoHook.LogoColor = c, hexText);

            Func<float, Color> grad = x => id switch
            {
                HSLSliderId.Hue => Main.hslToRgb(x, 1, 0.5f),
                HSLSliderId.Saturation => Main.hslToRgb(hsl.X, x, hsl.Z),
                _ => Main.hslToRgb(hsl.X, hsl.Y, x)
            };

            var s = new UIColoredSlider(LocalizedText.Empty, get, set, () => { }, grad, Color.Transparent)
            {
                Top = { Pixels = 30 * (int)id },
                Width = StyleDimension.FromPixelsAndPercent(-10, 1)
            };
            return s;
        }

        // ───────────────────────────────────────────────────────────────
        //  Tab logic
        // ───────────────────────────────────────────────────────────────
        private void SelectTab(LogoTab t)
        {
            tab = t;

            // visual state for tab buttons
            btnScale.SetSelected(t == LogoTab.Scale);
            btnRot.SetSelected(t == LogoTab.Rotation);
            btnCol.SetSelected(t == LogoTab.Color);
            btnPos.SetSelected(t == LogoTab.Position);

            // purge everything that can change per-tab
            ClearDynamic();

            Append(resetBtn);

            switch (t)
            {
                case LogoTab.Scale:
                    header.SetText($"Logo Scale: {LogoHook.LogoScale:F2}");
                    Append(scaleSlider);
                    break;

                case LogoTab.Rotation:
                    header.SetText($"Logo Rotation: {LogoHook.LogoRotation:F2}");
                    Append(rotationSlider);
                    break;

                case LogoTab.Position:
                    header.SetText("Logo Position");
                    Append(xPosSlider);
                    Append(yPosSlider);
                    break;

                case LogoTab.Color:
                    header.SetText("Logo Color");
                    Append(colorPanel);
                    Append(copyBtn);
                    Append(pasteBtn);
                    Append(randBtn);
                    Append(hexTag);
                    break;
            }

            // file chooser only on non-colour tabs
            if (t != LogoTab.Color) { Append(fileChoose); Append(fileText); }

            // color header custom pos
            if (t == LogoTab.Color)
            {
                header.Top.Set(8, 0);
                header.HAlign = 1f;
                header.Left.Set(-32,0);
            }
            else
            {
                header.Top.Set(8 + 32, 0);
                header.HAlign = 0.5f;
                header.Left.Set(0, 0);
            }
        }

        private void ClearDynamic()
        {
            scaleSlider.Remove();
            rotationSlider.Remove();
            xPosSlider.Remove();
            yPosSlider.Remove();

            colorPanel.Remove();
            hexTag.Remove();
            copyBtn.Remove();
            pasteBtn.Remove();
            randBtn.Remove();

            resetBtn.Remove();
            fileChoose.Remove();
            fileText.Remove();
        }

        private void ResetCurrentTab()
        {
            switch (tab)
            {
                case LogoTab.Scale:
                    LogoHook.LogoScale = 1;
                    scaleSlider.Ratio = 0;            // since InverseLerp(0,10,1)=0
                    break;

                case LogoTab.Rotation:
                    LogoHook.LogoRotation = 0;
                    rotationSlider.Ratio = 0;
                    break;

                case LogoTab.Position:
                    LogoHook.LogoOffsetX = LogoHook.LogoOffsetY = 0;
                    xPosSlider.Ratio = yPosSlider.Ratio = 0.5f;
                    break;

                case LogoTab.Color:
                    LogoHook.LogoColor = Color.White;
                    hsl = Main.rgbToHsl(LogoHook.LogoColor);
                    hexText.SetText(ColorHelper.ToHex(LogoHook.LogoColor));
                    break;
            }
        }

        // live label updates
        public override void Update(GameTime gt)
        {
            base.Update(gt);
            switch (tab)
            {
                case LogoTab.Scale:
                    header.SetText($"Logo Scale: {LogoHook.LogoScale:F2}");
                    break;
                case LogoTab.Rotation:
                    header.SetText($"Logo Rotation: {LogoHook.LogoRotation:F2}");
                    break;
                case LogoTab.Position:
                    header.SetText($"Logo Position: {LogoHook.LogoOffsetX:F2}, {LogoHook.LogoOffsetY:F2}");
                    break;
                case LogoTab.Color:
                    hexText.SetText(ColorHelper.ToHex(LogoHook.LogoColor));
                    break;
            }
        }
    }
}
