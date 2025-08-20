using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;

namespace UICustomizer.UI.MainMenuElements.Sections
{
    public class BaseSection : UIPanel
    {
        public BaseSection()
        {
            Width.Set(-28, 1);
            HAlign = 0.5f;
            Left.Set(12, 0);
        }
    }
}
