using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using UICustomizer.EditMode.Hooks;

namespace UICustomizer.Edit.UI;

internal sealed class PositionsPanel : UIPanel
{
    private readonly List<UIText> _coordTexts = new();

    public PositionsPanel()
    {
        BorderColor = new Color(89, 116, 213) * 0.9f;
        BackgroundColor = new Color(73, 94, 171) * 0.9f;

        Width.Set(520, 0);
        Height.Set(420, 0);
        Left.Set(70, 0);
        Top.Set(0, 0);
        SetPadding(8);

        float y = 6f;
        void AddLine(string name, Func<int> getX, Func<int> getY, Action<int, int> resetXY)
        {
            const float lineH = 22f;
            var nameTxt = new UIText($"{name}:", 0.8f) { Left = { Pixels = 0 }, Top = { Pixels = y } };
            Append(nameTxt);

            var coord = new UIText($"({getX()}, {getY()})", 0.8f)
            { Left = { Pixels = 80 }, Top = { Pixels = y } };
            _coordTexts.Add(coord);
            Append(coord);

            var reset = new UIPanel
            {
                Width = { Pixels = 50 },
                Height = { Pixels = 18 },
                Left = { Pixels = 160 },
                Top = { Pixels = y - 1 },
                BackgroundColor = new Color(100, 40, 40) * 0.7f,
                BorderColor = new Color(150, 60, 60)
            };
            reset.Append(new UIText("Reset", 0.7f) { HAlign = 0.5f, VAlign = 0.5f, TextColor = Color.White });
            reset.OnLeftClick += (_, _) => resetXY(0, 0);
            reset.OnMouseOver += (_, _) => reset.BackgroundColor = new Color(150, 50, 50) * 0.9f;
            reset.OnMouseOut += (_, _) => reset.BackgroundColor = new Color(100, 40, 40) * 0.7f;
            Append(reset);

            y += lineH;
        }

        AddLine("Hotbar", () => (int)HotbarHook.OffsetX, () => (int)HotbarHook.OffsetY, (x, y) => { HotbarHook.OffsetX = x; HotbarHook.OffsetY = y; });
        AddLine("Buffs", () => (int)BuffHook.OffsetX, () => (int)BuffHook.OffsetY, (x, y) => { BuffHook.OffsetX = x; BuffHook.OffsetY = y; });
        AddLine("Map", () => (int)MapHook.OffsetX, () => (int)MapHook.OffsetY, (x, y) => { MapHook.OffsetX = x; MapHook.OffsetY = y; });
        AddLine("Info Accs", () => (int)InfoAccsHook.OffsetX, () => (int)InfoAccsHook.OffsetY, (x, y) => { InfoAccsHook.OffsetX = x; InfoAccsHook.OffsetY = y; });
        AddLine("Classic Life", () => (int)ClassicLifeHook.OffsetX, () => (int)ClassicLifeHook.OffsetY, (x, y) => { ClassicLifeHook.OffsetX = x; ClassicLifeHook.OffsetY = y; });
        AddLine("Classic Mana", () => (int)ClassicManaHook.OffsetX, () => (int)ClassicManaHook.OffsetY, (x, y) => { ClassicManaHook.OffsetX = x; ClassicManaHook.OffsetY = y; });
        AddLine("Fancy Life", () => (int)FancyLifeHook.OffsetX, () => (int)FancyLifeHook.OffsetY, (x, y) => { FancyLifeHook.OffsetX = x; FancyLifeHook.OffsetY = y; });
        AddLine("Fancy LifeTxt", () => (int)FancyLifeTextHook.OffsetX, () => (int)FancyLifeTextHook.OffsetY, (x, y) => { FancyLifeTextHook.OffsetX = x; FancyLifeTextHook.OffsetY = y; });
        AddLine("Fancy Mana", () => (int)FancyManaHook.OffsetX, () => (int)FancyManaHook.OffsetY, (x, y) => { FancyManaHook.OffsetX = x; FancyManaHook.OffsetY = y; });
        AddLine("Bars", () => (int)HorizontalBarsHook.OffsetX, () => (int)HorizontalBarsHook.OffsetY, (x, y) => { HorizontalBarsHook.OffsetX = x; HorizontalBarsHook.OffsetY = y; });
        AddLine("Bar LifeTxt", () => (int)BarLifeTextHook.OffsetX, () => (int)BarLifeTextHook.OffsetY, (x, y) => { BarLifeTextHook.OffsetX = x; BarLifeTextHook.OffsetY = y; });
        AddLine("Bar ManaTxt", () => (int)BarManaTextHook.OffsetX, () => (int)BarManaTextHook.OffsetY, (x, y) => { BarManaTextHook.OffsetX = x; BarManaTextHook.OffsetY = y; });
        AddLine("Chat", () => (int)ChatHook.OffsetX, () => (int)ChatHook.OffsetY, (x, y) => { ChatHook.OffsetX = x; ChatHook.OffsetY = y; });
        AddLine("Inventory", () => (int)InventoryHook.OffsetX, () => (int)InventoryHook.OffsetY, (x, y) => { InventoryHook.OffsetX = x; InventoryHook.OffsetY = y; });
        AddLine("Crafting", () => (int)CraftingHook.OffsetX, () => (int)CraftingHook.OffsetY, (x, y) => { CraftingHook.OffsetX = x; CraftingHook.OffsetY = y; });
        AddLine("Accessories", () => (int)AccessoriesHook.OffsetX, () => (int)AccessoriesHook.OffsetY, (x, y) => { AccessoriesHook.OffsetX = x; AccessoriesHook.OffsetY = y; });
        AddLine("CraftWindow", () => (int)CraftWindowHook.OffsetX, () => (int)CraftWindowHook.OffsetY, (x, y) => { CraftWindowHook.OffsetX = x; CraftWindowHook.OffsetY = y; });
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // live coordinate refresh
        int i = 0;
        void Refresh(Func<int> getX, Func<int> getY)
        {
            if (i < _coordTexts.Count)
                _coordTexts[i++].SetText($"({getX()}, {getY()})");
        }

        i = 0;
        Refresh(() => (int)HotbarHook.OffsetX, () => (int)HotbarHook.OffsetY);
        Refresh(() => (int)BuffHook.OffsetX, () => (int)BuffHook.OffsetY);
        Refresh(() => (int)MapHook.OffsetX, () => (int)MapHook.OffsetY);
        Refresh(() => (int)InfoAccsHook.OffsetX, () => (int)InfoAccsHook.OffsetY);
        Refresh(() => (int)ClassicLifeHook.OffsetX, () => (int)ClassicLifeHook.OffsetY);
        Refresh(() => (int)ClassicManaHook.OffsetX, () => (int)ClassicManaHook.OffsetY);
        Refresh(() => (int)FancyLifeHook.OffsetX, () => (int)FancyLifeHook.OffsetY);
        Refresh(() => (int)FancyLifeTextHook.OffsetX, () => (int)FancyLifeTextHook.OffsetY);
        Refresh(() => (int)FancyManaHook.OffsetX, () => (int)FancyManaHook.OffsetY);
        Refresh(() => (int)HorizontalBarsHook.OffsetX, () => (int)HorizontalBarsHook.OffsetY);
        Refresh(() => (int)BarLifeTextHook.OffsetX, () => (int)BarLifeTextHook.OffsetY);
        Refresh(() => (int)BarManaTextHook.OffsetX, () => (int)BarManaTextHook.OffsetY);
        Refresh(() => (int)ChatHook.OffsetX, () => (int)ChatHook.OffsetY);
        Refresh(() => (int)InventoryHook.OffsetX, () => (int)InventoryHook.OffsetY);
        Refresh(() => (int)CraftingHook.OffsetX, () => (int)CraftingHook.OffsetY);
        Refresh(() => (int)AccessoriesHook.OffsetX, () => (int)AccessoriesHook.OffsetY);
        Refresh(() => (int)CraftWindowHook.OffsetX, () => (int)CraftWindowHook.OffsetY);
    }
}
