using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using UICustomizer.Common.Systems;
using UICustomizer.Common.Systems.Hooks;
using UICustomizer.Helpers.Layouts;

namespace UICustomizer.UI.Tabs
{
    public sealed class EditorTab : Tab
    {
        private UIText positions;
        public Checkbox CheckboxX { get; private set; }
        public Checkbox CheckboxY { get; private set; }
        public Checkbox CheckboxOutline { get; private set; }
        public Checkbox CheckboxFill { get; private set; }
        public Checkbox CheckboxNames { get; private set; }
        public Checkbox CheckboxTextPos { get; private set; }

        // Store checkbox states
        private CheckboxState _checkboxXState = CheckboxState.Checked;
        private CheckboxState _checkboxYState = CheckboxState.Checked;
        private CheckboxState _checkboxFillState = CheckboxState.Checked;
        private CheckboxState _checkboxOutlineState = CheckboxState.Checked;
        private CheckboxState _checkboxNamesState = CheckboxState.Checked;
        private CheckboxState _checkboxTextPosState = CheckboxState.Checked;

        // Layouts
        public string CurrentLayoutName => LayoutHelper.CurrentLayoutName;
        public List<Button> LayoutButtons = [];
        private int _lastLayoutCount = -1;

        // Collapsible headers
        public bool _uiEditorExpanded = true;
        public bool _layoutsExpanded = true;
        public bool _positionsExpanded = true;
        public bool _optionsExpanded = true;

        public bool hideAllMode = false;
        private Vector2 _hideAllBtnPosition;

        public EditorTab(Action<Tab> select, Scrollbar bar = null) : base("Editor", select, bar)
        {
        }

        public void ResetHideAllState()
        {
            hideAllMode = false;

            // Clean up any floating buttons
            UICustomizerSystem sys = ModContent.GetInstance<UICustomizerSystem>();
            if (sys?.state != null)
            {
                var floatingButtons = sys.state.Children.OfType<Button>()
                    .Where(b => b.buttonText.Text == "Save").ToList();
                foreach (var btn in floatingButtons)
                {
                    sys.state.RemoveChild(btn);
                }
            }
        }

        public void SetInitialCheckboxStates()
        {
            // Set initial checkbox states
            CheckboxX.state = _checkboxXState;
            CheckboxY.state = _checkboxYState;
            CheckboxFill.state = _checkboxFillState;
            CheckboxOutline.state = _checkboxOutlineState;
            CheckboxNames.state = _checkboxNamesState;
            CheckboxTextPos.state = _checkboxTextPosState;

            // Set fill, outline, names and text position checkboxes to unchecked by default
            CheckboxFill.state = CheckboxState.Unchecked;
            CheckboxOutline.state = CheckboxState.Unchecked;
            CheckboxNames.state = CheckboxState.Unchecked;
            CheckboxTextPos.state = CheckboxState.Unchecked;
            Log.Info("Initial checkbox states set.");

            // Update _state variables
            _checkboxXState = CheckboxX.state;
            _checkboxYState = CheckboxY.state;
            _checkboxFillState = CheckboxFill.state;
            _checkboxOutlineState = CheckboxOutline.state;
            _checkboxNamesState = CheckboxNames.state;
            _checkboxTextPosState = CheckboxTextPos.state;
            Log.Info("Checkbox states updated.");
        }

        public void PopulatePublic() => Populate();
        protected override void Populate()
        {
            list.Clear();
            list.ListPadding = 0;
            Log.Info("hideAllMode: " + hideAllMode);

            if (hideAllMode)
            {
                var saveBtn = new Button(
                    text: "Save",
                    tooltip: () => "Save and exit edit mode",
                    onClick: () =>
                    {
                        // Reset hide all state before exiting
                        ResetHideAllState();

                        // Exit edit mode
                        UICustomizerSystem.ExitEditMode();
                    }
                );

                // Use stored position
                saveBtn.Left.Set(_hideAllBtnPosition.X, 0);
                saveBtn.Top.Set(_hideAllBtnPosition.Y, 0);
                saveBtn.Width.Set(100, 0);
                saveBtn.Height.Set(30, 0);

                UICustomizerSystem sys = ModContent.GetInstance<UICustomizerSystem>();
                sys?.state?.Append(saveBtn);
                return;
            }

            Log.Info("Populating EditorTab");

            AddCollapsibleHeader("UI Editor", () => _uiEditorExpanded, state => _uiEditorExpanded = state);
            PopulateUIEditor();

            AddCollapsibleHeader("Layouts", () => _layoutsExpanded, state => _layoutsExpanded = state);
            PopulateLayouts();

            AddCollapsibleHeader("Positions", () => _positionsExpanded, state => _positionsExpanded = state);
            PopulatePositions();

            AddCollapsibleHeader("Layout Options", () => _optionsExpanded, state => _optionsExpanded = state);
            PopulateOptions();

            list.Recalculate();
        }

