using UICustomizer.Common.Systems.Hooks.MainMenu;

namespace UICustomizer.UI.MainMenuElements
{
    internal class TimeSpeedSlider : ZenSlider
    {
        private float Min = -20f;
        private float Max = 20f;

        public TimeSpeedSlider()
        {
            Width.Set(0, 1);
            Top.Set(94, 0);
            Ratio = ColorHelper.InverseLerp(Min, Max, TimeSpeedHook.MenuTimeSpeed);
            InnerTexture = Ass.SliderGradient;

            void refresh(float v)
            {
                // Map Ratio (0..1) to desired speed range, e.g. 0.1x to 5x
                float speed = MathHelper.Lerp(Min, Max, v);
                TimeSpeedHook.MenuTimeSpeed = speed;
            }

            OnDrag += refresh;
            OnValueAppliedOnMouseUp += refresh;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Top.Set(94, 0);
        }
    }
}
