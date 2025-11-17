using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;
using UIEditor.Core.Helpers;
using UIEditor.Core.MainMenuEditor.Helpers;
using UIEditor.Core.MainMenuEditor.Hooks;
using UIEditor.UI;
using static Terraria.GameContent.UI.States.UICharacterCreation;

namespace UIEditor.Core.MainMenuEditor.UI.Sections
{
    internal sealed class LogoSection : BaseSection
    {
        private enum LogoTab { Scale, Rotation, Color, Position }
        private LogoTab tab = LogoTab.Scale;   // start on Scale

        // ─── UI elements ────────────────────────────────────────────────
        private readonly UIText header;

        private readonly TabButton scaleTab, rotationTab, colorTab, positionTab;

        private readonly Slider scaleSlider, rotationSlider;
        private readonly Slider xPosSlider, yPosSlider;

        private readonly UIPanel colorPanel;
        private readonly UIColoredSlider hue, sat, lum;
        private readonly SmallColoredImageButton copyBtn, pasteBtn, randBtn;
        private readonly UIPanel hexTag;
        private readonly UIText hexText;

        private readonly ResetButton reset;
        private readonly ResetButton resetLogoBtn;

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
            scaleTab = MakeTab(Ass.S, LogoTab.Scale, 0);
            rotationTab = MakeTab(Ass.R, LogoTab.Rotation, 32);
            positionTab = MakeTab(Ass.P, LogoTab.Position, 64);
            colorTab = MakeTab(Ass.C, LogoTab.Color, 94);
            if (ColorHelper.TryParseHex(Conf.C.MainMenuLogo.Color, out var color))
            {
                colorTab.SetColor(color);
            }

            // ─── 3. Reset ──────────────────────────────────────────────
            reset = new ResetButton { Left = { Pixels = 6 }, Top = { Pixels = 6 } };
            reset.OnLeftMouseDown += (_, _) =>
            {
                switch (tab)
                {
                    case LogoTab.Scale:
                        LogoHook.Scale = 1;
                        scaleSlider.Ratio = 0.1f;
                        break;

                    case LogoTab.Rotation:
                        LogoHook.Rotation = 0;
                        rotationSlider.Ratio = 0;
                        break;

                    case LogoTab.Position:
                        LogoHook.OffsetX = LogoHook.OffsetY = 0;
                        xPosSlider.Ratio = yPosSlider.Ratio = 0.5f;
                        break;

                    case LogoTab.Color:
                        LogoHook.Color = Color.White;
                        hsl = Main.rgbToHsl(LogoHook.Color);
                        hexText.SetText(ColorHelper.ColorToHex(LogoHook.Color));
                        break;
                }
            };

            // ─── 4. Sliders ────────────────────────────────────────────
            scaleSlider = new Slider { Top = { Pixels = 45 + 32 } };
            scaleSlider.Ratio = ColorHelper.InverseLerp(0, 10, Conf.C.MainMenuLogo.Scale);
            scaleSlider.OnDrag += v => Conf.C.MainMenuLogo.Scale = LogoHook.Scale = MathHelper.Lerp(0, 10, v);

            rotationSlider = new Slider { Top = { Pixels = 45 + 32 } };
            rotationSlider.Ratio = ColorHelper.InverseLerp(0, 10, Conf.C.MainMenuLogo.Rotation);
            rotationSlider.OnDrag += v => Conf.C.MainMenuLogo.Rotation = LogoHook.Rotation = MathHelper.Lerp(0, 6.28f, v);

            xPosSlider = new Slider { Top = { Pixels = 45 + 32 } };
            xPosSlider.Ratio = ColorHelper.InverseLerp(-Main.screenWidth * 0.5f,
                                                       +Main.screenWidth * 0.5f, Conf.C.MainMenuLogo.OffsetX);

            xPosSlider.OnDrag += v =>
            Conf.C.MainMenuLogo.OffsetX =
                LogoHook.OffsetX = MathHelper.Lerp(-Main.screenWidth * 0.5f,
                                                       +Main.screenWidth * 0.5f, v);

            yPosSlider = new Slider { Top = { Pixels = 75 + 32 } };
            yPosSlider.Ratio = ColorHelper.InverseLerp(-Main.screenWidth * 0.5f,
                                                       +Main.screenWidth * 0.5f, Conf.C.MainMenuLogo.OffsetX);
            yPosSlider.OnDrag += v =>
            Conf.C.MainMenuLogo.OffsetY =
                LogoHook.OffsetY = MathHelper.Lerp(-Main.screenHeight * 0.5f,
                                                       +Main.screenHeight * 0.5f, v);
            xPosSlider.Ratio = yPosSlider.Ratio = 0.5f;

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

