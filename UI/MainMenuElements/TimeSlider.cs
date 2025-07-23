namespace UICustomizer.UI.MainMenuElements
{
    internal class TimeSlider : ZenSlider
    {
        public TimeSlider()
        {
            Width.Set(0, 1);
            Top.Set(24+24, 0);
            Ratio = WorldTimeHelper.GetRatioFromTime();
            InnerTexture = Ass.SliderTime;

            void refresh(float v)
            {
                WorldTimeHelper.SetTime(v);
                Ratio = WorldTimeHelper.GetRatioFromTime();
            }

            OnDrag += refresh;
            OnValueAppliedOnMouseUp += refresh;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
