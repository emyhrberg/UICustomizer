using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UICustomizer.Helpers
{
    public static class UIEditorSettings
    {
        public static bool SnapToEdges { get; set; } = true;
        public static bool ShowHitboxes { get; set; } = true;
        public static bool ShowNames { get; set; } = true;
        public static bool Fit { get; set; } = true;
        public static float Darkness { get; set; } = 0;
        public static float Opacity { get; set; } = 0.25f;
        public static int Stroke { get; set; } = 5;
    }

}