            copyBtn = MakeBtn("Copy", 0, rowY, () => ColorHelper.CopyHex(() => LogoHook.Color));
            pasteBtn = MakeBtn("Paste", 32, rowY, () => ColorHelper.PasteHex(ref hsl, c => LogoHook.Color = c, hexText));
            randBtn = MakeBtn("Randomize", 64, rowY, () => ColorHelper.RandomizeColor(ref hsl, c => LogoHook.Color = c, hexText));

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
            fileText = new OnOffTextButton("No file chosen", NoOnOff: true, ShowOnHover: true)
            {
                HAlign = 0.5f,
                Left = { Pixels = 50 },
                Top = { Pixels = 160 }
            };
            string filePath = Conf.C.MainMenuLogo.LogoFileName;
            if (!string.IsNullOrEmpty(filePath))
            {
                fileText.SetText(filePath);
            }
            fileChoose.OnLeftClick += (_, _) =>
            {
                string file = FileUploadHelper.OpenFileDialog();
                Texture2D tex = FileUploadHelper.ReadAndCreateTextureFromPath(file);
                LogoHook.CustomLogoTexture = tex; // set the texture in the hook
                Conf.C.MainMenuLogo.LogoFileName = tex.Name;
                Conf.Save();

                if (!string.IsNullOrEmpty(tex.Name))
                    fileText.SetText(tex.Name);
            };

            resetLogoBtn = new()
            {
                VAlign = 1f
            };
            resetLogoBtn.OnLeftClick += (_, _) =>
            {
                LogoHook.ResetCustomLogo();      // clear the loaded texture
                fileText.SetText("No file chosen");
            };

            // ─── initialise mainState ───────────────────────────────────────
            hsl = Main.rgbToHsl(LogoHook.Color);
            SelectTab(LogoTab.Scale);          // show initial controls
        }

        // ───────────────────────────────────────────────────────────────
        //  Helper builders
        // ───────────────────────────────────────────────────────────────
        private TabButton MakeTab(Asset<Texture2D> tex, LogoTab t, float left)
        {
            var b = new TabButton(t.ToString())
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
            {
                ColorHelper.ApplyHslValue(ref hsl, id, v, c => LogoHook.Color = c, hexText);

                if (tab == LogoTab.Color)
                {
                    Conf.C.MainMenuLogo.Color = ColorHelper.ColorToHex(Main.hslToRgb(hsl));
                    Conf.Save();
                }
            };

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

            // visual mainState for tab buttons
            scaleTab.SetSelected(t == LogoTab.Scale);
            rotationTab.SetSelected(t == LogoTab.Rotation);
            colorTab.SetSelected(t == LogoTab.Color);
            positionTab.SetSelected(t == LogoTab.Position);

            // purge everything that can change per-tab
            ClearDynamic();

            Append(reset);

            switch (t)
            {
                case LogoTab.Scale:
                    header.SetText($"Logo Scale: {LogoHook.Scale:F2}");
                    Append(scaleSlider);
                    break;

                case LogoTab.Rotation:
                    header.SetText($"Logo Rotation: {LogoHook.Rotation:F2}");
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

            // color header custom pos
            if (t == LogoTab.Color)
            {
                header.Top.Set(8, 0);
                header.HAlign = 1f;
                header.Left.Set(-32, 0);
            }
            else
            {
                // file chooser only on non-colour tabs
                Append(fileText);
                Append(fileChoose);
                Append(resetLogoBtn);

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

            reset.Remove();
            resetLogoBtn.Remove();
            fileChoose.Remove();
            fileText.Remove();
        }

        // live label updates
        public override void Update(GameTime gt)
        {
            base.Update(gt);
            switch (tab)
            {
                case LogoTab.Scale:
                    header.SetText($"Logo Scale: {LogoHook.Scale:F2}");
                    break;
                case LogoTab.Rotation:
                    header.SetText($"Logo Rotation: {LogoHook.Rotation:F2}");
                    break;
                case LogoTab.Position:
                    header.SetText($"Logo Position: {LogoHook.OffsetX:F2}, {LogoHook.OffsetY:F2}");
                    break;
                case LogoTab.Color:
                    hexText.SetText(ColorHelper.ColorToHex(LogoHook.Color));
                    colorTab.SetColor(LogoHook.Color);
                    break;
            }
        }
    }
}
