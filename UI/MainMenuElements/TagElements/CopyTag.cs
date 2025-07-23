using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.OS;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using UICustomizer.Common.Systems.Hooks.MainMenu;
using static System.Net.Mime.MediaTypeNames;
using static UICustomizer.Common.Systems.MainMenu.MainMenuState;

namespace UICustomizer.UI.MainMenuElements.TagElements
{
    public class CopyTag : UIColoredImageButton
    {
        public CopyTag(Asset<Texture2D> texture, TextColorType type, bool isSmall = true) : base(texture, isSmall)
        {
            VAlign = 1f;
            Left.Set(0, 0);

            OnLeftMouseDown += (_, __) =>
            {
                Color c = type switch
                {
                    TextColorType.Fill => MainMenuTextColorHook.NormalColor,
                    TextColorType.Outline => MainMenuOutlineTextColorHook.Color,
                    TextColorType.Hover => MainMenuTextColorHook.HoverColor,
                    _ => MainMenuTextColorHook.NormalColor
                };

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

            if (IsMouseHovering)
            {
                UICommon.TooltipMouseText("Copy color");
            }
        }
    }
}