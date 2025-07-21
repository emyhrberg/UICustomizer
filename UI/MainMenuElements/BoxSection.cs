using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;

namespace UICustomizer.UI.MainMenuElements
{
    internal class BoxSection : UIPanel
    {
        public BoxSection()
        {
            Width.Set(-28, 1);
            Height.Set(130, 0);
            HAlign = 0.5f;
            PaddingTop = 4f;
            Left.Set(12, 0);
            OverflowHidden = false;
        }

        public override void Update(GameTime gameTime)
        {
            Width.Set(-28, 1);
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
