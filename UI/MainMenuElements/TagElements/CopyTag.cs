using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.OS;
using Terraria;
using Terraria.GameContent.UI.Elements;
using UICustomizer.Common.Systems.Hooks.MainMenu;

namespace UICustomizer.UI.MainMenuElements.TagElements
{
    public class CopyTag : UIColoredImageButton
    {
        public CopyTag(Asset<Texture2D> texture, bool isSmall = true) : base(texture, isSmall)
        {
            VAlign = 1f;
            Left.Set(0, 0);

            OnLeftMouseDown += (_, __) =>
            {
                Color c = MainMenuTextColorHook.MainMenuTextColor;
                // to #RRGGBB
                string hex = $"{c.R:X2}{c.G:X2}{c.B:X2}";
                Platform.Get<IClipboard>().Value = hex;
            };
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            //Log.Info(Main.MouseScreen.ToScreenPosition().ToString());
        }
    }
}