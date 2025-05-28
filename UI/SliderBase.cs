namespace UICustomizer.UI
{
    public enum SliderUsageLevel
    {
        NotSelected,
        SelectedAndLocked,
        OtherElementIsLocked
    }

    public class SliderBase : UIElement
    {
        protected static UIElement CurrentLockedSlider;
        protected static UIElement CurrentAimedSlider;

        /// <summary>
        /// Checks if any slider is currently locked.
        /// Used to determine if the UI should react to slider interactions.
        /// </summary>
        public static bool IsAnySliderLocked => CurrentLockedSlider != null;

        protected SliderUsageLevel UsageLevel
        {
            get
            {
                if (CurrentLockedSlider == this)
                {
                    return SliderUsageLevel.SelectedAndLocked;
                }
                else if (CurrentLockedSlider != null)
                {
                    return SliderUsageLevel.OtherElementIsLocked;
                }
                else
                {
                    return SliderUsageLevel.NotSelected;
                }
            }
        }

        public static void EscapeElements()
        {
            CurrentLockedSlider = null;
            CurrentAimedSlider = null;
        }
    }
}