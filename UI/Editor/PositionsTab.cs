using System;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using UICustomizer.Common.Systems.Hooks;
using static UICustomizer.Helpers.Layouts.ElementHelper;

namespace UICustomizer.UI.Editor
{
    public class PositionsTab : Tab
    {
        private bool _positionsExpanded = true;
        private UIText[] coordTexts; // Store references to coordinate texts

        public PositionsTab() : base("Positions")
        {
        }

        public override void Populate()
        {
            list.Clear();
            list.SetPadding(2);

            var positionsSection = new CollapsibleSection(
                title: "Element Positions",
                initialState: _positionsExpanded,
                buildContent: BuildPositionsContent,
                onToggle: () => _positionsExpanded = !_positionsExpanded,
                contentHeight: () => 340f
            );

            //Gap(8);
            //positionsSection.Left.Set(0, 0);
            //positionsSection.Width.Set(-20, 1f);

            list.Add(positionsSection);
        }

        private void BuildPositionsContent(UIElement contentContainer)
        {
            const float LineHeight = 18f;
            const float FirstColumnOffset = 135f;
            const float CoordColumnWidth = 68f;

            var elements = new (Element element, Func<int> getX, Func<int> getY, Action<int, int> reset)[]
            {
                ( Element.Hotbar,      () => (int)HotbarHook.OffsetX,      () => (int)HotbarHook.OffsetY,      (x,y) => { HotbarHook.OffsetX = x; HotbarHook.OffsetY = y; } ),
                ( Element.Buffs,       () => (int)BuffHook.OffsetX,        () => (int)BuffHook.OffsetY,        (x,y) => { BuffHook.OffsetX = x;   BuffHook.OffsetY   = y; } ),
                ( Element.Map,         () => (int)MapHook.OffsetX,         () => (int)MapHook.OffsetY,         (x,y) => { MapHook.OffsetX = x;    MapHook.OffsetY    = y; } ),
                ( Element.InfoAccs,    () => (int)InfoAccsHook.OffsetX,    () => (int)InfoAccsHook.OffsetY,    (x,y) => { InfoAccsHook.OffsetX = x;InfoAccsHook.OffsetY = y; } ),
                ( Element.ClassicLife, () => (int)ClassicLifeHook.OffsetX, () => (int)ClassicLifeHook.OffsetY, (x,y) => { ClassicLifeHook.OffsetX = x;ClassicLifeHook.OffsetY = y; } ),
                ( Element.ClassicMana, () => (int)ClassicManaHook.OffsetX, () => (int)ClassicManaHook.OffsetY, (x,y) => { ClassicManaHook.OffsetX = x;ClassicManaHook.OffsetY = y; } ),
                ( Element.FancyLife,   () => (int)FancyLifeHook.OffsetX,   () => (int)FancyLifeHook.OffsetY,   (x,y) => { FancyLifeHook.OffsetX = x;  FancyLifeHook.OffsetY  = y; } ),
                ( Element.FancyLifeText,() => (int)FancyLifeTextHook.OffsetX,() => (int)FancyLifeTextHook.OffsetY,(x,y) => { FancyLifeTextHook.OffsetX = x;FancyLifeTextHook.OffsetY = y; } ),
                ( Element.FancyMana,   () => (int)FancyManaHook.OffsetX,   () => (int)FancyManaHook.OffsetY,   (x,y) => { FancyManaHook.OffsetX = x;  FancyManaHook.OffsetY  = y; } ),
                ( Element.HorizontalBars,() => (int)HorizontalBarsHook.OffsetX,() => (int)HorizontalBarsHook.OffsetY,(x,y) => { HorizontalBarsHook.OffsetX = x;HorizontalBarsHook.OffsetY = y; } ),
                ( Element.BarLifeText, () => (int)BarLifeTextHook.OffsetX, () => (int)BarLifeTextHook.OffsetY, (x,y) => { BarLifeTextHook.OffsetX = x;  BarLifeTextHook.OffsetY  = y; } ),
                ( Element.BarManaText, () => (int)BarManaTextHook.OffsetX, () => (int)BarManaTextHook.OffsetY, (x,y) => { BarManaTextHook.OffsetX = x;  BarManaTextHook.OffsetY  = y; } ),
                ( Element.Chat,        () => (int)ChatHook.OffsetX,        () => (int)ChatHook.OffsetY,        (x,y) => { ChatHook.OffsetX = x;       ChatHook.OffsetY       = y; } ),
                ( Element.Inventory,   () => (int)InventoryHook.OffsetX,   () => (int)InventoryHook.OffsetY,   (x,y) => { InventoryHook.OffsetX = x;  InventoryHook.OffsetY  = y; } ),
                ( Element.Crafting,     () => (int)CraftingHook.OffsetX,() => (int)CraftingHook.OffsetY,(x,y) => { CraftingHook.OffsetX = x;  CraftingHook.OffsetY  = y; } ),
                ( Element.Accessories, () => (int)AccessoriesHook.OffsetX, () => (int)AccessoriesHook.OffsetY, (x,y) => { AccessoriesHook.OffsetX = x;AccessoriesHook.OffsetY = y; } ),
                ( Element.CraftingWindow,() => (int)CraftWindowHook.OffsetX,() => (int)CraftWindowHook.OffsetY,(x,y) => { CraftWindowHook.OffsetX = x;  CraftWindowHook.OffsetY  = y; } )
            };

            // Initialize coordinate text array
            coordTexts = new UIText[elements.Length];

            float yOff = 10f;
            for (int i = 0; i < elements.Length; i++)
            {
                var (element, getX, getY, reset) = elements[i];

                // name
                var nameText = new UIText($"{element}:", 0.35f, true)
                {
                    Left = { Pixels = 0 },
                    Top = { Pixels = yOff },
                    TextColor = Color.LightBlue
                };
                contentContainer.Append(nameText);

                // coords (store reference for updating)
                coordTexts[i] = new UIText($"({getX()}, {getY()})", 0.35f, true)
                {
                    Left = { Pixels = FirstColumnOffset },
                    Top = { Pixels = yOff },
                    TextColor = Color.White
                };
                contentContainer.Append(coordTexts[i]);

                // reset button
                var resetBtn = new UIPanel
                {
                    Width = { Pixels = 70 },
                    Height = { Pixels = 20 },
                    Left = { Pixels = FirstColumnOffset + CoordColumnWidth },
                    Top = { Pixels = yOff - 5 },
                    BackgroundColor = Color.DarkRed * 0.7f,
                    BorderColor = Color.Red
                };
                var resetTxt = new UIText("Reset", 0.32f, true)
                {
                    HAlign = 0.5f,
                    VAlign = 0.5f,
                    TextColor = Color.White
                };
                resetBtn.Append(resetTxt);
                resetBtn.OnLeftClick += (_, _) => reset(0, 0);
                resetBtn.OnMouseOver += (_, _) => resetBtn.BackgroundColor = Color.Red * 0.9f;
                resetBtn.OnMouseOut += (_, _) => resetBtn.BackgroundColor = Color.DarkRed * 0.8f;

                contentContainer.Append(resetBtn);

                yOff += LineHeight;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update positions every frame if panel exists and is expanded
            if (_positionsExpanded && coordTexts != null)
            {
                UpdatePositionTexts();
            }
        }

        private void UpdatePositionTexts()
        {
            var elements = new (Func<int> getX, Func<int> getY)[]
            {
                (() => (int)HotbarHook.OffsetX, () => (int)HotbarHook.OffsetY),
                (() => (int)BuffHook.OffsetX, () => (int)BuffHook.OffsetY),
                (() => (int)MapHook.OffsetX, () => (int)MapHook.OffsetY),
                (() => (int)InfoAccsHook.OffsetX, () => (int)InfoAccsHook.OffsetY),
                (() => (int)ClassicLifeHook.OffsetX, () => (int)ClassicLifeHook.OffsetY),
                (() => (int)ClassicManaHook.OffsetX, () => (int)ClassicManaHook.OffsetY),
                (() => (int)FancyLifeHook.OffsetX, () => (int)FancyLifeHook.OffsetY),
                (() => (int)FancyLifeTextHook.OffsetX, () => (int)FancyLifeTextHook.OffsetY),
                (() => (int)FancyManaHook.OffsetX, () => (int)FancyManaHook.OffsetY),
                (() => (int)HorizontalBarsHook.OffsetX, () => (int)HorizontalBarsHook.OffsetY),
                (() => (int)BarLifeTextHook.OffsetX, () => (int)BarLifeTextHook.OffsetY),
                (() => (int)BarManaTextHook.OffsetX, () => (int)BarManaTextHook.OffsetY),
                (() => (int)ChatHook.OffsetX, () => (int)ChatHook.OffsetY),
                (() => (int)InventoryHook.OffsetX, () => (int)InventoryHook.OffsetY),
                (() => (int)CraftingHook.OffsetX, () => (int)CraftingHook.OffsetY),
                (() => (int)AccessoriesHook.OffsetX, () => (int)AccessoriesHook.OffsetY),
                (() => (int)CraftWindowHook.OffsetX, () => (int)CraftWindowHook.OffsetY)
            };

            // Update existing coordinate texts
            for (int i = 0; i < Math.Min(coordTexts.Length, elements.Length); i++)
            {
                var (getX, getY) = elements[i];
                coordTexts[i].SetText($"({getX()}, {getY()})", 0.30f, true);
            }
        }
    }
}