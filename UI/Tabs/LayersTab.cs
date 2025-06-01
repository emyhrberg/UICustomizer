using System;
using System.Collections.Generic;
using System.Linq;
using UICustomizer.Common.Systems;
using static UICustomizer.UI.CollapsibleHeader;

namespace UICustomizer.UI.Tabs
{
    public sealed class LayersTab : Tab
    {
        private int _knownCount = -1; // forces an initial build
        private bool vanillaExpanded = true; // whether the Vanilla section is expanded
        private readonly Dictionary<string, bool> modsExpandedMap = new();

        public LayersTab(Action<Tab> select, Scrollbar bar)
            : base("Layers", select, bar)
        {
        }

        private void Build()
        {
            list.Clear();
            Gap(2);
            AddSection("Vanilla", name => name.StartsWith("Vanilla", StringComparison.OrdinalIgnoreCase));
            Gap(8);

            foreach (var modPrefix in GetModPrefixes())
            {
                AddSection(modPrefix, name => name.StartsWith(modPrefix + ":", StringComparison.OrdinalIgnoreCase));
                Gap(8);
            }

            // Handle layers without colons (mod names without prefixes)
            var layersWithoutColons = LayerToggleSystem.LayerStates.Keys
                .Where(name => !name.StartsWith("Vanilla", StringComparison.OrdinalIgnoreCase) && !name.Contains(':'))
                .ToList();

            if (layersWithoutColons.Any())
            {
                AddSection("Mods", name => !name.StartsWith("Vanilla", StringComparison.OrdinalIgnoreCase) && !name.Contains(':'));
                Gap(8);
            }

            Gap(4);
            list.Recalculate();
            _knownCount = LayerToggleSystem.LayerStates.Count;
        }

        private IEnumerable<string> GetModPrefixes()
        {
            return LayerToggleSystem.LayerStates.Keys
                .Where(name => !name.StartsWith("Vanilla", StringComparison.OrdinalIgnoreCase) && name.Contains(':'))
                .Select(name => name.Split(':')[0])
                .Select(prefix => prefix == "BrickAndMortar" ? "DragonLens" : prefix) // Convert BrickAndMortar to DragonLens
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s, StringComparer.OrdinalIgnoreCase);
        }

        private void AddSection(string headerText, Func<string, bool> matchPredicate)
        {
            var headerRow = new UIElement { Width = { Percent = 1f }, Height = { Pixels = 24 } };
            bool isVanilla = headerText.Equals("Vanilla", StringComparison.OrdinalIgnoreCase);
            bool isExpanded = isVanilla ? vanillaExpanded :
                (modsExpandedMap.TryGetValue(headerText, out var value) ? value : (modsExpandedMap[headerText] = true));

            var sectionLabel = new CollapsibleHeader(
                text: headerText,
                initialState: isExpanded ? CollapseState.Expanded : CollapseState.Collapsed,
                onClick: () =>
                {
                    if (isVanilla) vanillaExpanded = !vanillaExpanded;
                    else modsExpandedMap[headerText] = !modsExpandedMap[headerText];
                    Build();
                },
                hoverText: () => $"Click to {(isExpanded ? "collapse" : "expand")} the {headerText} section",
                large: true
            )
            { HAlign = 0.2f, VAlign = 0.5f };

            headerRow.Append(sectionLabel);

            var toggleAll = new Checkbox("All", $"Toggle all {headerText} layers")
            { HAlign = 0.8f, VAlign = 0.5f };
            toggleAll.OnMouseOver += (_, _) => ModContent.GetInstance<UICustomizerSystem>().state.panel.CancelDrag();

            // Update the predicate for DragonLens to match BrickAndMortar layers
            Func<string, bool> actualPredicate = headerText == "DragonLens"
                ? name => name.StartsWith("BrickAndMortar:", StringComparison.OrdinalIgnoreCase)
                : matchPredicate;

            bool allChecked = LayerToggleSystem.LayerStates
                .Where(kv => actualPredicate(kv.Key))
                .All(kv => kv.Value);
            toggleAll.state = allChecked ? CheckboxState.Checked : CheckboxState.Unchecked;
            toggleAll.box.SetImage(allChecked ? Ass.Check : Ass.Uncheck);

            toggleAll.OnLeftClick += (_, _) =>
            {
                ModContent.GetInstance<UICustomizerSystem>().state.panel.CancelDrag();
                bool anyUnchecked = LayerToggleSystem.LayerStates
                    .Where(kv => actualPredicate(kv.Key))
                    .Any(kv => !kv.Value);
                foreach (var key in LayerToggleSystem.LayerStates.Keys
                         .Where(name => actualPredicate(name))
                         .ToList())
                    LayerToggleSystem.LayerStates[key] = anyUnchecked;
                toggleAll.state = anyUnchecked ? CheckboxState.Checked : CheckboxState.Unchecked;
                toggleAll.box.SetImage(anyUnchecked ? Ass.Check : Ass.Uncheck);
                Build();
            };

            headerRow.Append(toggleAll);
            list.Add(headerRow);

            if (isExpanded)
            {
                var matchingLayers = LayerToggleSystem.LayerStates
                    .Where(kv => actualPredicate(kv.Key))
                    .OrderBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase);

                foreach (var kv in matchingLayers)
                {
                    var layerName = kv.Key;
                    var visible = kv.Value;
                    Checkbox check = null;
                    check = new Checkbox(layerName, "", width: 400, onClick: () =>
                    {
                        LayerToggleSystem.LayerStates[layerName] = check.state == CheckboxState.Checked;
                    })
                    { state = visible ? CheckboxState.Checked : CheckboxState.Unchecked };
                    check.box.SetImage(check.state == CheckboxState.Checked ? Ass.Check : Ass.Uncheck);
                    list.Add(check);
                }
            }
        }

        protected override void Populate() => Build();

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // If new layers have appeared or been removed, rebuild
            if (LayerToggleSystem.LayerStates.Count != _knownCount)
            {
                Build();
            }
        }
    }
}
