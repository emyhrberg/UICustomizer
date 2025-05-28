using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using UICustomizer.Common.Systems;
using UICustomizer.Common.Systems.Hooks;

namespace UICustomizer.UI
{
    public class ResetButton : UIPanel
    {
        public bool Active = false;
        public UIText saveText;

        internal ResetButton()
        {
            // Panel size and position
            Width.Set(100, 0);
            Height.Set(35, 0);
            VAlign = 0.5f;
            HAlign = 0.95f;

            // Add UIText in the middle
            saveText = new("Reset UI", 0.45f, true);
            saveText.HAlign = 0.5f;
            Append(saveText);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            // Reset all hooks.
            ChatHook.OffsetX = 0;
            ChatHook.OffsetY = 0;
            HotbarHook.OffsetX = 0;
            HotbarHook.OffsetY = 0;

            // Cancel edit mode.
            UICustomizerSystem.ExitEditMode();
        }

        public override void Update(GameTime gameTime)
        {
            if (!Active) return;

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active) return;

            if (!UICustomizerSystem.EditModeActive) return;

            base.Draw(spriteBatch);

            if (IsMouseHovering)
            {
                UICommon.TooltipMouseText("Click to cancel without saving");
            }
        }
    }
}
