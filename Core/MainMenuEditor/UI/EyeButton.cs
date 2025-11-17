using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using UIEditor.Core.Helpers;

namespace UIEditor.Core.MainMenuEditor.UI
{
    public class EyeButton : UIImage
    {
        private bool isOn = false;
        public EyeButton(Asset<Texture2D> tex, Action action) : base(texture: tex)
        {
            ImageScale = 0.62f;
            HAlign = 1f;
            Left.Set(-3, 0);
            Top.Set(0, 0);

            OnLeftClick += (_, _) => action?.Invoke();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (IsMouseHovering)
            {
                if (_texture == Ass.Inventory_Tick_On) isOn = true;

                string showOrHide = isOn ? "Hide" : "Show";
                UICommon.TooltipMouseText(showOrHide);
            }
        }
    }
}
