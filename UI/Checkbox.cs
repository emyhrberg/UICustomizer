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
        public bool Active = true;

        public Checkbox(string text, string hover, int width = 50, Action onClick = null, CheckboxState initialState = CheckboxState.Checked)
            : base()
        {
            state = initialState;
            hoverText = hover;
            this.onClick = onClick;

            Height.Set(16, 0);
            Width.Set(width, 0);
            //MaxWidth.Set(20, 1);
            Left.Set(0, 0);

            // Box
            box = new CheckboxBox(Ass.Check);
            box.SetImage(initialState == CheckboxState.Checked ? Ass.Check : Ass.Uncheck);
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
            if (!Active) return;

            base.LeftClick(evt);

            Toggle();
            onClick?.Invoke();
        }

        public void SetState(CheckboxState newState)
        {
            state = newState;
            box.SetImage(newState == CheckboxState.Checked ? Ass.Check : Ass.Uncheck);
        }

        private void Toggle()
        {
            if (state == CheckboxState.Checked)
            {
                SetState(CheckboxState.Unchecked);
            }
            else
            {
                SetState(CheckboxState.Checked);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active) return;

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
            // Check if parent checkbox is active before drawing
            if (Parent is Checkbox checkbox && !checkbox.Active)
            {
                return; // Don't draw if checkbox is not active
            }

            // Skip base draw and draw custom position and opacity
            float opacity = Parent is Checkbox cb && cb.IsMouseHovering ? 1f : 0.4f;
            Vector2 pos = Parent.GetDimensions().Position();
            sb.Draw(_texture.Value, pos, Color.White * opacity);
        }
    }
}
