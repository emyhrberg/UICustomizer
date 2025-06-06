using System;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using static UICustomizer.UI.CollapsibleHeader;

namespace UICustomizer.UI
{
    public abstract class Tab : UIPanel
    {
        public readonly UIList list = [];
        private readonly Action<Tab> selectCallback;
        public UIText header;
        public Scrollbar scrollbar;

        protected Tab(string text, Action<Tab> select, Scrollbar scrollbar)
        {
            Height.Set(30, 0);
            BackgroundColor = ColorHelper.DarkBluePanel;
            SetPadding(0);

            this.scrollbar = scrollbar;
            this.selectCallback = select;

            // Hover
            OnMouseOver += (_, _) => BorderColor = Color.Yellow;
            OnMouseOut += (_, _) => BorderColor = Color.Black;

            // Text
            header = new UIText(text, 0.5f, true) { HAlign = .5f, VAlign = .5f };
            Append(header);

            // List
            list = new UIList
            {
                Width = { Percent = 1f },
                Height = { Percent = 1f, Pixels = -30 + 12 },
                ListPadding = 4f,
                ManualSortMethod = (e) => { }
            };
            if (scrollbar != null)
            {
                list.SetScrollbar(scrollbar);
            }

            // Wire click
            OnLeftClick += (_, _) =>
            {
                selectCallback?.Invoke(this);
            };
            Populate();
        }

        public abstract void Populate();

        protected CollapsibleHeader AddCollapsibleHeader(string text, Func<bool> getState, Action<bool> setState, Action onToggle = null)
        {
            Gap(4);
            bool currentState = getState(); // Get the current state
            var header = new CollapsibleHeader(
                text: text,
                initialState: currentState ? CollapseState.Expanded : CollapseState.Collapsed, // Pass current state
                onClick: () =>
                {
                    setState(!currentState); // Toggle the state
                    onToggle?.Invoke();
                    Populate(); // Refresh to show/hide content
                },
                hoverText: () => currentState ? $"Click to collapse {text.ToLower()}" : $"Click to expand {text.ToLower()}"
            );
            TryAdd(header);
            Gap(12);
            return header;
        }

        protected void TryAdd(UIElement element)
        {
            if (!list.Contains(element))
            {
                list.Add(element);
            }
        }

        protected void Gap(float px)
        {
            var spacer = new UIElement();
            spacer.Height.Set(px, 0);
            list.Add(spacer);
        }
    }
}
