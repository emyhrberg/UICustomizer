using System;
using Terraria.GameContent.UI.Elements;

namespace UICustomizer.UI.MainMenuElements
{
    internal class OnOffTextButton : UIText
    {
        public bool isOn = true;
        public OnOffTextButton(string text, float textScale = 0.85f, bool initiallyOn = true, bool large = false) : base(text, textScale, large)
        {
            if (!initiallyOn) isOn = false;

            string onOff = isOn ? "On" : "Off";

            SetText(text + onOff);

            HAlign = 0.5f;
            TextColor = Color.Gray;

            OnMouseOver += (_,_) => {
                TextColor = Color.White;
            };

            OnMouseOut += (_, _) => {
                TextColor = Color.Gray;
            };

            OnLeftClick += (_, _) =>
            {
                isOn = !isOn;

                string onOff = isOn ? "On" : "Off";
                SetText(text + onOff);
            };
        }
    }
}
