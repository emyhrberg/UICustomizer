using Microsoft.Xna.Framework.Graphics;
using UICustomizer.UI;

namespace UICustomizer.Common.Systems
{
    public class UICustomizerState : UIState
    {
        public UIEditorPanel editorPanel;
        public LayerTogglePanel layerPanel;
        public LayoutsPanel layoutsPanel;
        public UICustomizerState()
        {
            // The entire panel that contains all the UI elements for editing.
            editorPanel = new();
            Append(editorPanel);

            layerPanel = new();
            Append(layerPanel);

            layoutsPanel = new();
            Append(layoutsPanel);

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
                //DrawHelper.DrawHitboxOutlineAndText(sb, DragHelper.ResourceBarBounds(), "Resources");
            }

            base.Draw(sb);
        }
    }
}