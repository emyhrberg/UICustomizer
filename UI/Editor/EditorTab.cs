using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using UICustomizer.Common.Systems;
using UICustomizer.Common.Systems.Hooks;
using UICustomizer.Helpers;
using UICustomizer.Helpers.Layouts;
using UICustomizer.UI.SliderElements;

namespace UICustomizer.UI.Editor
{
    public sealed class EditorTab : Tab
    {
        public Checkbox CheckboxEdit { get; private set; }
        public Checkbox CheckboxHitboxes { get; private set; }
        public Checkbox CheckboxResize { get; private set; }
        public Checkbox CheckboxSnap { get; private set; }
        public Checkbox CheckboxNames { get; private set; }

        // Simplified state storage using a dictionary
        private readonly Dictionary<string, CheckboxState> _checkboxStates = new()
        {
            ["Edit"] = CheckboxState.Checked,
            ["Resize"] = CheckboxState.Checked,
            ["Hitboxes"] = CheckboxState.Checked,
            ["Snap"] = CheckboxState.Checked,
            ["Names"] = CheckboxState.Checked,
        };

        private float DarknessLevel;
        public bool _uiEditorExpanded = true;
        public bool hideAllMode = false;
        private Vector2 _hideAllBtnPosition;

        public EditorTab() : base("UI")
        {
            _uiEditorExpanded = true;
        }

        // Helper method to save current checkbox states
        private void SaveCheckboxStates()
        {
            var checkboxes = new Dictionary<string, Checkbox>
            {
                ["Edit"] = CheckboxEdit,
                ["Resize"] = CheckboxResize,
                ["Hitboxes"] = CheckboxHitboxes,
                ["Snap"] = CheckboxSnap,
                ["Text"] = CheckboxNames,
            };

            foreach (var (key, checkbox) in checkboxes)
            {
                if (checkbox != null)
                    _checkboxStates[key] = checkbox.state;
            }
        }

        // Helper method to get stored state
        private CheckboxState GetStoredState(string key) => _checkboxStates.TryGetValue(key, out var state) ? state : CheckboxState.Unchecked;

