using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using UICustomizer.Common.Systems.Hooks;
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

            bool AnyModWithUIToolAdded = ModLoader.TryGetMod("DragonLens", out _) || ModLoader.TryGetMod("ModReloader", out _);

            if (!AnyModWithUIToolAdded)
            {
                // If we have mods that add their own edit button, we don't need to add our own.
                // This is because they will handle the edit mode toggle.
                if (!ModLoader.TryGetMod("DragonLens", out _) && !ModLoader.TryGetMod("ModReloader", out _))
                {
                    // The text next to settings open when inventory is closed
                    EditButton editButton = new();
                    Append(editButton);
                }
            }
        }

        // Draw black background with lower opacity to show we're in edit mode.
        // TODO: Exclude all UI that can be moved from this black drawing. Maybe with draw interface layers?
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            // --- HOT RELOAD TESTING ---
            int hidden = Main.LocalPlayer.hideInfo.Cast<bool>().Count(b => !b);
            int active = InfoDisplayLoader.ActiveDisplays();
            //Log.Info("test");
            //Log.ChatSlow("test", 3000);


            if (!UICustomizerSystem.EditModeActive) return;

            // Draw hover around every element
            DrawHitboxOutlineAndText(sb, DragSystem.MapBounds(), "Map", textPos: TextPosition.Left, x: 20);
            DrawHitboxOutlineAndText(sb, DragSystem.InfoAccsBounds(), "Info\nAccs", textPos: TextPosition.Left, x: 20);

            if (Main.drawingPlayerChat)
                DrawHitboxOutlineAndText(sb, DragSystem.ChatBounds(), "Chat", textPos: TextPosition.Top);

            // Draw hotbar or inventory
            if (Main.playerInventory)
            {
                DrawHitboxOutlineAndText(sb, DragSystem.InventoryBounds(), "Inventory", textPos: TextPosition.Left, x: 20);
                DrawHitboxOutlineAndText(sb, DragSystem.CraftingBounds(), "Crafting", textPos: TextPosition.Right, x: 20);
                DrawHitboxOutlineAndText(sb, DragSystem.AccessoriesBounds(), "Accessories", textPos: TextPosition.Top, x: 20);
            }
            else
            {
                DrawHitboxOutlineAndText(sb, DragSystem.HotbarBounds(), "Hotbar", textPos: TextPosition.Right, x: 20);
                DrawHitboxOutlineAndText(sb, DragSystem.BuffBounds(), "Buffs", textPos: TextPosition.Right);
            }

            if (Main.recBigList)
            {
                DrawHitboxOutlineAndText(sb, DragSystem.CraftWindowBounds(), "BigRecList", textPos: TextPosition.Top);
            }

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