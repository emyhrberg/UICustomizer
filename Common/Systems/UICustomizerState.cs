using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using UICustomizer.Common.Configs;
using UICustomizer.Common.Systems.Hooks;
using UICustomizer.UI;

namespace UICustomizer.Common.Systems
{
    public class UICustomizerState : UIState
    {
        public UIEditorPanel editorPanel;
        public UICustomizerState()
        {
            // The entire panel that contains all the UI elements for editing.
            editorPanel = new();
            editorPanel.Width.Set(200,0);
            editorPanel.Left.Set(200, 0);
            editorPanel.Top.Set(200, 0);
            editorPanel.Height.Set(200, 0);

            Append(editorPanel);

            // The text next to settings open when inventory is open
            EditButton editButton = new();
            Append(editButton);
        }

        // Draw black background with lower opacity to show we're in edit mode.
        // TODO: Exclude all UI that can be moved from this black drawing. Maybe with draw interface layers?
        public override void Draw(SpriteBatch sb)
        {
            if (UICustomizerSystem.EditModeActive)
            {
                // Draw black hover effect
                Texture2D tex = Terraria.GameContent.TextureAssets.MagicPixel.Value;
                sb.Draw(tex, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), null, Color.Black * 0.65f);

                // Draw hover around every element
                DrawHelper.DrawHitboxOutlineAndText(sb, DragHelper.ChatBounds(), "Chat");
                DrawHelper.DrawHitboxOutlineAndText(sb, DragHelper.HotbarBounds(), "Hotbar");
                DrawHelper.DrawHitboxOutlineAndText(sb, DragHelper.MapBounds(), "Map");
                DrawHelper.DrawHitboxOutlineAndText(sb, DragHelper.InfoAccsBounds(), "InfoAccs");
            }

            base.Draw(sb);
        }
    }
}