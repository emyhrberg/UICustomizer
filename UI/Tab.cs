using System;
using System.Linq;
using Terraria.GameContent.UI.Elements;

namespace UICustomizer.UI
{
    public abstract class Tab : UIPanel
    {
        public readonly UIList list = [];
        public Action OnSelect { get; set; }
        public UIText header;
        public UIScrollbar scrollbar;

        protected Tab(string text, Action<Tab> select, UIScrollbar scrollbar = null)
        {
            Height.Set(30, 0);
            BackgroundColor = ColorHelper.DarkBluePanel;
            SetPadding(0);

            this.scrollbar = scrollbar;

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
            OnLeftClick += (_, _) => select(this);

            Populate();
        }

        protected abstract void Populate();

        protected void TryAdd(UIElement element)
        {
            if (!list.Contains(element))
            {
                list.Add(element);
            }
        }
        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            OnSelect?.Invoke();
        }

        protected void Gap(float px = 4f)
        {
            var spacer = new UIElement();
            spacer.Height.Set(px, 0);
            list.Add(spacer);
        }
    }
}
