using System;
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
        public ThemeTab themesTab;
        public LayersTab layersTab;

        // Content
        public int W = 300;
        public int H = 400;
        public UIPanel headerElement;
        public Resize resize;
        public UIElement body = new();   // the content region
        private readonly Scrollbar scrollbar;

        public Panel()
        {
            SetDefaultSizeAndPosition();

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
            scrollbar = new();
            body.Append(scrollbar);

            // Tabs
            float tabW = (W - 30f) / W / 3f;          // W is the panel pixel width

            editorTab = new EditorTab(Select, scrollbar) { Width = { Percent = tabW } };
            layersTab = new LayersTab(Select, scrollbar) { Width = { Percent = tabW }, Left = { Percent = tabW } };
            themesTab = new ThemeTab(Select, scrollbar) { Width = { Percent = tabW }, Left = { Percent = tabW * 2 } };

            headerElement.Append(editorTab);
            headerElement.Append(layersTab);
            headerElement.Append(themesTab);

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

        public void SetDefaultSizeAndPosition()
        {
            // Set default size and position
            Left.Set(-40, 0f);
            Top.Set(-40, 0f);
            Width.Set(W, 0f);
            Height.Set(H, 0f);
            VAlign = 1.0f;
            HAlign = 1.0f;
            Recalculate();
        }

        private void Select(Tab t)
        {
            if (!UICustomizerSystem.EditModeActive) return;

            if (current == t) return;
            current = t;

            // Highlight color
            Tab[] tabs = [editorTab, themesTab, layersTab];
            foreach (var tab in tabs)
                tab.header.TextColor = tab == t ? Color.Yellow : Color.White;

            body.RemoveAllChildren();

            // Append everything
            body.Append(t.list);
            body.Append(scrollbar);
            t.list.SetScrollbar(scrollbar);
            t.list.Recalculate();

            // Show scrollbar only if content is taller than view
            float contentHeight = 0f;
            foreach (UIElement child in t.list.Children)
            {
                float childTop = child.Top.Pixels;
                float childHeight = child.GetDimensions().Height;
                contentHeight = Math.Max(contentHeight, childTop + childHeight);
            }
            float viewHeight = t.list.GetInnerDimensions().Height;
            scrollbar.Visible = contentHeight > viewHeight;

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
