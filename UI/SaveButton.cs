using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using UICustomizer.Common.Configs;
using UICustomizer.Common.Systems;
using UICustomizer.Common.Systems.Hooks;

namespace UICustomizer.UI
{
    public class SaveButton : UIPanel
    {
        public bool Active = false;
        public UIText saveText;

        internal SaveButton()
        {
            // Panel size and position
            Width.Set(100, 0);
            Height.Set(35, 0);
            VAlign = 0.45f;
            HAlign = 0.95f;

            // Add UIText in the middle
            saveText = new("Save UI", 0.45f, true);
            saveText.HAlign = 0.5f;
            Append(saveText);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            if (!Active) return;

            // Save.
            Conf.C.ChatOffsetX = ChatHook.OffsetX;
            Conf.C.ChatOffsetY = ChatHook.OffsetY;
            Conf.C.HotbarOffsetX = HotbarHook.OffsetX;
            Conf.C.HotbarOffsetY = HotbarHook.OffsetY;
            Conf.Save();

            // Exit.
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
                UICommon.TooltipMouseText("Click to save the new UI");
            }
        }
    }
}
