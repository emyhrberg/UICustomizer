using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;

namespace UIEditor.Core.MainMenuEditor.UI.Sections
{
    public class BaseSection : UIPanel
    {
        public BaseSection()
        {
            Width.Set(-28, 1);
            HAlign = 0.5f;
            Left.Set(12, 0);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
