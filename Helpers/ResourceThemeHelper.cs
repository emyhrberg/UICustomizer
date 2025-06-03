namespace UICustomizer.Helpers
{
    public static class ResourceThemeHelper
    {
        public enum ResourceTheme
        {
            Classic,
            Fancy1,
            Fancy2,
            Bars,
            Bars2,
            Bars3
        }

        public static void SetResourceTheme(ResourceTheme theme)
        {
            Main.ResourceSetsManager.SetActiveFrameFromIndex(theme switch
            {
                ResourceTheme.Classic => 0,
                ResourceTheme.Fancy1 => 1,
                ResourceTheme.Fancy2 => 2,
                ResourceTheme.Bars => 3,
                ResourceTheme.Bars2 => 4,
                ResourceTheme.Bars3 => 5,
                _ => 0
            });
        }

        public static void GetActiveTheme(out ResourceTheme theme)
        {
            int activeIndex = Main.ResourceSetsManager.ActiveSet.DisplayedName switch
            {
                "Classic" => 0,
                "Fancy 1" => 1,
                "Fancy 2" => 2,
                "Bars" => 3,
                "Bars 2" => 4,
                "Bars 3" => 5,
                _ => 0
            };
            theme = activeIndex switch
            {
                0 => ResourceTheme.Classic,
                1 => ResourceTheme.Fancy1,
                2 => ResourceTheme.Fancy2,
                3 => ResourceTheme.Bars,
                4 => ResourceTheme.Bars2,
                5 => ResourceTheme.Bars3,
                _ => ResourceTheme.Classic // Default to Classic if index is out of range
            };
        }
    }
}