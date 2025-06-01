using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace UICustomizer.UI
{
    /// <summary>
    /// Scrollbar that can be disabled.
    /// </summary>
    public class Scrollbar : UIScrollbar
    {
        /// <summary>
        /// When false, this scrollbar does not draw or process mouse input.
        /// </summary>
        public bool Visible { get; set; } = true;

        public Scrollbar()
        {
            Width.Set(20, 0);
            Height.Set(-30 - 12, 1);
            Left.Set(-12, 1);
            Top.Set(6, 0);
        }

        public override void Update(GameTime gameTime) { if (Visible) base.Update(gameTime); }
        public override void Draw(SpriteBatch spriteBatch) { if (Visible) base.DrawSelf(spriteBatch); }
        public override void LeftMouseDown(UIMouseEvent evt) { if (Visible) base.LeftMouseDown(evt); }
        public override void LeftMouseUp(UIMouseEvent evt) { if (Visible) base.LeftMouseUp(evt); }
        public override void RightMouseDown(UIMouseEvent evt) { if (Visible) base.RightMouseDown(evt); }
    }
}
