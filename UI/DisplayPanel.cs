using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using UICustomizer.Common.Systems;
using UICustomizer.Common.Systems.Hooks;

namespace UICustomizer.UI
{
    public class DisplayPanel : UIPanel
    {
        public bool Active = false;
        public UIText displayText;

        internal DisplayPanel()
        {
            // Panel size and position
            Width.Set(200, 0);
            Height.Set(200, 0);
            VAlign = 0.95f;
            HAlign = 0.9f;

            // Add UIText in the middle
            displayText = new("", 0.30f, true);
            displayText.HAlign = 0.0f;
            Append(displayText);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            //base.LeftClick(evt);
            //UICustomizerSystem.EnterEditMode();
        }

        public override void Update(GameTime gameTime)
        {
            if (!Active) return;

            string text = "";
            text += $"Chat Hover: {DragSystem.MouseInBounds(DragSystem.ChatBounds())}\n";
            text += $"Hotbar Hover: {DragSystem.MouseInBounds(DragSystem.HotbarBounds())}\n";
            text += $"ChatX: {(int)ChatHook.OffsetX}\n";
            text += $"ChatY: {(int)ChatHook.OffsetY}\n";
            text += $"HotbarX: {(int)HotbarHook.OffsetX}\n";
            text += $"HotbarY: {(int)HotbarHook.OffsetY}\n";

            displayText.SetText(text);

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active) return;

            //if (UICustomizerSystem.EditModeActive) return;

            base.Draw(spriteBatch);
        }
    }
}
