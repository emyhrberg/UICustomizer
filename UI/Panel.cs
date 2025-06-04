using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        public int H = 420;
        public UIPanel headerElement;
        public Resize resize;
        public UIElement body = new();   // the content region
        private readonly Scrollbar scrollbar;

        public Panel()
        {
            SetDefaultSizeAndPosition();
            UICustomizerSystem.EnterEditMode();

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
            float tabW = (W - 30f) / W / 3f; // -30 for close button, 3f for three tabs

            editorTab = new(Select, scrollbar) { Width = { Percent = tabW } };
            layersTab = new(Select, scrollbar) { Width = { Percent = tabW }, Left = { Percent = tabW } };
            themesTab = new(Select, scrollbar) { Width = { Percent = tabW }, Left = { Percent = tabW * 2 } };

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

            // THIS WILL OPEN PANEL ON LAUNCH!
            // Select(editorTab); // default tab
            if (current != null)
            {
                Select(current);
            }
        }

        public void SetDefaultSizeAndPosition()
        {
            // Set default size and position
            Left.Set(-40, 0f);
            Top.Set(-60, 0f);
            Width.Set(W, 0f);
            Height.Set(H, 0f);
            VAlign = 1.0f;
            HAlign = 1.0f;
            Recalculate();
        }

        public void SelectPublic(Tab t) => Select(t);
        private void Select(Tab t)
        {
            if (!UICustomizerSystem.EditModeActive) return;
            if (current == t) return;

            // Select new tab
            current = t;

            // Highlight color
            Tab[] tabs = [editorTab, themesTab, layersTab];
            foreach (var tab in tabs)
                tab.header.TextColor = tab == t ? Color.Yellow : Color.White;

            // Update body content
            body.RemoveAllChildren();
            body.Append(t.list);
            body.Append(scrollbar);
            t.list.SetScrollbar(scrollbar);
            t.list.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            if (!UICustomizerSystem.EditModeActive) return;

            base.Update(gameTime);

            // UpdateScrollbarVisbility();
        }

        private void UpdateScrollbarVisbility()
        {
            // Set scrollbar visibility
            if (current?.list != null)
            {
                // Default to false
                scrollbar.Visible = false;

                // Count the number of expanded sections
                int expandedCount = 0;

                if (current is ThemeTab)
                {
                    // Theme tab has no sections to count
                    scrollbar.Visible = true;
                    expandedCount = 0;
                }
                else if (current is EditorTab editor)
                {
                    if (editor._uiEditorExpanded) expandedCount++;
                    if (editor._layoutsExpanded) expandedCount++;
                    if (editor._optionsExpanded) expandedCount++;
                    expandedCount++;
                }
                else if (current is LayersTab layers)
                {
                    if (layers.vanillaExpanded) expandedCount++;
                    if (expandedCount == 1 && layers.modsExpandedMap.Values.All(v => !v))
                    {
                        scrollbar.Visible = true;
                        return;
                    }
                    expandedCount += layers.modsExpandedMap.Values.Count(v => v);
                }

                // Show scrollbar only if more than one section is expanded
                if (expandedCount > 1)
                {
                    scrollbar.Visible = true;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!UICustomizerSystem.EditModeActive) return;

            // Don't draw the panel at all when hiding everything
            if (current is EditorTab editorTab && editorTab.hideAllMode)
            {
                // If press escape, exit hide mode
                // NOTE: This causes collection enumeration issues
                // if (Main.keyState.IsKeyDown(Keys.Escape))
                // {
                //     UICustomizerSystem.ExitEditMode();
                //     editorTab.hideAllMode = false;
                //     editorTab.ResetHideAllState();
                //     editorTab.PopulatePublic();
                // }

                return; // Panel is completely invisible, floating button handled by UIState
            }

            // Normal drawing when not hiding
            base.Draw(spriteBatch);
        }
    }
}
