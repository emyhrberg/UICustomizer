using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;

namespace UICustomizer.UI
{
    public enum CheckboxState { Unchecked, Checked }
    public class Checkbox : UIElement
    {
        public CheckboxState state = CheckboxState.Checked;
        public CheckboxBox box;
        private UIText label;
        private string hoverText;
        private Action onClick;

        public Checkbox(string text, string hover, Action onClick = null)
        {
            hoverText = hover;
            this.onClick = onClick;

            Height.Set(16, 0);
            Width.Set(400, 0);
            //MaxWidth.Set(20, 1);
            Left.Set(0, 0);

            // Box
            box = new CheckboxBox(Ass.Check);
            box.Left.Set(0, 0);
            box.Top.Set(0, 0);
            Append(box);

            // Label
            label = new UIText(text, 0.34f, true);
            label.Left.Set(30, 0);
            label.Top.Set(5, 0);
            Append(label);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            Toggle();
            onClick?.Invoke();
        }

        private void Toggle()
        {
            if (state == CheckboxState.Checked)
            {
                state = CheckboxState.Unchecked;
                box.SetImage(Ass.Uncheck);
            }
            else
            {
                state = CheckboxState.Checked;
                box.SetImage(Ass.Check);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (IsMouseHovering && hoverText != "")
            {
                UICommon.TooltipMouseText(hoverText);
            }
        }
    }

    public class CheckboxBox : UIImageButton
    {
        public CheckboxBox(Asset<Texture2D> texture) : base(texture)
        {
            Width.Set(22, 0);
            Height.Set(22, 0);
            MaxWidth.Set(22, 0);
            MaxHeight.Set(22, 0);
        }

        public override void Draw(SpriteBatch sb)
        {
            // Skip base draw and draw custom position and opacity
            //base.Draw(spriteBatch);

            if (Parent is Checkbox checkbox)
            {
                //DrawHelper.DrawProperScale(spriteBatch, checkbox, _texture.Value);
                float opacity = checkbox.IsMouseHovering ? 1f : 0.4f;
                Vector2 pos = checkbox.GetDimensions().Position();
                sb.Draw(_texture.Value, pos, Color.White * opacity);
            }
        }
    }
}
