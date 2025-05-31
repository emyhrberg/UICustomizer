using Terraria.GameContent.UI.Elements;
using UICustomizer.Common.Systems;

namespace UICustomizer.UI
{
    public class CloseButton : UIPanel
    {
        public CloseButton(string text = "X")
        {
            Width.Set(30, 0);
            Height.Set(30, 0);
            HAlign = 1f;
            VAlign = 0f;
            SetPadding(0);
            Append(new UIText(text, 0.45f, true) { HAlign = .5f, VAlign = .5f });
            OnMouseOver += (_, _) => BorderColor = Color.Yellow;
            OnMouseOut += (_, _) => BorderColor = Color.Black;
            OnLeftClick += (_, _) => UICustomizerSystem.ExitEditMode();
        }
    }
}