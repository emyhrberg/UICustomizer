using Terraria;
using Terraria.GameContent.UI.Elements;

namespace UICustomizer.UI
{
    public class CloseButton : UIPanel
    {
        public CloseButton()
        {
            Width.Set(30, 0);
            Height.Set(30, 0);
            HAlign = 1f;
            VAlign = 0f;
            SetPadding(0);

            UIText x = new("X", 0.45f, true)
            {
                HAlign = 0.5f,
                VAlign = 0.5f
            };
            Append(x);

            OnMouseOver += (_, _) =>
            {
                BorderColor = Color.Yellow;
                if (Main.mouseLeft)
                {
                     //x._color = Color.Yellow;
                    if (Main.mouseLeftRelease)
                    {
                        x._color = Color.White;
                        // UICustomizerSystem.ExitEditMode();
                    }
                }
            };
            OnMouseOut += (_, _) => BorderColor = Color.Black;
        }
    }
}
