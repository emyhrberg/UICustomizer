using System;
using System.IO;
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
    internal sealed class BackgroundSection : BaseSection
    {
        private enum BackgroundTab { Scale, Rotation, Color, Position }
        private BackgroundTab tab = BackgroundTab.Scale;   // start on Scale

        // ─── UI elements ────────────────────────────────────────────────
        private readonly UIText header;

        private readonly TabButton scaleTab, rotationTab, colorTab, positionTab;

        private readonly ZoeSlider scaleSlider, rotationSlider;
        private readonly ZoeSlider xPosSlider, yPosSlider;

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

        public BackgroundSection()
        {
            Height.Set(200, 0);

            // ─── 1. Header ──────────────────────────────────────────────
            header = new UIText(string.Empty)
            {
                HAlign = 0.5f,
                Top = { Pixels = 8 + 32 }
            };
            Append(header);

            // ─── 2. Tab buttons ────────────────────────────────────────
            scaleTab = MakeTab(Ass.S, BackgroundTab.Scale, 0);
            rotationTab = MakeTab(Ass.R, BackgroundTab.Rotation, 32);
            positionTab = MakeTab(Ass.P, BackgroundTab.Position, 64);
            colorTab = MakeTab(Ass.C, BackgroundTab.Color, 94);
            if (ColorHelper.TryParseHex(Conf.C.MainMenuBackground.Color, out var color))
            {
                colorTab.SetColor(color);
            }

            // ─── 3. Reset ──────────────────────────────────────────────
            reset = new ResetButton { Left = { Pixels = 6 }, Top = { Pixels = 6 } };
            reset.OnLeftMouseDown += (_, _) =>
            {
                switch (tab)
                {
                    case BackgroundTab.Scale:
                        BackgroundHook.Scale = 1;
                        scaleSlider.Ratio = 0.1f;
                        break;

                    case BackgroundTab.Rotation:
                        BackgroundHook.Rotation = 0;
                        rotationSlider.Ratio = 0;
                        break;

                    case BackgroundTab.Position:
                        BackgroundHook.OffsetX = BackgroundHook.OffsetY = 0;
                        xPosSlider.Ratio = yPosSlider.Ratio = 0.5f;
                        break;

                    case BackgroundTab.Color:
                        BackgroundHook.Color = Color.White;
                        hsl = Main.rgbToHsl(BackgroundHook.Color);
                        hexText.SetText(ColorHelper.ToHex(BackgroundHook.Color));
                        break;
                }
            };

            resetLogoBtn = new()
            {
                VAlign = 1f
            };
            resetLogoBtn.OnLeftClick += (_, _) =>
            {
                BackgroundHook.ResetCustomBackground();      // clear the loaded texture
                fileText.SetText("No file chosen");
            };

            // ─── 4. Sliders ────────────────────────────────────────────
            scaleSlider = new ZoeSlider { Top = { Pixels = 45 + 32 } };
            scaleSlider.Ratio = ColorHelper.InverseLerp(0, 10, Conf.C.MainMenuBackground.Scale);
            scaleSlider.OnDrag += v => Conf.C.MainMenuBackground.Scale = BackgroundHook.Scale = MathHelper.Lerp(0, 10, v);

            rotationSlider = new ZoeSlider { Top = { Pixels = 45 + 32 } };
            rotationSlider.Ratio = ColorHelper.InverseLerp(0, 10, Conf.C.MainMenuBackground.Rotation);
            rotationSlider.OnDrag += v => Conf.C.MainMenuBackground.Rotation = BackgroundHook.Rotation = MathHelper.Lerp(0, 6.28f, v);

            xPosSlider = new ZoeSlider { Top = { Pixels = 45 + 32 } };
            xPosSlider.Ratio = ColorHelper.InverseLerp(-Main.screenWidth * 0.5f,
                                                       +Main.screenWidth * 0.5f, Conf.C.MainMenuBackground.OffsetX);
            xPosSlider.OnDrag += v =>
            Conf.C.MainMenuBackground.OffsetX =
                BackgroundHook.OffsetX = MathHelper.Lerp(-Main.screenWidth * 0.5f,
                                                       +Main.screenWidth * 0.5f, v);


            yPosSlider = new ZoeSlider { Top = { Pixels = 75 + 32 } };
            yPosSlider.Ratio = ColorHelper.InverseLerp(-Main.screenWidth * 0.5f,
                                                       +Main.screenWidth * 0.5f, Conf.C.MainMenuBackground.OffsetY);
            yPosSlider.OnDrag += v =>
            Conf.C.MainMenuBackground.OffsetY =
                BackgroundHook.OffsetY = MathHelper.Lerp(-Main.screenHeight * 0.5f,
                                                       +Main.screenHeight * 0.5f, v);

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

            copyBtn = MakeBtn("Copy", 0, rowY, () => ColorHelper.CopyHex(() => BackgroundHook.Color));
            pasteBtn = MakeBtn("Paste", 32, rowY, () => ColorHelper.PasteHex(ref hsl, c => BackgroundHook.Color = c, hexText));
            randBtn = MakeBtn("Randomize", 64, rowY, () => ColorHelper.RandomizeColor(ref hsl, c => BackgroundHook.Color = c, hexText));

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
            fileChoose.OnLeftClick += (_, _) =>
            {
                string file = FileUploadHelper.OpenFileDialog();
                if (string.IsNullOrWhiteSpace(file) || !File.Exists(file))
                    return;

                Texture2D tex = FileUploadHelper.ReadAndCreateTextureFromPath(file);
                if (tex == null)
                    return;

                BackgroundHook.CustomBackgroundTexture = tex;
                Conf.C.MainMenuBackground.BackgroundFileName = file;   // store path
                Conf.Save();

                fileText.SetText(file);
            };
            string filePath = Conf.C.MainMenuBackground.BackgroundFileName;
            if (!string.IsNullOrEmpty(filePath))
                fileText.SetText(filePath);

            // ─── initialise state ───────────────────────────────────────
            hsl = Main.rgbToHsl(BackgroundHook.Color);
            SelectTab(BackgroundTab.Scale);          // show initial controls
        }

        // ───────────────────────────────────────────────────────────────
        //  Helper builders
        // ───────────────────────────────────────────────────────────────
        private TabButton MakeTab(Asset<Texture2D> tex, BackgroundTab t, float left)
        {
            TabButton b = new(t.ToString())
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
                ColorHelper.ApplyHslValue(ref hsl, id, v, c => BackgroundHook.Color = c, hexText);

                // Save to config
                if (tab == BackgroundTab.Color)
                {
                    Conf.C.MainMenuBackground.Color = ColorHelper.ToHex(Main.hslToRgb(hsl));
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
        private void SelectTab(BackgroundTab t)
        {
            tab = t;

            // visual state for tab buttons
            scaleTab.SetSelected(t == BackgroundTab.Scale);
            rotationTab.SetSelected(t == BackgroundTab.Rotation);
            colorTab.SetSelected(t == BackgroundTab.Color);
            positionTab.SetSelected(t == BackgroundTab.Position);

            // purge everything that can change per-tab
            ClearDynamic();

            Append(reset);

            switch (t)
            {
                case BackgroundTab.Scale:
                    header.SetText($"Background Scale: {BackgroundHook.Scale:F2}");
                    Append(scaleSlider);
                    break;

                case BackgroundTab.Rotation:
                    header.SetText($"Background Rotation: {BackgroundHook.Rotation:F2}");
                    Append(rotationSlider);
                    break;

                case BackgroundTab.Position:
                    header.SetText("Background Position");
                    Append(xPosSlider);
                    Append(yPosSlider);
                    break;

                case BackgroundTab.Color:
                    header.SetText("BG Color");
                    Append(colorPanel);
                    Append(copyBtn);
                    Append(pasteBtn);
                    Append(randBtn);
                    Append(hexTag);
                    break;
            }

            // color header custom pos
            if (t == BackgroundTab.Color)
            {
                header.Top.Set(8, 0);
                header.HAlign = 1f;
                header.Left.Set(-32, 0);
            }
            else
            {
                // file chooser only on non-colour tabs
                Append(fileChoose);
                Append(fileText);
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

        private void ResetCurrentTab()
        {

        }

        // live label updates
        public override void Update(GameTime gt)
        {
            base.Update(gt);
            switch (tab)
            {
                case BackgroundTab.Scale:
                    header.SetText($"Background Scale: {BackgroundHook.Scale:F2}");
                    break;
                case BackgroundTab.Rotation:
                    header.SetText($"Background Rotation: {BackgroundHook.Rotation:F2}");
                    break;
                case BackgroundTab.Position:
                    header.SetText($"Background Position: {BackgroundHook.OffsetX:F2}, {BackgroundHook.OffsetY:F2}");
                    break;
                case BackgroundTab.Color:
                    hexText.SetText(ColorHelper.ToHex(BackgroundHook.Color));
                    colorTab.SetColor(BackgroundHook.Color);
                    break;
            }
        }
    }
}
