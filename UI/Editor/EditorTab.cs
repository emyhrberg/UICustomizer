using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using UICustomizer.Common.Configs;
using UICustomizer.Common.Systems;
using UICustomizer.Helpers.Layouts;

namespace UICustomizer.UI.Editor
{
    public sealed class EditorTab : Tab
    {
        #region Checkboxes
        /// <summary>
        /// Each checkbox are added here and contains a state in the dictionary.
        /// A checkbox <see cref="CheckboxElement"/> is created for each of these.
        /// /// The <see cref="CheckboxElement"/> will update the state of in this dictionary when toggled.
        /// </summary>
        public enum Checkbox
        {
            EditMode,
            SnapToEdges,
            ShowHitboxes,
            FitBounds,
            ShowNames,
            ShowElementToggle,
        }

        public Dictionary<Checkbox, bool> checkboxStates = new()
        {
            [Checkbox.EditMode] = true,
            [Checkbox.SnapToEdges] = true,
            [Checkbox.ShowHitboxes] = true,
            [Checkbox.FitBounds] = true,
            [Checkbox.ShowNames] = true,
            [Checkbox.ShowElementToggle] = false
        };
        #endregion

        private readonly Dictionary<string, bool> _expandedSections = new(StringComparer.OrdinalIgnoreCase);

        public EditorTab() : base("UI")
        {

        }

        public override void Populate()
        {
            list.Clear();
            list.ListPadding = 20;
            list.SetPadding(20);
            list.Left.Set(-8, 0);
            list.Top.Set(-10, 0);

            bool moveExpanded = _expandedSections.TryGetValue("Move UI", out var mv) ? mv : (_expandedSections["Move UI"] = true);
            list.Add(new CollapsibleSection(
                title: "Move UI",
                initialState: moveExpanded,
                buildContent: BuildUIEditor,
                onToggle: () => _expandedSections["Move UI"] = !_expandedSections["Move UI"],
                contentHeightFunc: () => 250f
            ));

            bool setExpanded = _expandedSections.TryGetValue("Set Theme", out var sv) ? sv : (_expandedSections["Set Theme"] = true);
            list.Add(new CollapsibleSection(
                title: "Set Theme",
                initialState: setExpanded,
                buildContent: BuildThemeEditor,
                onToggle: () => _expandedSections["Set Theme"] = !_expandedSections["Set Theme"],
                contentHeightFunc: () => 90f
            ));
        }

        private void BuildUIEditor(UIList list)
        {
            PopulateButtons(list);
            PopulateCheckboxes(list);
            PopulateSliders(list);
        }

