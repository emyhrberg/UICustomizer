using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;

namespace UICustomizer.UI.EditMode
{
    internal class EditModeLabel : UIText
    {
        public EditModeLabel(string text, float textScale = 1, bool large = false) : base(text, textScale, large)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