        private void PopulateUIEditor()
        {
            if (!_uiEditorExpanded) return;

            // Capture current checkbox states BEFORE rebuilding
            if (CheckboxX != null) _checkboxXState = CheckboxX.state;
            if (CheckboxY != null) _checkboxYState = CheckboxY.state;
            if (CheckboxFill != null) _checkboxFillState = CheckboxFill.state;
            if (CheckboxOutline != null) _checkboxOutlineState = CheckboxOutline.state;
            if (CheckboxNames != null) _checkboxNamesState = CheckboxNames.state;
            if (CheckboxTextPos != null) _checkboxTextPosState = CheckboxTextPos.state;

            const int btnH = 30;
            const int vGap = 4;
            const int rowTot = btnH + vGap;
            const int leftX = 0;
            const int chkX = 104; // first checkbox column starting left pos
            const int chkX2 = chkX + 70; // second checkbox column starting left pos
            const int chkYOffset = 2;

            static UIElement Row(UIElement btn, UIElement chkLeft = null, UIElement chkRight = null, int rowIndex = 0)
            {
                var row = new UIElement
                {
                    Width = { Percent = 1f },
                    Height = { Pixels = btnH }
                };
                int top = rowIndex * rowTot;
                row.Top.Pixels = top;

                btn.Left.Pixels = leftX;
                btn.Top.Pixels = 0;
                row.Append(btn);

                if (chkLeft != null)
                {
                    chkLeft.Left.Pixels = chkX;
                    chkLeft.Top.Pixels = chkYOffset;
                    row.Append(chkLeft);
                }
                if (chkRight != null)
                {
                    chkRight.Left.Pixels = chkX2;
                    chkRight.Top.Pixels = chkYOffset;
                    row.Append(chkRight);
                }
                return row;
            }

            Button hideAllBtn = null;
            hideAllBtn = new Button(
                text: "Hide All",
                tooltip: () => "Hide everything except a save button",
                onClick: () =>
                {
                    hideAllMode = true;
                    var dimensions = hideAllBtn.GetDimensions();
                    _hideAllBtnPosition = new Vector2(dimensions.X, dimensions.Y);
                    Populate(); // Rebuild by removing everything except the new button
                }
            );
            var saveBtn = new Button("Save", () => "Save and exit edit mode", UICustomizerSystem.ExitEditMode);
            var resetBtn = new Button("Reset", () => "Reset all offsets", ResetAllOffsets);
            var lifeBtn = new Button("Life",
                () => Main.ResourceSetsManager.ActiveSet.DisplayedName,
                onClick: () => Main.ResourceSetsManager.CycleResourceSet(),
                onRightClick: () =>
                {
                    int currentIndex = Main.ResourceSetsManager.selectedSet;
                    int newIndex = (currentIndex - 1 + Main.ResourceSetsManager.accessKeys.Count) % Main.ResourceSetsManager.accessKeys.Count;
                    Main.ResourceSetsManager.SetActiveFrameFromIndex(newIndex);
                },
                width: 70);
            var mapBtn = new Button("Map",
            tooltip: () =>
            {
                return Main.mapStyle switch
                {
                    0 => "Map: Hidden",
                    1 => "Map: Minimap",
                    2 => "Map: Overlay",
                    _ => throw new NotImplementedException(),
                };
            }, width: 70,
            onClick: () => { Main.mapStyle = (Main.mapStyle + 1) % 3; },
            onRightClick: () => { Main.mapStyle = (Main.mapStyle - 1 + 3) % 3; }
            );
            var scaleBtn = new Button("UIScale",
                tooltip: () => $"UIScale: {Main.UIScale * 100:F1}%",
                onClick: () =>
                {
                    if (Main.UIScale >= 2.0f)
                    {
                        Main.UIScale = 2.0f;
                    }
                    else
                    {
                        Main.UIScale += 0.05f;
                    }
                },
                onRightClick: () =>
                {
                    if (Main.UIScale <= 0.5f)
                    {
                        Main.UIScale = 0.5f;
                    }

                    // Decrease by 0.01 on right click
                    Main.UIScale -= 0.01f;
                }, width: 100
            );

            // Create checkboxes with preserved states (remove OnLeftClick handlers)
            CheckboxX = new Checkbox("X", "Move only in X", 50, initialState: _checkboxXState);
            CheckboxY = new Checkbox("Y", "Move only in Y", 50, initialState: _checkboxYState);
            CheckboxFill = new Checkbox("Fill", "Show fill", 80, initialState: _checkboxFillState);
            CheckboxOutline = new Checkbox("Outline", "Show outlines", 80, initialState: _checkboxOutlineState);
            CheckboxNames = new Checkbox("Text", "Show names", 80, initialState: _checkboxNamesState);
            CheckboxTextPos = new Checkbox("TextPos", "Offset text position", 80, initialState: _checkboxTextPosState);

            Gap(4);
            TryAdd(Row(saveBtn, CheckboxX, CheckboxY, 0));
            TryAdd(Row(hideAllBtn, CheckboxFill, CheckboxOutline, 1));
            TryAdd(Row(resetBtn, CheckboxNames, CheckboxTextPos, 2));
            var buttonRow = new UIElement
            {
                Width = { Percent = 1f },
                Height = { Pixels = btnH },
                Top = { Pixels = 3 * rowTot }
            };

            scaleBtn.Left.Pixels = leftX;
            scaleBtn.Top.Pixels = 0;
            buttonRow.Append(scaleBtn);

            mapBtn.Left.Pixels = leftX + 105;
            mapBtn.Top.Pixels = 0;
            buttonRow.Append(mapBtn);

            lifeBtn.Left.Pixels = leftX + 185;
            lifeBtn.Top.Pixels = 0;
            buttonRow.Append(lifeBtn);

            TryAdd(buttonRow);
            Gap(12);
        }

