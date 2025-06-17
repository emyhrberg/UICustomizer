using System.Collections.Generic;

namespace UICustomizer.Helpers.Layouts
{
    public static class ElementHelper
    {
        public enum Element
        {
            Chat,
            Hotbar,
            Map,
            InfoAccs,
            ClassicLife,
            ClassicMana,
            FancyLife,
            FancyLifeText,
            FancyMana,
            HorizontalBars,
            BarLifeText,
            BarManaText,
            Buffs,
            Inventory,
            Crafting,
            Accessories,
            CraftingWindow
        }

        public static Dictionary<Element, string> ElementInterfaceLayerMapping = new()
        {
            [Element.Chat] = "Vanilla: Player Chat",
            [Element.Hotbar] = "Vanilla: Hotbar",
            [Element.Map] = "Vanilla: Map / Minimap",
            [Element.InfoAccs] = "Vanilla: Info Accessories Bar", 
            [Element.ClassicLife] = "Vanilla: Resource Bars",
            [Element.ClassicMana] = "Vanilla: Resource Bars",
            [Element.FancyLife] = "Vanilla: Resource Bars",
            [Element.FancyLifeText] = "Vanilla: Resource Bars",
            [Element.FancyMana] = "Vanilla: Resource Bars",
            [Element.HorizontalBars] = "Vanilla: Resource Bars",
            [Element.Buffs] = "Vanilla: Resource Bars",
            [Element.Inventory] = "Vanilla: Inventory",
            [Element.Crafting] = "Vanilla: Inventory",
            [Element.Accessories] = "Vanilla: Inventory",
            [Element.CraftingWindow] = "Vanilla: Inventory",
        };
    }
}