using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;

namespace UICustomizer.UI
{
    public class EyeLayerToggleElement : UIImage
    {
        public bool isChecked = false;

        public EyeLayerToggleElement(Asset<Texture2D> texture) : base(texture)
        {
            OnLeftClick += (_, _) =>
            {
                Toggle();
            };
        }

        public void Toggle()
        {
            isChecked = !isChecked;
            SetImage(isChecked ? Ass.EyeOpen : Ass.EyeClosed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