        private void PopulateLayouts()
        {
            if (!_layoutsExpanded) return;

            foreach (var layoutName in FileHelper.GetLayouts())
            {
                string tooltip = layoutName == LayoutHelper.CurrentLayoutName
                    ? "This is the currently applied layout."
                    : $"Apply the '{layoutName}' layout";

                if (layoutName == "Active")
                {
                    tooltip = "Currently active editing layout that changes every time you drag";
                }

                var btn = new Button(
                    text: layoutName,
                    tooltip: () => tooltip,
                    onClick: () =>
                    {
                        var sys = ModContent.GetInstance<UICustomizerSystem>();
                        sys.state.panel.CancelDrag();
                        LayoutHelper.ApplyLayout(layoutName);
                        LayoutHelper.CurrentLayoutName = layoutName;
                        LayoutHelper.SaveLastLayout(); // Save the selected layout
                        Populate(); // Refresh to update colors
                    },
                    maxWidth: true,
                    onRightClick: () =>
                    {
                        // Open the layout file in the editor
                        if (UICustomizerSystem.EditModeActive)
                        {
                            FileHelper.OpenLayoutFile(layoutName);
                        }
                    }
                );

                if (layoutName == LayoutHelper.CurrentLayoutName)
                    btn.buttonText.TextColor = Color.Yellow;
                else
                    btn.buttonText.TextColor = Color.White;

                LayoutButtons.Add(btn);
                TryAdd(btn);
            }
            Gap(12);
        }

        private void PopulatePositions()
        {
            if (!_positionsExpanded) return;

            // Positions text area
            positions = new UIText("", 0.35f, true)
            {
                Width = { Percent = 1f },
                Height = { Pixels = 240 }
            };
            TryAdd(positions);
            Gap(12);
        }

