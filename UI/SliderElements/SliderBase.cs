using Terraria.UI;

namespace UICustomizer.UI.SliderElements
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

        // This bool is useful for disabling dragging while dragging other elements simultaneously.
        public static bool IsAnySliderLocked => CurrentLockedSlider != null;

        protected SliderUsageLevel UsageLevel =>
            CurrentLockedSlider == this ? SliderUsageLevel.SelectedAndLocked :
            CurrentLockedSlider != null ? SliderUsageLevel.OtherElementIsLocked :
            SliderUsageLevel.NotSelected;

        public static void EscapeElements()
        {
            CurrentLockedSlider = null;
            CurrentAimedSlider = null;
        }
    }
}