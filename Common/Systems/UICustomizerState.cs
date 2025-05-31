using Microsoft.Xna.Framework.Graphics;
using UICustomizer.UI;

namespace UICustomizer.Common.Systems
{
    public class UICustomizerState : UIState
    {
        public Panel panel;
        public UICustomizerState()
        {
            // The entire panel that contains all the UI elements for editing.
            panel = new();
            Append(panel);

            // The text next to settings open when inventory is open
            EditButton editButton = new();
            Append(editButton);
        }

        // Draw black background with lower opacity to show we're in edit mode.
        // TODO: Exclude all UI that can be moved from this black drawing. Maybe with draw interface layers?
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (!UICustomizerSystem.EditModeActive) return;

            // Draw hover around every element
            DrawHelper.DrawHitboxOutlineAndText(sb, DragHelper.ChatBounds(), "Chat");
            DrawHelper.DrawHitboxOutlineAndText(sb, DragHelper.HotbarBounds(), "Hotbar");
            DrawHelper.DrawHitboxOutlineAndText(sb, DragHelper.MapBounds(), "Map");
            DrawHelper.DrawHitboxOutlineAndText(sb, DragHelper.InfoAccsBounds(), "InfoAccs");

            // Draw resource bars. Check which health and mana style is active:
            string activeSetName = Main.ResourceSetsManager.ActiveSet.DisplayedName;
            if (activeSetName.StartsWith("Classic"))
            {
                DrawHelper.DrawHitboxOutlineAndText(sb, DragHelper.ClassicLifeBounds(), "Classic Life");
                DrawHelper.DrawHitboxOutlineAndText(sb, DragHelper.ClassicManaBounds(), "Classic Mana");
            }
            else if (activeSetName.StartsWith("Fancy"))
            {
                DrawHelper.DrawHitboxOutlineAndText(sb, DragHelper.FancyLifeBounds(), "Fancy Life");
                DrawHelper.DrawHitboxOutlineAndText(sb, DragHelper.FancyManaBounds(), "Fancy Mana");
            }
            else if (activeSetName.StartsWith("Bars"))
            {
                DrawHelper.DrawHitboxOutlineAndText(sb, DragHelper.BarsBounds(), "Bars");
            }
        }
    }
}