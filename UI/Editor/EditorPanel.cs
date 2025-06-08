using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using UICustomizer.Common.Systems;
using UICustomizer.Helpers;
using static UICustomizer.Helpers.DrawHelper;
using static UICustomizer.Helpers.Layouts.ElementHelper;

namespace UICustomizer.UI.Editor
{
    public class EditorPanel : BasePanel
    {
        // Tabs
        public EditorTab editorTab;
        public PositionsTab positionsTab;
        public LayoutsTab layoutsTab;

        protected override Action CloseAction => EditorSystem.SetActiveFalse;

        protected override (Tab, Tab, Tab) CreateTabs()
        {
            editorTab = new EditorTab();
            PopulateDefaultColors();
            positionsTab = new PositionsTab();
            layoutsTab = new LayoutsTab();
            return (editorTab, positionsTab, layoutsTab);
        }

        // Store colors for each UI element
        private readonly Dictionary<Element, Color> elementColors = [];

        public void PopulateDefaultColors()
        {
            elementColors.Clear();
            foreach (Element ele in Enum.GetValues<Element>())
            {
                elementColors[ele] = ele switch
                {
                    Element.Map => Color.Black,
                    Element.InfoAccs => Color.Red,
                    Element.Chat => Color.Blue,
                    Element.Inventory => Color.Blue,
                    Element.Crafting => Color.Yellow,
                    Element.Accessories => Color.Magenta,
                    Element.Hotbar => Color.Cyan,
                    Element.Buffs => Color.Purple,
                    Element.CraftingWindow => Color.OrangeRed,
                    Element.ClassicLife => Color.Pink,
                    Element.ClassicMana => Color.Teal,
                    Element.FancyLife => Color.Lime,
                    Element.FancyLifeText => Color.GreenYellow,
                    Element.FancyMana => Color.SkyBlue,
                    Element.HorizontalBars => Color.Gold,
                    Element.BarLifeText => Color.Silver,
                    Element.BarManaText => Color.Brown,
                    _ => Color.Red // default to red if element isnt found
                };
            }
        }

        public void PopulateRandomColors()
        {
            elementColors.Clear();
            foreach (Element ele in Enum.GetValues<Element>())
                elementColors[ele] = new Color(Main.rand.Next(40, 256), Main.rand.Next(40, 256), Main.rand.Next(40, 256));
        }

        public override void Draw(SpriteBatch sb)
        {
            if (EditorSystem.IsActive)
            {
                base.Draw(sb);
                DrawHitboxes(sb);
            }
        }

        private void DrawHitboxes(SpriteBatch sb)
        {
            // Draw hover around every element
            DrawHitboxOutlineAndText(sb, DragSystem.InfoAccsBounds(), Element.InfoAccs.ToString(), x: -70, color: elementColors[Element.InfoAccs]);

            if (Main.drawingPlayerChat)
                DrawHitboxOutlineAndText(sb, DragSystem.ChatBounds(), Element.Map.ToString(), color: elementColors[Element.Map]);

            // Draw hotbar or inventory
            if (Main.playerInventory)
            {
                DrawHitboxOutlineAndText(sb, DragSystem.InventoryBounds(), Element.Inventory.ToString(), x: -75, color: elementColors[Element.Inventory]);
                DrawHitboxOutlineAndText(sb, DragSystem.CraftingBounds(), Element.Crafting.ToString(), x: -70, color: elementColors[Element.Crafting]);
                DrawHitboxOutlineAndText(sb, DragSystem.AccessoriesBounds(), Element.Accessories.ToString(), x: -90, color: elementColors[Element.Accessories]);
            }
            else
            {
                DrawHitboxOutlineAndText(sb, DragSystem.HotbarBounds(), Element.Hotbar.ToString(), x: -55, color: elementColors[Element.Hotbar]);
                DrawHitboxOutlineAndText(sb, DragSystem.BuffBounds(), Element.Buffs.ToString(), x: -45, color: elementColors[Element.Buffs]);
            }

            if (Main.recBigList)
                DrawHitboxOutlineAndText(sb, DragSystem.CraftingWindowBounds(), Element.CraftingWindow.ToString(), x: -125, color: elementColors[Element.CraftingWindow]);

            // Draw resource bars. Check which health and mana style is active:
            string activeSetName = Main.ResourceSetsManager.ActiveSet.DisplayedName;
            if (activeSetName.StartsWith("Classic"))
            {
                DrawHitboxOutlineAndText(sb, DragSystem.ClassicLifeBounds(), Element.ClassicLife.ToString(), x: -90, color: elementColors[Element.ClassicLife]);
                DrawHitboxOutlineAndText(sb, DragSystem.ClassicManaBounds(), Element.ClassicMana.ToString(), x: -5, color: elementColors[Element.ClassicMana]);
            }
            else if (activeSetName == "Fancy")
            {
                DrawHitboxOutlineAndText(sb, DragSystem.FancyLifeBounds(), Element.FancyLife.ToString(), x: -80, color: elementColors[Element.FancyLife]);
                DrawHitboxOutlineAndText(sb, DragSystem.FancyManaBounds(), Element.FancyMana.ToString(), x: -5, color: elementColors[Element.FancyMana]);
            }
            else if (activeSetName == "Fancy 2")
            {
                DrawHitboxOutlineAndText(sb, DragSystem.FancyLifeBounds(), Element.FancyLife.ToString(), x: -80, color: elementColors[Element.FancyLife]);
                DrawHitboxOutlineAndText(sb, DragSystem.FancyLifeTextBounds(), Element.FancyLifeText.ToString(), x: -112, color: elementColors[Element.FancyLifeText]);
                DrawHitboxOutlineAndText(sb, DragSystem.FancyManaBounds(), Element.FancyMana.ToString(), x: -5, color: elementColors[Element.FancyMana]);
            }
            else if (activeSetName == "Bars")
            {
                DrawHitboxOutlineAndText(sb, DragSystem.BarsBounds(), Element.HorizontalBars.ToString(), x: -120, color: elementColors[Element.HorizontalBars]);
            }
            else if (activeSetName == "Bars 2")
            {
                DrawHitboxOutlineAndText(sb, DragSystem.BarsBounds(), Element.HorizontalBars.ToString(), x: -120, color: elementColors[Element.HorizontalBars]);
                DrawHitboxOutlineAndText(sb, DragSystem.BarLifeTextBounds(), Element.BarLifeText.ToString(), x: -95, color: elementColors[Element.BarLifeText]);
            }
            else if (activeSetName == "Bars 3")
            {
                DrawHitboxOutlineAndText(sb, DragSystem.BarsBounds(), Element.HorizontalBars.ToString(), x: -120, color: elementColors[Element.HorizontalBars]);
                DrawHitboxOutlineAndText(sb, DragSystem.BarLifeTextBounds(), Element.BarLifeText.ToString(), x: -95, color: elementColors[Element.BarLifeText]);
                DrawHitboxOutlineAndText(sb, DragSystem.BarManaTextBounds(), Element.BarManaText.ToString(), x: -110, color: elementColors[Element.BarManaText]);
            }

            DrawHitboxOutlineAndText(sb, DragSystem.MapBounds(), Element.Map.ToString(), x: -40, color: elementColors[Element.Map]);
        }
    }
}
