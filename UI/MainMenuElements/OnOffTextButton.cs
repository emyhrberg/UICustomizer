using System;
using Terraria.GameContent.UI.Elements;

namespace UICustomizer.UI.MainMenuElements
{
    internal class OnOffTextButton : UIText
    {
        public bool isOn = true;
        public OnOffTextButton(string text, bool initiallyOn = true, bool NoOnOff = false) : base(text, textScale: 0.82f, large: false)
        {
            if (!initiallyOn) isOn = false;

            string onOff = isOn ? "On" : "Off";

            if (NoOnOff) SetText(text);
            else SetText(text + onOff);

            HAlign = 0.5f;
            TextColor = Color.Gray;

            OnMouseOver += (_, _) =>
            {
                TextColor = Color.White;
            };

            OnMouseOut += (_, _) =>
            {
                TextColor = Color.Gray;
            };

            OnLeftClick += (_, _) =>
            {
                if (NoOnOff) return;
                isOn = !isOn;

                string onOff = isOn ? "On" : "Off";
                SetText(text + onOff);
            };
        }
    }
}
