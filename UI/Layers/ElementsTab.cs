using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using UICustomizer.Common.States;
using UICustomizer.Helpers;

namespace UICustomizer.UI.Layers
{
    public sealed class ElementsTab : Tab
    {
        private readonly Dictionary<string, bool> _expandedSections = [];
        private int _knownModCount = -1;
        private int _knownTotalElementCount = -1;

        private readonly Dictionary<string, CheckboxEyeElement> _sectionToggleAllCheckboxes = [];

        public ElementsTab() : base("Elements") { }

        public override void Populate()
        {
            if (list == null)
            {
                Log.Info("List null.");
                return;
            }

            list.Clear();
            _sectionToggleAllCheckboxes.Clear();

            list.SetPadding(20);
            list.ListPadding = 2;
            list.Left.Set(-8, 0);
            list.Top.Set(-10, 0);

            if (UIElementDrawSystem.modElementMap == null || UIElementDrawSystem.modElementMap.Count == 0)
            {
                var noElementsText = new UIText("No UI elements registered yet.")
                {
                    HAlign = 0.5f,
                    VAlign = 0.5f
                };
                list.Add(noElementsText);
                UpdateKnownCounts();
                return;
            }

            // Ensure all known elements have a visibility state
            foreach (var modEntry in UIElementDrawSystem.modElementMap)
            {
                foreach (var elementName in modEntry.Value)
                {
                    if (!UIElementDrawSystem.elementVisibilityStates.ContainsKey(elementName))
                    {
                        UIElementDrawSystem.elementVisibilityStates[elementName] = true; // Default to visible
                    }
                }
            }

            PopulateModSections();
            UpdateKnownCounts();
        }

        private void PopulateModSections()
        {
            if (list == null || UIElementDrawSystem.modElementMap == null) return;

            var sortedModNames = UIElementDrawSystem.modElementMap.Keys.OrderBy(name => name, StringComparer.OrdinalIgnoreCase);

            foreach (string modName in sortedModNames)
            {
                if (UIElementDrawSystem.modElementMap.TryGetValue(modName, out var elementsInMod) && elementsInMod.Any())
                {
                    BuildModSection(modName, elementsInMod.ToList());
                }
            }
        }

        private void BuildModSection(string modName, List<string> elements)
        {
            if (list == null) return;

            const float elementCheckboxHeight = 25f;
            float contentHeight = Math.Max(20f, elements.Count * elementCheckboxHeight+20);

            if (!_expandedSections.ContainsKey(modName))
            {
                _expandedSections[modName] = false;
            }

            var section = new CollapsibleSection(
                title: modName,
                buildContent: contentList =>
                {
                    contentList.ListPadding = 0f;
                    foreach (var elementFullName in elements.OrderBy(name => name, StringComparer.OrdinalIgnoreCase))
                    {
                        string simpleElementName = elementFullName.Contains('.') ? elementFullName.Substring(elementFullName.LastIndexOf('.') + 1) : elementFullName;

                        bool currentVisibility = UIElementDrawSystem.elementVisibilityStates.TryGetValue(elementFullName, out bool vis) ? vis : true;

                        var chk = new CheckboxEyeElement(
                            text: simpleElementName,
                            initialState: currentVisibility,
                            onStateChanged: (newState) =>
                            {
                                UIElementDrawSystem.elementVisibilityStates[elementFullName] = newState;
                                UpdateSectionHeaderState(modName, elements);
                            },
                            tooltip: elementFullName,
                            maxWidth: true,
                            width: -2,
                            height: (int)elementCheckboxHeight - 2
                        )
                        {
                            Active = true,
                        };
                        contentList.Add(chk);
                    }
                },
                initialState: _expandedSections[modName],
                onToggle: () => _expandedSections[modName] = !_expandedSections[modName],
                contentHeightFunc: () => contentHeight,
                buildHeader: header =>
                {
                    SetupSectionHeaderControls(header, modName, elements);
                }
            );
            list.Add(section);
        }

        private void SetupSectionHeaderControls(UIElement header, string modName, List<string> elements)
        {
            // header.RemoveAllChildren(); // Clear previous controls if any

            int totalInSection = elements.Count;
            int enabledInSection = elements.Count(elName => UIElementDrawSystem.elementVisibilityStates.TryGetValue(elName, out bool vis) && vis);

            var countText = new UIText($"({enabledInSection}/{totalInSection})", 0.8f)
            {
                VAlign = 0.5f,
                HAlign = 1f,
                TextColor = Color.White,
                Left = { Pixels = -52, Percent = 0f }
            };
            header.Append(countText);

            bool allCurrentlyEnabled = totalInSection > 0 && enabledInSection == totalInSection;
            var toggleAllChk = new CheckboxEyeElement(
                text: "",
                initialState: allCurrentlyEnabled,
                onStateChanged: (newState) =>
                {
                    foreach (var elName in elements)
                    {
                        UIElementDrawSystem.elementVisibilityStates[elName] = newState;
                    }
                    Populate();
                },
                width: 22, height: 22, maxWidth: false,
                tooltip: $"Toggle all in {modName}",
                skipDrawPanel: true // For the header icon, skip panel drawing
            )
            {
                Active = true,
                HAlign = 1f,
                VAlign = 0.5f,
                Left = { Pixels = -50, Percent = 1f },
                Top = { Pixels = 0, Percent = 0f }
            };

            _sectionToggleAllCheckboxes[modName] = toggleAllChk; // Store reference
            header.Append(toggleAllChk);
        }

        // Call this when an individual checkbox state changes to update its section header
        private void UpdateSectionHeaderState(string modName, List<string> elements)
        {
            foreach (var uiElement in list._items) // Assuming 'list' is UIList
            {
                // if (uiElement is CollapsibleSection section && section.TitleText == modName)
                // {
                // Populate();
                // return;
                // }
            }
        }


        private void UpdateKnownCounts()
        {
            if (UIElementDrawSystem.modElementMap == null)
            {
                _knownModCount = 0;
                _knownTotalElementCount = 0;
                return;
            }
            _knownModCount = UIElementDrawSystem.modElementMap.Count;
            _knownTotalElementCount = UIElementDrawSystem.modElementMap.Sum(kvp => kvp.Value.Count);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (list == null) return;

            int currentModCount = 0;
            int currentTotalElementCount = 0;

            if (UIElementDrawSystem.modElementMap != null)
            {
                currentModCount = UIElementDrawSystem.modElementMap.Count;
                currentTotalElementCount = UIElementDrawSystem.modElementMap.Sum(kvp => kvp.Value.Count);
            }

            if (_knownModCount != currentModCount || _knownTotalElementCount != currentTotalElementCount)
            {
                // New elements might have been registered, ensure they have visibility states
                if (UIElementDrawSystem.modElementMap != null)
                {
                    foreach (var modEntry in UIElementDrawSystem.modElementMap)
                    {
                        foreach (var elementName in modEntry.Value)
                        {
                            if (!UIElementDrawSystem.elementVisibilityStates.ContainsKey(elementName))
                            {
                                UIElementDrawSystem.elementVisibilityStates[elementName] = true; // Default to visible
                            }
                        }
                    }
                }
                Populate();
            }
        }
    }
}