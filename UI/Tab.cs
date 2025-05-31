using System;
using Terraria.GameContent.UI.Elements;

namespace UICustomizer.UI
{
    // 1) Tabs DO NOT contain their list visually – we don’t append it there.
    public abstract class Tab : UIPanel
    {
        public readonly UIList list = [];
        public Action OnSelect { get; set; }

        protected Tab(string text)
        {
            Height.Set(24, 0);
            BackgroundColor = ColorHelper.DarkBluePanel;
            SetPadding(0);

            // Text
            UIText textElement = new UIText(text, 0.55f, true) { HAlign = .5f, VAlign = .5f };
            Append(textElement);

            // List
            list = new UIList
            {
                Width = { Percent = 1f },
                Height = { Percent = 1f },
                ListPadding = 4f,
                ManualSortMethod = (e) => { }
            };
            Populate();
        }

        protected abstract void Populate();

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
