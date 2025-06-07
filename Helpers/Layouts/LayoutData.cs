using System.Collections.Generic;
using Newtonsoft.Json.Converters;
using static UICustomizer.Helpers.Layouts.MapThemeHelper;
using static UICustomizer.Helpers.Layouts.OffsetHelper;
using static UICustomizer.Helpers.Layouts.ResourceThemeHelper;

namespace UICustomizer.Helpers.Layouts
{
    public class LayoutData
    {
        /// <summary>
        /// Holds all the positions (offsets) of UI elements.
        /// </summary>
        public Dictionary<Offset, Vector2> Offsets { get; set; } = [];

        /// <summary>
        /// The life and mana theme for the layout. Uses StringEnumConverter to ensure string representation in JSON as opposed to integer values.
        /// NOTE: Make sure to use Newtonsoft, NOT System.Text.Json.
        /// </summary>
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public ResourceTheme ResourceTheme { get; set; } = ResourceTheme.Classic;

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public MapTheme MapTheme { get; set; } = MapTheme.Default;
    }
}
