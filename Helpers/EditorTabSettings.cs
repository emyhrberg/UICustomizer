namespace UICustomizer.Helpers
{
    public static class EditorTabSettings
    {
        // Toggles
        public static bool SnapToEdges { get; set; } = true;
        public static bool ShowHitboxes { get; set; } = true;
        public static bool ShowNames { get; set; } = true;
        public static bool ShowEyeToggle { get; set; } = true;
        public static bool FitBounds { get; set; } = true;

        // Settings
        public static float Opacity { get; set; } = 0.25f; // fill opacity of hitboxes
        public static int Stroke { get; set; } = 5; // not currently a slider, but could be in the future
        public static int SnapThreshold { get; set; } = 20; //  currently a slider
    }

}
