using System;
using System.Collections.Generic;
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
        public string CurrentLayoutName => LayoutHelper.CurrentLayoutName;
        private readonly Dictionary<string, bool> expandedSections = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        public LayoutsTab() : base("Layouts") { }

        public override void Populate()
        {
            list.Clear();
            list.SetPadding(20);
            list.ListPadding = 20;
            list.Left.Set(-8, 0);
            list.Top.Set(-10, 0);

            void AddSection(string title, Action<UIElement> build, Func<float> height = null)
            {
                bool isExpanded = expandedSections.TryGetValue(title, out var v) ? v : (expandedSections[title] = true);
                var section = new CollapsibleSection(
                    title: title,
                    initialState: isExpanded,
                    buildContent: build,
                    onToggle: () => expandedSections[title] = !expandedSections[title],
                    contentHeightFunc: height
                );
                list.Add(section);
            }

            var layouts = FileHelper.GetLayouts().ToList();

            AddSection("Active Layout", BuildCurr, height: () => 50);
            AddSection("Layout Presets", BuildLayoutsContent, () => Math.Max(80, layouts.Count * 30 + 10));
            AddSection("Layout Options", BuildOptionsContent, () => 120);

            list.Recalculate();
        }

        private void BuildCurr(UIElement content)
        {
            float y = 0;

            foreach (var name in FileHelper.GetLayouts().Where(n => n == "Active"))
            {
                bool isCurrent = name == LayoutHelper.CurrentLayoutName;
                var btn = new Button("Active",
                    onClick: () =>
                    {
                        ModContent.GetInstance<EditorSystem>().state.editorPanel.CancelDrag();
                        LayoutHelper.ApplyLayout("Active");
                        LayoutHelper.CurrentLayoutName = "Active";
                        LayoutHelper.SaveLastLayout();
                        Populate();
                    },
                    tooltip: () => "Current layout",
                    maxWidth: true
                )
                {
                    Left = { Pixels = 0 },
                    Top = { Pixels = y },
                    BackgroundColor = isCurrent ? Color.Green : UICommon.DefaultUIBlue,
                    // BorderColor = isCurrent ? Color.Yellow : Color.Black
                };
                content.Append(btn);
            }
        }

        private void BuildLayoutsContent(UIElement content)
        {
            const float h = 30f, pad = 4f;
            float y = 0;
            foreach (var name in FileHelper.GetLayouts().Where(n => n != "Active"))
            {
                bool isCurrent = name == LayoutHelper.CurrentLayoutName;
                var btn = new Button(name,
                    onClick: () =>
                    {
                        ModContent.GetInstance<EditorSystem>().state.editorPanel.CancelDrag();
                        LayoutHelper.ApplyLayout(name);
                        LayoutHelper.CurrentLayoutName = name;
                        LayoutHelper.SaveLastLayout();
                        Populate();
                    },
                    tooltip: () => isCurrent ? "Current layout" : "",
                    maxWidth: true
                )
                {
                    Left = { Pixels = 0 },
                    Top = { Pixels = y },
                    BackgroundColor = isCurrent ? Color.Green : UICommon.DefaultUIBlue
                };
                //btn.BorderColor = isCurrent ? Color.Yellow : Color.Black; // doesnt work?
                btn.OnRightClick += (_, _) =>
                {
                    FileHelper.OpenLayoutFile(name);
                };
                content.Append(btn);
                //Gap(4);
                y += h + pad;
            }
        }

        private void BuildOptionsContent(UIElement content)
        {
            const float h = 30f, pad = 4f;
            float y = 0;
            var actions = new (string text, string tooltip, Action act)[]
            {
        ("Open layout folder", "Open the layouts folder", () =>
        {
            FileHelper.OpenLayoutFolder();
        }),
        ("Save this layout", "Create a new layout file", () =>
        {
                FileHelper.CreateAndOpenNewLayoutFile("MyCustomLayout");
                Populate();
        }),
        ("Remove all layouts", "Delete all layouts", () =>
        {
                FileHelper.DeleteAllLayouts();
                Populate();
        })
            };
            foreach (var (txt, tip, act) in actions)
            {
                var btn = new Button(txt,
                    onClick: act,
                    tooltip: () => tip,
                    maxWidth: true
                )
                {
                    Left = { Pixels = 0 },
                    Top = { Pixels = y }
                };
                content.Append(btn);
                y += h + pad;
            }
        }
    }
}
