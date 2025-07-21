using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using UICustomizer.Common.Systems.Hooks.MainMenu;

namespace UICustomizer.UI.MainMenuElements.TagElements
{
    public class RandomizeTag : UIColoredImageButton
    {
        public RandomizeTag(Asset<Texture2D> texture, bool isSmall = true) : base(texture, isSmall)
        {
            VAlign = 1f;
            Left.Set(80, 0);

            OnLeftMouseDown += (_, __) =>
            {
                var c = new Color(Main.rand.Next(256), Main.rand.Next(256), Main.rand.Next(256));
                MainMenuTextColorHook.MainMenuTextColor = c;
            };
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
    }
}