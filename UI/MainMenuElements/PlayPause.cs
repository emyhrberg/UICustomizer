using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;
using UICustomizer.Common.Systems.MainMenu;

namespace UICustomizer.UI.MainMenuElements
{
    public class PlayPause : UIColoredImageButton
    {
        private Asset<Texture2D> playTex;
        private Asset<Texture2D> pauseTex;

        public PlayPause(bool isSmall = true) : base(Ass.Pause, isSmall)
        {
            playTex = Ass.Play;
            pauseTex = Ass.Pause;

            HAlign = 1f;
            Width.Set(22, 0);
            Height.Set(22, 0);
            Top.Set(3, 0);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //UpdateIcon();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (IsMouseHovering)
            {
                string tooltip = MainMenuPauseSystem.IsPaused ? "Play" : "Pause";
                UICommon.TooltipMouseText(tooltip);
            }
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            MainMenuPauseSystem.IsPaused = !MainMenuPauseSystem.IsPaused;
        }
    }

}