        private void PopulateButtons(UIList list)
        {
            string saveText = "Save";
            string resetText = "Reset";
            string hideModeText = "Hide All";

            var saveBtn = new Button(saveText, () =>
            {
                EditorSystem.SetEditing(false);
                EditorSystem.SetActive(false);

                if (!Conf.C.ShowCombatTextTooltips) return;
                CombatText.NewText(Main.LocalPlayer.getRect(), Color.Green, "UI Layout saved!");
            }, () => "Save and exit", width: 70);

            Button hideAllBtn = null;
            hideAllBtn = new Button(
                text: hideModeText,
                tooltip: () => "Hide everything except the save button",
                onClick: () =>
                {
                    SaveButtonOnlySystem.SetHideMode(true);
                    SaveButtonOnlySystem.SetSaveButtonPosition(hideAllBtn.GetDimensions().ToRectangle());
                },
                width: 70
            );

            var resetBtn = new Button(resetText, LayoutHelper.ResetAllOffsets, () => "Reset all offsets", width: 70);

            var row = new UIElement();
            row.Width.Set(0, 1);
            row.Height.Set(30, 0);

            List<Button> elements = [saveBtn, resetBtn, hideAllBtn];

            float columnWidth = 1f / 3;
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i] != null)
                {
                    elements[i].Left.Set(5, i * columnWidth);
                    elements[i].Width.Set(-10, columnWidth);
                    row.Append(elements[i]);
                }
            }

            list.Add(row);

            Main.QueueMainThreadAction(() =>
            {
                // Main.NewText(list.Width.Pixels);
            });
        }

        private void PopulateCheckboxes(UIList list)
        {
            var checkboxEdit = new CheckboxElement(
                text: "Edit Mode",
                initialState: checkboxStates[Checkbox.EditMode],
                onStateChanged: newState =>
                {
                    checkboxStates[Checkbox.EditMode] = newState;
                    EditorSystem.SetEditing(newState);
                },
                width: 100,
                tooltip: "Enable edit mode to be able to drag elements"
            )
            { Active = true };

            var checkboxSnap = new CheckboxElement(
                text: "Snap Edges",
                initialState: checkboxStates[Checkbox.SnapToEdges],
                onStateChanged: newState =>
                {
                    checkboxStates[Checkbox.SnapToEdges] = newState;
                    EditorTabSettings.SnapToEdges = newState;
                },
                width: 100,
                tooltip: "Clamps to the edges of the screen when dragging"
            )
            { Active = true };

            var checkboxHitboxes = new CheckboxElement(
                text: "Show Hitboxes",
                initialState: checkboxStates[Checkbox.ShowHitboxes],
                onStateChanged: newState =>
                {
                    checkboxStates[Checkbox.ShowHitboxes] = newState;
                    EditorTabSettings.ShowHitboxes = newState;
                },
                width: 100,
                tooltip: "Show hitboxes of all elements"
            )
            { Active = true };

            var checkboxFitBounds = new CheckboxElement(
                text: "Fit Bounds",
                initialState: checkboxStates[Checkbox.FitBounds],
                onStateChanged: newState =>
                {
                    checkboxStates[Checkbox.FitBounds] = newState;
                    EditorTabSettings.FitBounds = newState;
                },
                width: 100,
                tooltip: "Dynamically adjust hitbox bounds when resizing"
            )
            { Active = true };

            var checkboxNames = new CheckboxElement(
                text: "Names",
                initialState: checkboxStates[Checkbox.ShowNames],
                onStateChanged: newState =>
                {
                    checkboxStates[Checkbox.ShowNames] = newState;
                    EditorTabSettings.ShowNames = newState;
                },
                width: 100,
                tooltip: "Show the names of elements"
            )
            { Active = true };

            var checkboxEyeToggle = new CheckboxElement(
                text: "Eye Toggle",
                initialState: checkboxStates[Checkbox.ShowElementToggle],
                onStateChanged: newState =>
                {
                    checkboxStates[Checkbox.ShowElementToggle] = newState;
                    EditorTabSettings.ShowEyeToggle = newState;
                },
                width: 100,
                tooltip: "Show eye icon to toggle visibility"
            )
            { Active = true };

            AddRow(list, checkboxEdit, checkboxSnap);
            AddRow(list, checkboxFitBounds, checkboxNames);
            AddRow(list, checkboxHitboxes, checkboxEyeToggle);
        }

        private static void AddRow(UIList list, params UIElement[] elements)
        {
            var row = new UIElement
            {
                Width = { Percent = 1f },
                Height = { Pixels = 30f }
            };

            float columnWidth = 1f / elements.Length;
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i] != null)
                {
                    elements[i].Left.Set(0, i * columnWidth);
                    elements[i].Width.Set(0, columnWidth);
                    row.Append(elements[i]);
                }
            }

            list.Add(row);
        }
        private static void PopulateSliders(UIList list)
        {
            // UI Scale slider: Apply on release, show live and applied values
            var uiSlider = new ZenSliderElement(
                "UI Scale",
                "Changes the main scale of all UI",
                0.5f, 2.0f, Main.UIScale, 0.01f,
                (val) =>
                {
                    Main.UIScale = val;
                    Main.temporaryGUIScaleSlider = val;
                },
                applyOnRelease: true
            );
            list.Add(uiSlider);

            // Snap Threshold slider: Normal behavior (always update)
            var snapThreshold = new ZenSliderElement(
                "Snap",
                "Snap to edges threshold",
                0f, 100f, EditorTabSettings.SnapThreshold, 1f,
                (val) => EditorTabSettings.SnapThreshold = (int)val,
                applyOnRelease: false
            );
            list.Add(snapThreshold);

            // Snap Threshold slider: Normal behavior (always update)
            var darkSlider = new ZenSliderElement(
                "Dark",
                "Set darkness level to see elements easier",
                0f, 100f, 
                defaultValue: DarkSystem.GetDarknessLevel(), 
                step: 1f,
                onValueChanged: DarkSystem.SetDarknessLevel,
                applyOnRelease: false
            );
            list.Add(darkSlider);
        }

        private void BuildThemeEditor(UIList list)
        {
            string GetMapBtnText() => $"[c/FFFFFF:Map:] [c/FFFF00:{Main.MinimapFrameManagerInstance.ActiveSelectionKeyName}]";
            string GetLifeBtn() => $"[c/FFFFFF:Resource Bars:] [c/FFFF00:{Main.ResourceSetsManager.ActiveSet.DisplayedName}]";

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
                width: 0, maxWidth: true);

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
            tooltip: () => { return ""; },
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

            mapBtn.OnLeftClick += (_, _) =>
            {
                mapBtn.UpdateButtonText(GetMapBtnText());
            };
            mapBtn.OnRightClick += (_, _) =>
            {
                mapBtn.UpdateButtonText(GetMapBtnText());
            };

            UIElement gap = new();
            gap.Top.Set(10, 0);

            list.Add(gap);
            list.Add(mapBtn);
            list.Add(lifeBtn);
        }
    }
}