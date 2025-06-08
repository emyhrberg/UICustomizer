using System;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using UICustomizer.Helpers;

namespace UICustomizer.UI
{
    public abstract class Tab : UIPanel
    {
        public readonly UIList list = []; // contains all elements. wired to scrollbar.
        private Action<Tab> selectCallback;
        public UIText header;
        private Scrollbar scrollbar;

        protected Tab(string text)
        {
            Height.Set(40, 0);
            //BackgroundColor = ColorHelper.DarkBluePanel;
            SetPadding(2);

            // Hover
            OnMouseOver += (_, _) => BorderColor = Color.Yellow;
            OnMouseOut += (_, _) => BorderColor = Color.Black;

            // Text
            header = new UIText(text, 0.46f, true) { HAlign = .5f, VAlign = .5f };
            Append(header);

            // List setup
            list.Width.Set(0, 1f);
            list.Height.Set(-30 + 12, 1f);
            list.ListPadding = 4f;
            list.ManualSortMethod = _ => { };

            // Wire click
            OnLeftClick += (_, _) => selectCallback?.Invoke(this);
        }

        internal void SetSelectCallback(Action<Tab> callback) => selectCallback = callback;
        internal void SetScrollbar(Scrollbar sb)
        {
            scrollbar = sb;
            list.SetScrollbar(scrollbar);
        }

        public abstract void Populate();

        protected void Add(UIElement element)
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
