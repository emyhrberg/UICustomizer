using Microsoft.Xna.Framework.Graphics;
using UICustomizer.UI;
using static UICustomizer.Helpers.DrawHelper;

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
            DrawHitboxOutlineAndText(sb, DragSystem.HotbarBounds(), "Hotbar", textPos: TextPosition.Right);
            DrawHitboxOutlineAndText(sb, DragSystem.BuffBounds(), "Buffs", textPos: TextPosition.Right);
            DrawHitboxOutlineAndText(sb, DragSystem.MapBounds(), "Map", textPos: TextPosition.Left, x: 20);
            DrawHitboxOutlineAndText(sb, DragSystem.InfoAccsBounds(), "Info\nAccessories", textPos: TextPosition.Left, x: 20);
            DrawHitboxOutlineAndText(sb, DragSystem.ChatBounds(), "Chat", textPos: TextPosition.Top);

            // Draw resource bars. Check which health and mana style is active:
            string activeSetName = Main.ResourceSetsManager.ActiveSet.DisplayedName;
            if (activeSetName.StartsWith("Classic"))
            {
                DrawHitboxOutlineAndText(sb, DragSystem.ClassicLifeBounds(), "Classic\n Life", textPos: TextPosition.Left);
                DrawHitboxOutlineAndText(sb, DragSystem.ClassicManaBounds(), "Classic\n Mana", textPos: TextPosition.Bottom);
            }
            else if (activeSetName == "Fancy 2")
            {
                DrawHitboxOutlineAndText(sb, DragSystem.FancyLifeBounds(), "Fancy \nLife 2", textPos: TextPosition.Left);
                DrawHitboxOutlineAndText(sb, DragSystem.FancyManaBounds(), "Fancy \nMana 2", textPos: TextPosition.Bottom, x: -5);
            }
            else if (activeSetName == "Fancy")
            {
                DrawHitboxOutlineAndText(sb, DragSystem.FancyLifeBounds(), "Fancy \nLife 1", textPos: TextPosition.Left);
                DrawHitboxOutlineAndText(sb, DragSystem.FancyManaBounds(), "Fancy \nMana 1", textPos: TextPosition.Bottom, x: -5);
            }
            else if (activeSetName == "Bars")
            {
                DrawHitboxOutlineAndText(sb, DragSystem.BarsBounds(), activeSetName, textPos: TextPosition.Left);
            }
            else if (activeSetName == "Bars 2")
            {
                DrawHitboxOutlineAndText(sb, DragSystem.BarsBounds(), activeSetName, textPos: TextPosition.Left);
                DrawHitboxOutlineAndText(sb, DragSystem.BarLifeTextBounds(), "Life", textPos: TextPosition.Right, x: 20);
            }
            else if (activeSetName == "Bars 3")
            {
                DrawHitboxOutlineAndText(sb, DragSystem.BarsBounds(), activeSetName, textPos: TextPosition.Left);
                DrawHitboxOutlineAndText(sb, DragSystem.BarLifeTextBounds(), "Life", textPos: TextPosition.Right, x: 20);
                DrawHitboxOutlineAndText(sb, DragSystem.BarManaTextBounds(), "Mana", textPos: TextPosition.Right, x: 20);
            }
        }
    }
}