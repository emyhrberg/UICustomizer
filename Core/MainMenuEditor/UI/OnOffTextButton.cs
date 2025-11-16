using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;

namespace UIEditor.Core.MainMenuEditor.UI
{
    internal class OnOffTextButton : UIText
    {
        public bool isOn = true;

        private bool _showOnHover;

        public OnOffTextButton(string text, bool initiallyOn = true, bool NoOnOff = false, bool ShowOnHover = false)
    : base("", textScale: 0.82f, large: false)
        {
            isOn = initiallyOn;
            _showOnHover = ShowOnHover;

            HAlign = 0.5f;
            if (!ShowOnHover) TextColor = Color.Gray;

            UpdateLabelText(text, NoOnOff);

            OnMouseOver += (_, _) =>
            {
                if (!ShowOnHover) TextColor = Color.White;
            };

            OnMouseOut += (_, _) =>
            {
                if (!ShowOnHover) TextColor = Color.Gray;
            };

            OnLeftClick += (_, _) =>
            {
                if (NoOnOff || ShowOnHover) return;
                isOn = !isOn;
                UpdateLabelText(text, NoOnOff);
            };
        }

        private void UpdateLabelText(string baseText, bool NoOnOff)
        {
            string onOff = isOn ? "On" : "Off";
            SetText(NoOnOff ? baseText : baseText + onOff);
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