        private void PopulateOptions()
        {
            if (!_optionsExpanded) return;

            var openFolderBtn = new Button(
                text: "Open layouts folder",
                tooltip: () => "Open the layouts folder to edit, share or add new layouts",
                onClick: () =>
                {
                    if (!UICustomizerSystem.EditModeActive) return;
                    FileHelper.OpenLayoutFolder();
                },
                maxWidth: true
            );
            var createNewBtn = new Button(
                text: "Save current layout",
                tooltip: () => "Creates and opens a new layout file with the current layout",
                onClick: () =>
                {
                    if (!UICustomizerSystem.EditModeActive) return;
                    FileHelper.CreateAndOpenNewLayoutFile("MyNewLayout");
                    Populate();
                },
                maxWidth: true
            );
            var deleteAllBtn = new Button(
                text: "Remove all layouts",
                tooltip: () => "Remove all layouts from the folder",
                topOffset: 0,
                onClick: () =>
                {
                    if (!UICustomizerSystem.EditModeActive) return;
                    FileHelper.DeleteAllLayouts();
                    Populate();
                },
                maxWidth: true
            );

            TryAdd(openFolderBtn);
            TryAdd(createNewBtn);
            TryAdd(deleteAllBtn);
            Gap(12);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdatePositions();
            positions.Height.Set(300, 0); // TODO update to real value

            // Check if layouts folder changed
            int currentCount = FileHelper.GetLayouts().Count();
            if (currentCount != _lastLayoutCount)
            {
                _lastLayoutCount = currentCount;
                Populate(); // Rebuild the tab
            }
        }

        private void UpdatePositions()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Hotbar: ({(int)HotbarHook.OffsetX}, {(int)HotbarHook.OffsetY})");
            sb.AppendLine($"Buffs: ({(int)BuffHook.OffsetX}, {(int)BuffHook.OffsetY})");
            sb.AppendLine($"Map: ({(int)MapHook.OffsetX},  {(int)MapHook.OffsetY})");
            sb.AppendLine($"InfoAccs: ({(int)InfoAccsHook.OffsetX}, {(int)InfoAccsHook.OffsetY})");
            sb.AppendLine($"ClassicLife: ({(int)ClassicLifeHook.OffsetX}, {(int)ClassicLifeHook.OffsetY})");
            sb.AppendLine($"ClassicMana: ({(int)ClassicManaHook.OffsetX}, {(int)ClassicManaHook.OffsetY})");
            sb.AppendLine($"FancyLife: ({(int)FancyLifeHook.OffsetX}, {(int)FancyLifeHook.OffsetY})");
            sb.AppendLine($"FancyLifeText: ({(int)FancyLifeTextHook.OffsetX}, {(int)FancyLifeTextHook.OffsetY})");
            sb.AppendLine($"FancyMana: ({(int)FancyManaHook.OffsetX}, {(int)FancyManaHook.OffsetY})");
            sb.AppendLine($"Bars: ({(int)HorizontalBarsHook.OffsetX}, {(int)HorizontalBarsHook.OffsetY})");
            sb.AppendLine($"BarsLifeText: ({(int)BarLifeTextHook.OffsetX}, {(int)BarLifeTextHook.OffsetY})");
            sb.AppendLine($"BarsManaText: ({(int)BarManaTextHook.OffsetX}, {(int)BarManaTextHook.OffsetY})");
            sb.AppendLine($"Chat: ({(int)ChatHook.OffsetX}, {(int)ChatHook.OffsetY})");
            positions.TextOriginX = 0;
            //positions.VAlign = 0f;
            //positions.HAlign = 0f;
            //positions.Left.Set(10, 0);
            positions.SetText(sb.ToString(), 0.33f, true);
        }

        private void ResetAllOffsets()
        {
            ChatHook.OffsetX = ChatHook.OffsetY = 0f;
            HotbarHook.OffsetX = HotbarHook.OffsetY = 0f;
            MapHook.OffsetX = MapHook.OffsetY = 0f;
            FancyLifeHook.OffsetX = FancyLifeHook.OffsetY = 0f;
            ClassicLifeHook.OffsetX = ClassicLifeHook.OffsetY = 0f;
            ClassicManaHook.OffsetX = ClassicManaHook.OffsetY = 0f;
            FancyManaHook.OffsetX = FancyManaHook.OffsetY = 0f;
            HorizontalBarsHook.OffsetX = HorizontalBarsHook.OffsetY = 0f;
            InfoAccsHook.OffsetX = InfoAccsHook.OffsetY = 0f;
            BuffHook.OffsetX = BuffHook.OffsetY = 0f;
            BarLifeTextHook.OffsetX = BarLifeTextHook.OffsetY = 0f;
            BarManaTextHook.OffsetX = BarManaTextHook.OffsetY = 0f;

            // Write to active layout
            LayoutHelper.SaveActiveLayout();
        }
    }
}
