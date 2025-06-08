using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using UICustomizer.Common.Systems;
using UICustomizer.Helpers;
using UICustomizer.UI.Editor;

namespace UICustomizer.UI
{
    public abstract class BasePanel : DraggablePanel
    {
        private readonly List<Tab> _allTabs = [];
        private Tab _currentTab;
        public Scrollbar _scrollbar; // public to force disable it for some tabs
        private readonly UIPanel _header;
        private readonly UIElement _body;
        private readonly Resize _resize;

        protected BasePanel()
        {
            // 1) panel sizing
            Left.Set(-40, 0f);
            Top.Set(-60, 0f);
            Width.Set(300, 0f);
            Height.Set(440, 0f);
            VAlign = 1f;
            HAlign = 1f;
            BackgroundColor = ColorHelper.SuperDarkBluePanel;
            SetPadding(0);

            // 2) full‐width header
            _header = new UIPanel
            {
                Top = { Pixels = 0 },
                Left = { Pixels = 0 },
                Width = { Percent = 1f },
                Height = { Pixels = 30 }
            };
            _header.SetPadding(0);
            Append(_header);

            // 3) shared scrollbar
            _scrollbar = new Scrollbar();

            // 4) create exactly three tabs
            var (t1, t2, t3) = CreateTabs();
            AddTab(t1);
            AddTab(t2);
            AddTab(t3);

            // 5) lay them out _plus_ the close button as four equal columns
            LayoutHeader();

            // 6) body region under the header
            _body = new UIElement
            {
                Top = { Pixels = 30 },
                Width = { Percent = 1f },
                Height = { Percent = 1f, Pixels = -30 }
            };
            Append(_body);

            // 7) resize handle
            _resize = new Resize(Ass.Resize) { HAlign = 1f, VAlign = 1f };
            _resize.OnDragY += dy =>
            {
                if (!EditorSystem.IsActive) return;
                CancelDrag();

                float oldHeight = Height.Pixels;
                float newHeight = oldHeight + dy;

                // Clamp max and min height
                if (newHeight > 1000f || newHeight < 180f)
                {
                    return;
                }

                // Resize: Set new height and top!
                Height.Set(newHeight, 0f);
                float topOffset = newHeight - oldHeight;
                Top.Pixels += topOffset;

                Recalculate();
            };
            Append(_resize);

            // 8) select first tab
            if (_allTabs.Count > 0) Select(_allTabs[0]);
        }

        /// <summary>
        /// Turn your 3 tabs into columns 0,1,2 and your Close button into column 3.
        /// </summary>
        private void LayoutHeader()
        {
            // Force recalculation to get proper dimensions
            Recalculate();

            // Get the actual header width
            float headerWidth = _header.GetDimensions().Width;
            float closeButtonWidth = 30f;
            float availableWidth = headerWidth - closeButtonWidth;
            float tabWidth = availableWidth / _allTabs.Count;

            // Place each tab
            for (int i = 0; i < _allTabs.Count; i++)
            {
                var tab = _allTabs[i];
                tab.Left.Set(tabWidth * i, 0f);
                tab.Top.Set(0, 0f);
                tab.Width.Set(tabWidth, 0f);
                tab.Height.Set(30, 0f);
                _header.Append(tab);
            }

            // Place the close button at the right edge
            var close = new CloseButton();
            close.Height.Set(30, 0);
            close.OnLeftClick += (_, _) => CloseAction();
            _header.Append(close);
        }

        private void AddTab(Tab tab)
        {
            tab.SetSelectCallback(Select);
            tab.SetScrollbar(_scrollbar);
            _allTabs.Add(tab);
        }

        private void Select(Tab t)
        {
            if (_currentTab == t) return;
            _currentTab = t;
            foreach (var tab in _allTabs)
                tab.header.TextColor = tab == t ? Color.Yellow : Color.White;

            _body.RemoveAllChildren();
            t.list.SetScrollbar(_scrollbar);
            _body.Append(t.list);
            _body.Append(_scrollbar);
            t.Populate();
        }

        protected abstract (Tab, Tab, Tab) CreateTabs();
        protected abstract Action CloseAction { get; }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_currentTab is EditorTab editorTab)
            {
                _scrollbar.Visible = false;

                if (editorTab.hideAllMode)
                    return;

            }

            //else if (_currentTab is PositionsTab)
            //    _scrollbar.Visible = false;
            //else
            _scrollbar.Visible = false; // DISABLE FOR ALL


            base.Draw(spriteBatch);
        }
    }
}
