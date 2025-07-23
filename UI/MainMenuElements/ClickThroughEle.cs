using Terraria.UI;

namespace UICustomizer.UI.MainMenuElements
{
    public class ClickThroughElement : UIElement
    {
        public override bool ContainsPoint(Vector2 point)
        {
            foreach (var element in Elements)
            {
                if (element.ContainsPoint(point))
                    return true;
            }
            return false;
        }
    }

}
