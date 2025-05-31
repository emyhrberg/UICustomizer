using System;
using Terraria.GameContent.UI.Elements;
using UICustomizer.UI.Tabs;

namespace UICustomizer.UI
{
    public sealed class Panel : DraggablePanel
    {
        // Tabs
        private Tab current;
        public EditorTab editorTab;
        public LayoutsTab layoutsTab;
        public InterfaceLayersTab layersTab;

        // Content
        private UIElement headerElement;
        public Resize resize;
        private readonly UIElement body = new();   // the “page” region

        public Panel()
        {
            // Size and position
            Width.Set(300, 0);
            Height.Set(400, 0);
            Left.Set(Main.screenWidth - 315, 0);
            Top.Set(Main.screenHeight - 415, 0);

            // Header element
            headerElement = new UIElement
            {
                Width = { Percent = 1f },
                Height = { Pixels = 24 },
            };
            Append(headerElement);

            // Tabs
            const float w = 1f / 3f;
            editorTab = new EditorTab() { Width = { Percent = w } };
            layersTab = new InterfaceLayersTab() { Width = { Percent = w }, Left = { Percent = w } };
            layoutsTab = new LayoutsTab() { Width = { Percent = w }, Left = { Percent = 2 * w } };

            editorTab.OnSelect = () => Select(editorTab);
            layersTab.OnSelect = () => Select(layersTab);
            layoutsTab.OnSelect = () => Select(layoutsTab);

            headerElement.Append(editorTab);
            headerElement.Append(layersTab);
            headerElement.Append(layoutsTab);

            // Body
            body.Top.Set(24, 0);
            body.Width.Set(0, 1f);
            body.Height.Set(-24, 1f);
            Append(body);

            // Resize
            resize = new Resize(Ass.Resize)
            {
                HAlign = 1f,
                VAlign = 1f
            };
            resize.OnDragY += dy =>
            {
                CancelDrag();                       // stop the panel drag
                float newH = Math.Clamp(Height.Pixels + dy, 80f, 1000f);
                Height.Set(newH, 0);
                Recalculate();
            };
            Append(resize);

            Select(editorTab); // default tab
        }

        private void Select(Tab t)
        {
            if (current == t) return;
            current = t;

            body.RemoveAllChildren();
            body.Append(t.list);
        }
    }
}