        public void ResetHideAllState()
        {
            hideAllMode = false;

            // Clean up any floating buttons
            EditorSystem sys = ModContent.GetInstance<EditorSystem>();
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

        public override void Populate()
        {
            list.Clear();
            list.SetPadding(2);

            if (hideAllMode)
            {
                var saveBtn = new Button("Save", () => "Save and exit edit mode",
                    onClick: () =>
                    {
                        ResetHideAllState();
                        EditorSystem.SetActiveFalse();
                    });
                saveBtn.Left.Set(_hideAllBtnPosition.X, 0);
                saveBtn.Top.Set(_hideAllBtnPosition.Y, 0);
                saveBtn.Width.Set(100, 0);
                saveBtn.Height.Set(30, 0);
                ModContent.GetInstance<EditorSystem>()?.state?.Append(saveBtn);
                return;
            }

            // Save current states before rebuilding
            SaveCheckboxStates();

            // One collapsible "UI Editor" section:
            list.Add(new CollapsibleSection(
                title: "Move UI",
                initialState: _uiEditorExpanded,
                buildContent: BuildUIEditor,
                onToggle: () => _uiEditorExpanded = !_uiEditorExpanded,
                contentHeight: () => 170f
            ));

            list.Add(new CollapsibleSection(
                title: "Set UI",
                initialState: _uiEditorExpanded,
                buildContent: BuildThemeEditor,
                onToggle: () => _uiEditorExpanded = !_uiEditorExpanded,
                contentHeight: () => 2 * 30f + 20
            ));
        }

        private void BuildThemeEditor(UIElement content)
        {
            string GetMapBtnText() =>
    $"[c/FFFFFF:Map:] [c/FFFF00:{Main.MinimapFrameManagerInstance.ActiveSelectionKeyName}]";


            string GetLifeBtn() =>
    $"[c/FFFFFF:Resource Bars:] [c/FFFF00:{Main.ResourceSetsManager.ActiveSet.DisplayedName}]";

            content.RemoveAllChildren();
            float y = 0;

            // Life button to cycle through resource sets
            var lifeBtn = new Button($"{GetLifeBtn()}",
                tooltip: () => { return ""; },
                onClick: Main.ResourceSetsManager.CycleResourceSet,
                onRightClick: () =>
                {
                    int currentIndex = Main.ResourceSetsManager.selectedSet;
                    int newIndex = (currentIndex - 1 + Main.ResourceSetsManager.accessKeys.Count) % Main.ResourceSetsManager.accessKeys.Count;
                    Main.ResourceSetsManager.SetActiveFrameFromIndex(newIndex);
                },
                maxWidth: true);

            lifeBtn.OnLeftClick += (_, _) =>
            {
                lifeBtn.UpdateButtonText(GetLifeBtn());
            };
            lifeBtn.OnRightClick += (_, _) =>
            {
                lifeBtn.UpdateButtonText(GetLifeBtn());
            };

            // Map button to cycle through map styles
            var mapBtn = new Button($"{GetMapBtnText()}",
            tooltip: () =>
            {
                return "";
            }, width: 100,
            onRightClick: Main.MinimapFrameManagerInstance.CycleSelection,
            onClick: () =>
            {
                string currentKey = Main.MinimapFrameManagerInstance.ActiveSelectionKeyName;
                List<string> options = Main.MinimapFrameManagerInstance.Options.Keys.ToList();
                int currentIndex = options.IndexOf(currentKey);
                int newIndex = (currentIndex + 1 + options.Count) % options.Count;
                Main.MinimapFrameManagerInstance.SetActiveFrame(options[newIndex]);
            },
            maxWidth: true
            );
            mapBtn.Top.Set(30, 0);
            mapBtn.OnLeftClick += (_, _) =>
            {
                mapBtn.UpdateButtonText(GetMapBtnText());
            };
            mapBtn.OnRightClick += (_, _) =>
            {
                mapBtn.UpdateButtonText(GetMapBtnText());
            };

            content.Append(mapBtn);
            content.Append(lifeBtn);
        }

        private void BuildUIEditor(UIElement content)
        {
            float y = 0;

            void AddRow(UIElement btn, Checkbox mid, Checkbox right)
            {
                btn.Top.Set(y, 0);
                content.Append(btn);

                if (mid != null)
                {
                    mid.Top.Set(y, 0);
                    mid.Left.Set(100, 0);
                    content.Append(mid);
                }

                if (right != null)
                {
                    right.Top.Set(y, 0);
                    right.Left.Set(195, 0);
                    content.Append(right);
                }

                y += 30;
            }

            // create & store checkboxes
            CheckboxHitboxes = new Checkbox("Hitboxes", "Show hitboxes", 100,
                () => UIEditorSettings.ShowHitboxes = CheckboxHitboxes.state == CheckboxState.Checked,
                GetStoredState("Hitboxes"));
            UIEditorSettings.ShowHitboxes = CheckboxHitboxes.state == CheckboxState.Checked;

            CheckboxSnap = new Checkbox("Snap", "Snap to edges when dragging", 80,
                () => UIEditorSettings.SnapToEdges = CheckboxSnap.state == CheckboxState.Checked,
                GetStoredState("Snap"));
            UIEditorSettings.SnapToEdges = CheckboxSnap.state == CheckboxState.Checked;

            CheckboxEdit = new Checkbox("Edit", "Enable edit mode", 80,
                () => EditorSystem.SetEditing(CheckboxEdit.state == CheckboxState.Checked),
                EditorSystem.IsEditing ? CheckboxState.Checked : CheckboxState.Unchecked);
            EditorSystem.SetEditing(CheckboxEdit.state == CheckboxState.Checked);

            CheckboxNames = new Checkbox("Names", "Show info text next to elements", 80,
                () => UIEditorSettings.ShowNames = CheckboxNames.state == CheckboxState.Checked,
                GetStoredState("Text"));
            UIEditorSettings.ShowNames = CheckboxNames.state == CheckboxState.Checked;

            CheckboxResize = new Checkbox("Fit", "Scales to fit elements when they resize. Keep off to see default bounds.", 80,
                () => UIEditorSettings.Fit = CheckboxResize.state == CheckboxState.Checked,
                GetStoredState("Resize"));
            UIEditorSettings.Fit = CheckboxResize.state == CheckboxState.Checked;

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
            var saveBtn = new Button("Save", () => "Save and exit edit mode", EditorSystem.SetActiveFalse);
            var resetBtn = new Button("Reset", () => "Reset all offsets", ResetAllOffsets);

            // add them two‐per‐row (updated with new checkboxes)
            AddRow(saveBtn, CheckboxEdit, CheckboxResize);
            AddRow(hideAllBtn, CheckboxHitboxes, CheckboxSnap);
            AddRow(resetBtn, CheckboxNames, null);
            //y += 30;

            // sliders
            var darkSlider = new SliderElement(
                "Dark",
                0f, 1f,
                EditorSystem.GetDarknessLevel(),
                EditorSystem.SetDarknessLevel,
                step: 0.01f,
                textScale: 0.35f,
                tip: "Darkness level",
                fmt: v => $"{v * 100:F0}%"
            );
            darkSlider.Top.Set(y, 0);
            darkSlider.Left.Set(0, 0);
            darkSlider.Width.Set(-20, 1f);
            content.Append(darkSlider);
            y += 30;

            var opacitySlider = new SliderElement(
                "Opacity",
                0f, 1f,
                UIEditorSettings.Opacity,
                (float val) => UIEditorSettings.Opacity = (val),
                step: 0.01f,
                textScale: 0.35f,
                tip: "Opacity level of hitboxes",
                fmt: v => $"{v * 100:F0}%"
            );
            opacitySlider.Top.Set(y, 0);
            opacitySlider.Left.Set(0, 0);
            opacitySlider.Width.Set(-20, 1f);
            content.Append(opacitySlider);
            //y += 30;
            //Gap(100);
        }

        private void ResetAllOffsets()
        {
            ChatHook.OffsetX = ChatHook.OffsetY = 0f;
            HotbarHook.OffsetX = HotbarHook.OffsetY = 0f;
            MapHook.OffsetX = MapHook.OffsetY = 0f;
            ClassicLifeHook.OffsetX = ClassicLifeHook.OffsetY = 0f;
            ClassicManaHook.OffsetX = ClassicManaHook.OffsetY = 0f;
            FancyLifeHook.OffsetX = FancyLifeHook.OffsetY = 0f;
            FancyManaHook.OffsetX = FancyManaHook.OffsetY = 0f;
            FancyLifeTextHook.OffsetX = FancyLifeTextHook.OffsetY = 0f;
            HorizontalBarsHook.OffsetX = HorizontalBarsHook.OffsetY = 0f;
            InfoAccsHook.OffsetX = InfoAccsHook.OffsetY = 0f;
            BuffHook.OffsetX = BuffHook.OffsetY = 0f;
            BarLifeTextHook.OffsetX = BarLifeTextHook.OffsetY = 0f;
            BarManaTextHook.OffsetX = BarManaTextHook.OffsetY = 0f;
            InventoryHook.OffsetX = InventoryHook.OffsetY = 0f;
            CraftingHook.OffsetX = CraftingHook.OffsetY = 0f;
            AccessoriesHook.OffsetX = AccessoriesHook.OffsetY = 0f;
            CraftWindowHook.OffsetX = CraftWindowHook.OffsetY = 0f;

            // Write to active layout
            LayoutHelper.SaveActiveLayout();
        }
    }
}