using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using UICustomizer.Common.Systems;
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

        protected override Action CloseAction => () => EditorSystem.SetActive(false);

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
            base.Draw(sb);
            DrawHitboxes(sb);
        }

        private void DrawHitboxes(SpriteBatch sb)
        {
            // Draw hover around every element
            DrawHitboxOutlineAndText(sb, DragSystem.InfoAccsBounds(), Element.InfoAccs, x: -70, color: elementColors[Element.InfoAccs]);

            if (Main.drawingPlayerChat)
                DrawHitboxOutlineAndText(sb, DragSystem.ChatBounds(), Element.Map, color: elementColors[Element.Map]);

            // Draw hotbar or inventory
            if (Main.playerInventory)
            {
                DrawHitboxOutlineAndText(sb, DragSystem.InventoryBounds(), Element.Inventory, x: -75, color: elementColors[Element.Inventory]);
                DrawHitboxOutlineAndText(sb, DragSystem.CraftingBounds(), Element.Crafting, x: -70, color: elementColors[Element.Crafting]);
                DrawHitboxOutlineAndText(sb, DragSystem.AccessoriesBounds(), Element.Accessories, x: -90, color: elementColors[Element.Accessories]);
            }
            else
            {
                DrawHitboxOutlineAndText(sb, DragSystem.HotbarBounds(), Element.Hotbar, x: -55, color: elementColors[Element.Hotbar]);
                DrawHitboxOutlineAndText(sb, DragSystem.BuffBounds(), Element.Buffs, x: -45, color: elementColors[Element.Buffs]);
            }

            if (Main.recBigList)
                DrawHitboxOutlineAndText(sb, DragSystem.CraftingWindowBounds(), Element.CraftingWindow, x: -125, color: elementColors[Element.CraftingWindow]);

            // Draw resource bars. Check which health and mana style is active:
            string activeSetName = Main.ResourceSetsManager.ActiveSet.DisplayedName;
            if (activeSetName.StartsWith("Classic"))
            {
                DrawHitboxOutlineAndText(sb, DragSystem.ClassicLifeBounds(), Element.ClassicLife, x: -90, color: elementColors[Element.ClassicLife]);
                DrawHitboxOutlineAndText(sb, DragSystem.ClassicManaBounds(), Element.ClassicMana, x: -5, color: elementColors[Element.ClassicMana]);
            }
            else if (activeSetName == "Fancy")
            {
                DrawHitboxOutlineAndText(sb, DragSystem.FancyLifeBounds(), Element.FancyLife, x: -80, color: elementColors[Element.FancyLife]);
                DrawHitboxOutlineAndText(sb, DragSystem.FancyManaBounds(), Element.FancyMana, x: -5, color: elementColors[Element.FancyMana]);
            }
            else if (activeSetName == "Fancy 2")
            {
                DrawHitboxOutlineAndText(sb, DragSystem.FancyLifeBounds(), Element.FancyLife, x: -80, color: elementColors[Element.FancyLife]);
                DrawHitboxOutlineAndText(sb, DragSystem.FancyLifeTextBounds(), Element.FancyLifeText, x: -112, color: elementColors[Element.FancyLifeText]);
                DrawHitboxOutlineAndText(sb, DragSystem.FancyManaBounds(), Element.FancyMana, x: -5, color: elementColors[Element.FancyMana]);
            }
            else if (activeSetName == "Bars")
            {
                DrawHitboxOutlineAndText(sb, DragSystem.BarsBounds(), Element.HorizontalBars, x: -120, color: elementColors[Element.HorizontalBars]);
            }
            else if (activeSetName == "Bars 2")
            {
                DrawHitboxOutlineAndText(sb, DragSystem.BarsBounds(), Element.HorizontalBars, x: -120, color: elementColors[Element.HorizontalBars]);
                DrawHitboxOutlineAndText(sb, DragSystem.BarLifeTextBounds(), Element.BarLifeText, x: -95, color: elementColors[Element.BarLifeText]);
            }
            else if (activeSetName == "Bars 3")
            {
                DrawHitboxOutlineAndText(sb, DragSystem.BarsBounds(), Element.HorizontalBars, x: -120, color: elementColors[Element.HorizontalBars]);
                DrawHitboxOutlineAndText(sb, DragSystem.BarLifeTextBounds(), Element.BarLifeText, x: -95, color: elementColors[Element.BarLifeText]);
                DrawHitboxOutlineAndText(sb, DragSystem.BarManaTextBounds(), Element.BarManaText, x: -110, color: elementColors[Element.BarManaText]);
            }

            DrawHitboxOutlineAndText(sb, DragSystem.MapBounds(), Element.Map, x: -40, color: elementColors[Element.Map]);
        }
    }
}
