using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;

namespace UICustomizer.MainMenu.UI
{
    public class PlayPause : UIColoredImageButton
    {
        public bool isPaused = true; // default

        public PlayPause() : base(Ass.Pause, true)
        {
            HAlign = 1f;
            Width.Set(22, 0);
            Height.Set(22, 0);
            Top.Set(3, 0);

            SetImage(Ass.Pause);

            OnLeftClick += (_, _) =>
            {
                isPaused = !isPaused;
                SetImage(isPaused ? Ass.Pause : Ass.Play);
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
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
    }
}
