using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using UICustomizer.Common.Systems;
using UICustomizer.UI.Tabs;

namespace UICustomizer.UI
{
    public sealed class Panel : DraggablePanel
    {
        // Tabs
        public Tab current;
        public EditorTab editorTab;
        public LayoutsTab layoutsTab;
        public InterfaceLayersTab layersTab;

        // Content
        public int W = 300;
        public int H = 400;
        public UIPanel headerElement;
        public Resize resize;
        public UIElement body = new();   // the content region
        private readonly UIScrollbar scrollbar;

        public Panel()
        {
            // Size and position
            Width.Set(W, 0);
            Height.Set(H, 0);
            HAlign = 1.0f;
            VAlign = 1.0f;
            Left.Set(-20, 0);
            Top.Set(-20, 0);

            // Header element
            headerElement = new UIPanel
            {
                Top = { Pixels = -12 },
                Width = { Percent = 1f, Pixels = 24 },
                Height = { Pixels = 30 },
                Left = { Pixels = -12 },
                MaxWidth = { Percent = 1f, Pixels = 24 },
            };
            headerElement.SetPadding(0);
            Append(headerElement);

            // Scrollbar
            scrollbar = new UIScrollbar();
            scrollbar.Width.Set(20, 0);
            scrollbar.Height.Set(-30 - 12, 1);
            scrollbar.Left.Set(-12, 1);
            scrollbar.Top.Set(6, 0);
            body.Append(scrollbar);

            // Tabs
            float tabW = (W - 30f) / W / 3f;          // W is the panel pixel width

            editorTab = new EditorTab(Select, scrollbar) { Width = { Percent = tabW } };
            layersTab = new InterfaceLayersTab(Select, scrollbar) { Width = { Percent = tabW }, Left = { Percent = tabW } };
            layoutsTab = new LayoutsTab(Select) { Width = { Percent = tabW }, Left = { Percent = tabW * 2 } };

            headerElement.Append(editorTab);
            headerElement.Append(layersTab);
            headerElement.Append(layoutsTab);

            // X
            headerElement.Append(new CloseButton("X"));

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
                if (!UICustomizerSystem.EditModeActive) return;
                CancelDrag();

                float oldHeight = Height.Pixels;
                float newHeight = oldHeight + dy;
                float maxHeight = 180f;

                // Clamp max height
                if (newHeight > 1000f || newHeight < maxHeight)
                {
                    return;
                }

                // Resize: Set new heights!
                Height.Set(newHeight, 0f);
                // list.Height.Set(newHeight - 35, 0f);

                // Set new top offsets
                float topOffset = newHeight - oldHeight;
                Top.Pixels += topOffset;

                Recalculate();
            };
            Append(resize);

            Select(editorTab); // default tab
        }

        private void Select(Tab t)
        {
            if (!UICustomizerSystem.EditModeActive) return;

            if (current == t) return;
            current = t;

            // Highlight color
            Tab[] tabs = [editorTab, layoutsTab, layersTab];
            foreach (var tab in tabs)
                tab.header.TextColor = tab == t ? Color.Yellow : Color.White;

            body.RemoveAllChildren();

            // Append everything
            body.Append(t.list);
            body.Append(scrollbar);
            if (scrollbar != null)
                t.list.SetScrollbar(scrollbar);
            t.list.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            if (!UICustomizerSystem.EditModeActive) return;

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!UICustomizerSystem.EditModeActive) return;

            base.Draw(spriteBatch);
        }
    }
}
