using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using UICustomizer.Core.MainMenuEditor.Helpers;
using UICustomizer.Core.MainMenuEditor.Hooks;

namespace UICustomizer.Core.MainMenuEditor.UI
{
    public class PlayPause : UIColoredImageButton
    {
        public bool isPaused = true; // default

        public PlayPause() : base(Ass.Pause, true)
        {
            HAlign = 1f;
            Width.Set(22, 0);
            Height.Set(22, 0);
            Top.Set(0, 0);
            Left.Set(6, 0);

            SetImage(Ass.Pause);

            OnLeftClick += (_, _) =>
            {
                isPaused = !isPaused;
                MainMenuPauseSystem.IsPaused = !MainMenuPauseSystem.IsPaused;
                SetImage(isPaused ? Ass.Pause : Ass.Play);
                Conf.C.MainMenuTime.IsPaused = !isPaused;
                Conf.C.MainMenuTime.Time = (float)WorldTimeHelper.ConvertToTotalTime();
                Conf.Save();
            };
        }

        public override void Update(GameTime gameTime)
        {
            Left.Set(6, 0);
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
