using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace UICustomizer.UI
{
    /// <summary>
    /// Scrollbar that can be disabled.
    /// (Not successfully implemented yet!)
    /// </summary>
    public class Scrollbar : UIScrollbar
    {
        public bool Visible { get; set; } = true;

        // This didnt work properly, we need to re-implement by checking list contents instead if we want a functioning
        // scrollbar that hides itself when content is short enough not to need a scrollbar.
        //private bool IsScrollbarAtBottom => _viewPosition >= _maxViewSize - _viewSize;
        private bool IsScrollbarAtBottom => false;

        public Scrollbar()
        {
            int w = 20;
            Width.Set(w, 0);
            Height.Set(-30 - 12, 1);
            Left.Set(-12 - w / 2, 1);
            Top.Set(6, 0);
        }

        public override void Update(GameTime gameTime)
        {
            if (Visible && !IsScrollbarAtBottom)
            {
                base.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Visible && !IsScrollbarAtBottom)
            {
                base.Draw(spriteBatch);
            }
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            if (Visible && !IsScrollbarAtBottom)
            {
                base.LeftMouseDown(evt);
            }
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            if (Visible && !IsScrollbarAtBottom)
            {
                base.LeftMouseUp(evt);
            }
        }

        public override void RightMouseDown(UIMouseEvent evt)
        {
            if (Visible && !IsScrollbarAtBottom)
            {
                base.LeftMouseUp(evt);
            }
        }
    }
}
