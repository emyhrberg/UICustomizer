using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;
using UICustomizer.Common.Systems;

namespace UICustomizer.UI
{
    public enum CheckboxState { Unchecked, Checked }
    public class Checkbox : UIElement
    {
        public CheckboxState state = CheckboxState.Checked;
        public CheckboxBox box;
        private UIText label;
        private string hoverText;

        public Checkbox(string text, string hover)
        {
            hoverText = hover;
            Height.Set(22, 0);
            Width.Set(80, 0);
            //MaxWidth.Set(20, 1);
            Left.Set(0, 0);

            // Box
            box = new CheckboxBox(Ass.Check);
            box.Left.Set(0, 0);
            box.Top.Set(0, 0);
            Append(box);

            // Label
            label = new UIText(text, 0.4f, true);
            label.Left.Set(30, 0);
            label.Top.Set(5, 0);
            Append(label);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            Toggle();
        }

        private void Toggle()
        {
            if (!UICustomizerSystem.EditModeActive) return;
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
            //Left.Set(-10, 0);
            //Width.Set(20, 1);
            //MaxWidth.Set(20, 1);
            //label.Top.Set(5, 0);

            base.Draw(spriteBatch);
            if (IsMouseHovering)
            {
                Log.SlowInfo("hover");
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (IsMouseHovering)
            {
                //Log.Info("22");
            }
        }
    }
}
