using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using UICustomizer.Common.Systems;

namespace UICustomizer.UI
{
    public class EditButton : UIPanel
    {
        public bool Active = false;
        public UIText saveText;

        internal EditButton()
        {
            // Panel size and position
            Width.Set(100, 0);
            Height.Set(35, 0);
            VAlign = 0.4f;
            HAlign = 0.95f;

            // Add UIText in the middle
            saveText = new("Edit UI", 0.45f, true);
            saveText.HAlign = 0.5f;
            Append(saveText);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            if (!Active) return;

            UICustomizerSystem.EnterEditMode();
        }

        public override void Update(GameTime gameTime)
        {
            if (!Active) return;

            //VAlign = 0.4f;

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active) return;

            //if (UICustomizerSystem.EditModeActive) return;

            base.Draw(spriteBatch);

            if (IsMouseHovering)
            {
                UICommon.TooltipMouseText("Click to edit some UI");
            }
        }
    }
}
