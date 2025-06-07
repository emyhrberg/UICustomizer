namespace UICustomizer.Helpers.Layouts
{
    public static class ResourceThemeHelper
    {
        public enum ResourceTheme
        {
            Fancy,
            Fancy2,
            Bars,
            Bars2,
            Bars3,
            Classic,
        }

        public static void SetResourceTheme(ResourceTheme theme)
        {
            Main.ResourceSetsManager.SetActiveFrameFromIndex(theme switch
            {
                ResourceTheme.Fancy => 0,
                ResourceTheme.Fancy2 => 1,
                ResourceTheme.Bars => 2,
                ResourceTheme.Bars2 => 3,
                ResourceTheme.Bars3 => 4,
                ResourceTheme.Classic => 5,
                _ => 0
            });
        }

        public static void GetActiveResourceTheme(out ResourceTheme theme)
        {
            int activeIndex = Main.ResourceSetsManager.ActiveSet.DisplayedName switch
            {
                "Fancy" => 0,
                "Fancy2" => 1,
                "Bars" => 2,
                "Bars2" => 3,
                "Bars3" => 4,
                "Classic" => 5,
                _ => 0
            };
            theme = activeIndex switch
            {
                0 => ResourceTheme.Fancy,
                1 => ResourceTheme.Fancy2,
                2 => ResourceTheme.Bars,
                3 => ResourceTheme.Bars2,
                4 => ResourceTheme.Bars3,
                5 => ResourceTheme.Classic,
                _ => ResourceTheme.Classic
            };
        }
    }
}