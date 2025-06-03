using System.Collections.Generic;
using static UICustomizer.Helpers.ResourceThemeHelper;

namespace UICustomizer.Helpers.Layouts
{
    public class LayoutData
    {
        public Dictionary<string, Vector2> Positions { get; set; } = new();
        public ResourceTheme Theme { get; set; } = ResourceTheme.Classic;
    }
}