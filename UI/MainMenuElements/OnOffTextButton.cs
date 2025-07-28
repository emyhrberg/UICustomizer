using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;

namespace UICustomizer.UI.MainMenuElements
{
    internal class OnOffTextButton : UIText
    {
        public bool isOn = true;

        private bool _showOnHover;
        public OnOffTextButton(string text, bool initiallyOn = true, bool NoOnOff = false, bool ShowOnHover=false) : base(text, textScale: 0.82f, large: false)
        {
            this.isOn = initiallyOn;
            this._showOnHover = ShowOnHover;
            if (!initiallyOn) isOn = false;

            string onOff = isOn ? "On" : "Off";

            if (NoOnOff) SetText(text);
            else SetText(text + onOff);

            HAlign = 0.5f;

            if (!ShowOnHover)
            {
                TextColor = Color.Gray;
            }

            OnMouseOver += (_, _) =>
            {
                if (!ShowOnHover) TextColor = Color.White;
            };

            OnMouseOut += (_, _) =>
            {
                if (!ShowOnHover)  TextColor = Color.Gray;
            };

            OnLeftClick += (_, _) =>
            {
                if (NoOnOff || ShowOnHover) return;
                isOn = !isOn;

                string onOff = isOn ? "On" : "Off";
                SetText(text + onOff);
            };
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (_showOnHover && IsMouseHovering && Text != "No file chosen")
            {
                //DrawHelper.DrawTextAtMouse(spriteBatch, Text);
                UICommon.TooltipMouseText(Text);
            }
        }
    }
}
