using Terraria.GameContent.UI.States;
using UICustomizer.Common.Systems.Hooks.MainMenu;

namespace UICustomizer.UI.MainMenuElements
{
    internal class TimeSlider : ZenSlider
    {
        public TimeSlider()
        {
            Width.Set(-10, 1);
            Top.Set(24, 0);
            Ratio = WorldTimeHelper.GetRatioFromTime();

            void refresh()
            {
                // timeText.SetText(WorldTimeHelper.GetFormattedTime());
                Ratio = WorldTimeHelper.GetRatioFromTime();
                // pauseToggle.SetText($"Pause: {(MainMenuPauseSystem.TimeIsPausedBySlider ? "On" : "Off")}");
            }

            OnDrag += v =>
            {
                WorldTimeHelper.SetTime(v);
                refresh();
            };
            OnValueAppliedOnMouseUp += v =>
            {
                WorldTimeHelper.SetTime(v);
                refresh();
            };
        }

        public override void Update(GameTime gameTime)
        {
            InnerTexture = Ass.SliderGradient;
            base.Update(gameTime);
        }
    }
}
