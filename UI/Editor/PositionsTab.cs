using System;
using Microsoft.Xna.Framework; // Required for Color
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using UICustomizer.Common.Systems.Hooks; // Assuming your hooks are here
using UICustomizer.Helpers.Layouts;
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
            list.SetPadding(20);
            list.ListPadding = 0; // No extra padding between items in the list itself
            list.Left.Set(-8, 0);
            list.Top.Set(-10, 0);

            int elementCount = Enum.GetValues(typeof(Element)).Length;
            const float lineHeight = 22f;

            var positionsSection = new CollapsibleSection(
                title: "Positions", // CHANGED BACK
                initialState: _positionsExpanded,
                buildContent: BuildPositionsContent,
                onToggle: () => _positionsExpanded = !_positionsExpanded,
                contentHeightFunc: () => Math.Max(80, elementCount * lineHeight + 18),
                buildHeader: header =>
                {
                    var resetAll = new Button(
                        text: "Reset All",
                        onClick: () =>
                        {
                            LayoutHelper.ResetAllOffsets();
                        },
                        tooltip: () => "Reset all positions",
                        width: 85
                    )
                    {
                        HAlign = 1f,
                        VAlign = 0.5f,
                        Left = { Pixels = 0 }
                    };
                    header.Append(resetAll);
                }
            );

            list.Add(positionsSection);
            list.Recalculate();
        }

        private void BuildPositionsContent(UIElement contentContainer)
        {
            const float LineHeight = 22f;
            const float ElementNameMaxWidth = 95f; 
            const float CoordTextLeftOffset = ElementNameMaxWidth + 5f; 
            const float CoordTextWidth = 85f;
            const float ResetButtonLeftOffset = CoordTextLeftOffset + CoordTextWidth + 20f;

            var elementsData = new (Element element, Func<int> getX, Func<int> getY, Action<int, int> resetXY)[]
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
                ( Element.Crafting,    () => (int)CraftingHook.OffsetX,    () => (int)CraftingHook.OffsetY,    (x,y) => { CraftingHook.OffsetX = x;  CraftingHook.OffsetY  = y; } ),
                ( Element.Accessories, () => (int)AccessoriesHook.OffsetX, () => (int)AccessoriesHook.OffsetY, (x,y) => { AccessoriesHook.OffsetX = x;AccessoriesHook.OffsetY = y; } ),
                ( Element.CraftingWindow,() => (int)CraftWindowHook.OffsetX,() => (int)CraftWindowHook.OffsetY,(x,y) => { CraftWindowHook.OffsetX = x;  CraftWindowHook.OffsetY  = y; } )
            };

            // Initialize coordinate text array
            coordTexts = new UIText[elementsData.Length];

            float yOff = 5f;
            for (int i = 0; i < elementsData.Length; i++)
            {
                // CHANGED: Deconstruction
                var (element, getX, getY, resetXY) = elementsData[i];

                // Element name (same as before)
                var nameText = new UIText($"{element}:", 0.8f)
                {
                    Left = { Pixels = 0 },
                    Top = { Pixels = yOff },
                    Width = { Pixels = ElementNameMaxWidth },
                    TextOriginX = 0f,
                    TextColor = Color.White
                };
                contentContainer.Append(nameText);

                // Coordinates (X, Y) (same as before)
                coordTexts[i] = new UIText($"({getX()}, {getY()})", 0.8f)
                {
                    Left = { Pixels = CoordTextLeftOffset },
                    Top = { Pixels = yOff },
                    Width = { Pixels = CoordTextWidth },
                    TextOriginX = 0f,
                    TextColor = Color.White
                };
                contentContainer.Append(coordTexts[i]);

                // REMOVE Width display block
                // widthTexts[i] = new UIText($"W: {getWidth()}", 0.8f) ...
                // contentContainer.Append(widthTexts[i]);

                // Reset button for X,Y (Left.Pixels uses the adjusted ResetButtonLeftOffset)
                var resetBtn = new UIPanel
                {
                    Width = { Pixels = 60 },
                    Height = { Pixels = 18 },
                    Left = { Pixels = ResetButtonLeftOffset }, // Uses adjusted constant
                    Top = { Pixels = yOff - 1 },
                    BackgroundColor = new Color(100, 40, 40) * 0.7f,
                    BorderColor = new Color(150, 60, 60)
                };
                var resetTxt = new UIText("Reset", 0.75f)
                {
                    HAlign = 0.5f,
                    VAlign = 0.5f,
                    TextColor = Color.White
                };
                resetBtn.Append(resetTxt);
                resetBtn.OnLeftClick += (_, _) => resetXY(0, 0);
                resetBtn.OnMouseOver += (_, _) => resetBtn.BackgroundColor = new Color(150, 50, 50) * 0.9f;
                resetBtn.OnMouseOut += (_, _) => resetBtn.BackgroundColor = new Color(100, 40, 40) * 0.7f;

                contentContainer.Append(resetBtn);

                yOff += LineHeight;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_positionsExpanded && coordTexts != null)
            {
                UpdatePositionAndWidthTexts();
            }
        }

        private void UpdatePositionAndWidthTexts()
        {
            var elementsDataGetters = new (Func<int> getX, Func<int> getY)[]
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

            if (coordTexts == null || elementsDataGetters.Length == 0) return;


            for (int i = 0; i < Math.Min(coordTexts.Length, elementsDataGetters.Length); i++)
            {
                if (coordTexts[i] == null) continue;

                var (getX, getY) = elementsDataGetters[i];
                coordTexts[i].SetText($"({getX()}, {getY()})");
            }
        }
    }
}