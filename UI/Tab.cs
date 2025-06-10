using System;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using UICustomizer.Helpers;

namespace UICustomizer.UI
{
    public abstract class Tab : UIPanel
    {
        public readonly UIList list = new UIList();
        private Action<Tab> selectCallback;
        public UIText header;
        private Scrollbar scrollbar;

        // hover fade fields
        private bool _isHovering;
        private float _hoverTimer;
        private static readonly float HoverFadeTime = 0.2f;

        protected Tab(string text)
        {
            Height.Set(40, 0);
            OverflowHidden = true;
            BackgroundColor = ColorHelper.CalamityRed;
            SetPadding(2);

            // Hover tracking
            OnMouseOver += (_, _) => _isHovering = true;
            OnMouseOut += (_, _) => _isHovering = false;

            // Text
            header = new UIText(text, 0.46f, true) { HAlign = .5f, VAlign = .5f };
            Append(header);

            // List setup
            list.Width.Set(0, 1f);
            list.Height.Set(-30 + 12, 1f);
            list.ListPadding = 4f;
            list.ManualSortMethod = _ => { };

            OnLeftClick += (_, _) => selectCallback?.Invoke(this);
        }

        internal void SetSelectCallback(Action<Tab> callback) => selectCallback = callback;
        internal void SetScrollbar(Scrollbar sb)
        {
            scrollbar = sb;
            list.SetScrollbar(scrollbar);
        }

        public abstract void Populate();

        protected void Gap(float px)
        {
            var spacer = new UIElement();
            spacer.Height.Set(px, 0);
            list.Add(spacer);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_isHovering)
                _hoverTimer = Math.Min(HoverFadeTime, _hoverTimer + dt);
            else
                _hoverTimer = Math.Max(0f, _hoverTimer - dt);

            float t = _hoverTimer / HoverFadeTime;
            BorderColor = Color.Lerp(Color.Black, Color.Yellow, t);
        }
    }

}
