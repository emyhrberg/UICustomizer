using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using UICustomizer.Common.Systems;
using UICustomizer.Helpers.Layouts;

namespace UICustomizer.UI.Editor
{
    public class LayoutsTab : Tab
    {
        private bool _layoutsExpanded = true;
        private bool _optionsExpanded = false;

        public string CurrentLayoutName => LayoutHelper.CurrentLayoutName;

        public LayoutsTab() : base("Layouts")
        {
        }

        public override void Populate()
        {
            list.Clear();
            list.SetPadding(2);

            var layoutsSection = new CollapsibleSection(
                title: "Layout Presets",
                initialState: _layoutsExpanded,
                buildContent: BuildLayoutsContent,
                onToggle: () => _layoutsExpanded = !_layoutsExpanded,
                contentHeight: () => Math.Max(100, FileHelper.GetLayouts().Count() * 30 + 20)
            );

            list.Add(layoutsSection);

            // Layout options section
            var optionsSection = new CollapsibleSection(
                title: "Layout Options",
                initialState: _optionsExpanded,
                buildContent: BuildOptionsContent,
                onToggle: () => _optionsExpanded = !_optionsExpanded,
                contentHeight: () => 120f
            );

            //Gap(8);
            //optionsSection.Left.Set(0, 0);
            //optionsSection.Width.Set(-20, 1f);
            list.Add(optionsSection);
        }
        private void BuildLayoutsContent(UIElement content)
        {
            float y = 10;
            const float h = 25, pad = 5;

            foreach (var name in FileHelper.GetLayouts())
            {
                var isCurrent = name == LayoutHelper.CurrentLayoutName;
                var btn = new UIPanel
                {
                    Top = { Pixels = y },
                    Width = { Percent = 1f, Pixels = -12 },
                    Height = { Pixels = h },
                    BackgroundColor = isCurrent ? UICommon.DefaultUIBlueMouseOver * 0.5f : UICommon.DefaultUIBlue,
                    BorderColor = isCurrent ? Color.Yellow : Color.Black
                };

                var label = new UIText(name, 0.4f, true)
                {
                    HAlign = 0.5f,
                    VAlign = 0.5f,
                    TextColor = isCurrent ? Color.Yellow : Color.White
                };
                btn.Append(label);

                btn.OnLeftClick += (_, _) =>
                {
                    ModContent.GetInstance<EditorSystem>().state.editorPanel.CancelDrag();
                    LayoutHelper.ApplyLayout(name);
                    LayoutHelper.CurrentLayoutName = name;
                    LayoutHelper.SaveLastLayout();
                    Populate(); // Refresh to update colors
                };

                btn.OnRightClick += (_, _) =>
                {
                    // Open the layout file in the editor
                    if (EditorSystem.IsActive)
                    {
                        FileHelper.OpenLayoutFile(name);
                    }
                };

                // Add hover effects
                btn.OnMouseOver += (_, _) =>
                {
                    btn.BackgroundColor = isCurrent ? Color.Green * 0.8f : UICommon.DefaultUIBlueMouseOver * 0.1f;
                };

                btn.OnMouseOut += (_, _) =>
                {
                    btn.BackgroundColor = isCurrent ? Color.Green * 0.8f : UICommon.DefaultUIBlue;
                };

                content.Append(btn);
                y += h + pad;
            }
        }

        private void BuildOptionsContent(UIElement content)
        {
            float y = 10;
            const float h = 25, pad = 5;

            var actions = new (string text, string tooltip, Action act)[]
            {
                ("Open layout folder", "Open the layouts folder to edit, share or add new layouts", () => {
                    if (!EditorSystem.IsActive) return;
                    FileHelper.OpenLayoutFolder();
                }),
                ("Save this layout", "Creates and opens a new layout file with the current layout", () => {
                    if (!EditorSystem.IsActive) return;
                    FileHelper.CreateAndOpenNewLayoutFile("MyCustomLayout");
                    Populate();
                }),
                ("Remove all layouts", "Remove all layouts from the folder.\nAfter deletion, only some default layouts will be available after you reload.", () => {
                    if (!EditorSystem.IsActive) return;
                    FileHelper.DeleteAllLayouts();
                    Populate();
                })
            };

            foreach (var (txt, tooltip, act) in actions)
            {
                var btn = new UIPanel
                {
                    Top = { Pixels = y },
                    Width = { Percent = 1f, Pixels = -12 },
                    Height = { Pixels = h },
                    BackgroundColor = UICommon.DefaultUIBlue,
                    BorderColor = Color.Black
                };

                var btnText = new UIText(txt, 0.4f, true)
                {
                    HAlign = 0.5f,
                    VAlign = 0.5f,
                    TextColor = Color.White
                };
                btn.Append(btnText);

                btn.OnLeftClick += (_, _) => act();

                // Add hover effects
                btn.OnMouseOver += (_, _) => btn.BackgroundColor = UICommon.DefaultUIBlueMouseOver * 0.1f;
                btn.OnMouseOut += (_, _) => btn.BackgroundColor = UICommon.DefaultUIBlue;

                content.Append(btn);
                y += h + pad;
            }
        }
    }
}