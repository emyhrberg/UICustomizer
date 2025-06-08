using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using UICustomizer.Common.Systems;
using UICustomizer.Helpers;

namespace UICustomizer.UI.Layers
{
    public class LayersTab : Tab
    {
        private int _knownCount = -1; // forces an initial build
        public bool vanillaExpanded = true; // whether the Vanilla section is expanded
        public readonly Dictionary<string, bool> modsExpandedMap = [];

        public LayersTab() : base("Layers")
        {
        }

        private IEnumerable<string> GetModPrefixes()
        {
            return LayersSystem.LayerStates.Keys
                .Where(name => !name.StartsWith("Vanilla", StringComparison.OrdinalIgnoreCase) && name.Contains(':'))
                .Select(name => name.Split(':')[0])
                .Select(prefix => prefix == "BrickAndMortar" ? "DragonLens" : prefix) // Convert BrickAndMortar to DragonLens
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s, StringComparer.OrdinalIgnoreCase);
        }

        public override void Populate()
        {
            list.Clear();
            list.SetPadding(0);
            //Gap(8);

            // local helper to build one CollapsibleSection
            void BuildSection(string title, Func<string, bool> predicate)
            {
                // decide whether this section is expanded
                bool isVanilla = title.Equals("Vanilla", StringComparison.OrdinalIgnoreCase);
                bool isExpanded = isVanilla
                    ? vanillaExpanded
                    : modsExpandedMap.TryGetValue(title, out var v) ? v : (modsExpandedMap[title] = true);

                // translate “DragonLens” → “BrickAndMortar:” internally
                Func<string, bool> actualPredicate = title == "DragonLens"
                    ? name => name.StartsWith("BrickAndMortar:", StringComparison.OrdinalIgnoreCase)
                    : predicate;

                // dynamic height based on how many layers
                Func<float> contentHeight = () =>
                    Math.Max(80, LayersSystem.LayerStates.Count(kv => actualPredicate(kv.Key)) * 22 + 20);

                // here's the section, note the extra buildHeader param:
                var section = new CollapsibleSection(
                    title: title,
                    initialState: isExpanded,
                    buildContent: content =>
                    {
                        float y = 10;
                        foreach (var kv in LayersSystem.LayerStates
                                                 .Where(kv => actualPredicate(kv.Key))
                                                 .OrderBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase))
                        {
                            var chk = new Checkbox(kv.Key, hover: kv.Key.Length > 30 ? kv.Key : null, width: 300)
                            {
                                Top = { Pixels = y },
                                state = kv.Value ? CheckboxState.Checked : CheckboxState.Unchecked
                            };
                            chk.box.SetImage(chk.state == CheckboxState.Checked ? Ass.Check : Ass.Uncheck);
                            chk.OnLeftClick += (_, _) =>
                                LayersSystem.LayerStates[kv.Key] = chk.state == CheckboxState.Checked;
                            content.Append(chk);
                            y += 22;
                        }
                    },
                    onToggle: () =>
                    {
                        if (isVanilla) vanillaExpanded = !vanillaExpanded;
                        else modsExpandedMap[title] = !modsExpandedMap[title];
                    },
                    contentHeight: contentHeight,

                    // buildHeader: drop a checkbox into the header panel itself
                    buildHeader: header =>
                    {
                        // compute initial “all-on” state for this section
                        bool allOn = LayersSystem.LayerStates
                                         .Where(kv => actualPredicate(kv.Key))
                                         .All(kv => kv.Value);

                        // make a tiny checkbox up in the header
                        var toggle = new Checkbox("", hover: $"Toggle every {title} layer", width: 20)
                        {
                            HAlign = 1f,         // stick to right edge
                            VAlign = 0.5f,       // vertically centered
                            Left = { Pixels = -24 },  // nudge in from the right border
                            Top = { Pixels = -4 },  // nudge in from the right border
                            state = allOn ? CheckboxState.Checked : CheckboxState.Unchecked
                        };
                        toggle.box.SetImage(allOn ? Ass.Check : Ass.Uncheck);

                        // when clicked, flip ALL matching layers, then rebuild the UI
                        toggle.OnLeftClick += (evt, _) =>
                        {
                            if (evt.Target is Checkbox)
                            {
                                return;
                            }

                            bool newState = toggle.state == CheckboxState.Unchecked;
                            foreach (var key in LayersSystem.LayerStates.Keys
                                                         .Where(k => actualPredicate(k)))
                                LayersSystem.LayerStates[key] = newState;
                            // rebuild so every checkbox and our header‐checkbox text update
                            Populate();
                        };

                        header.Append(toggle);
                    }
                );

                list.Add(section);
                Gap(8);
            }

            var others = LayersSystem.LayerStates.Keys
                           .Where(n => !n.StartsWith("Vanilla: Hotbar", StringComparison.OrdinalIgnoreCase) && !n.Contains(':'))
                           .ToList();

            // Build the Vanilla section
            BuildSection(
                "Vanilla",
                name => name.StartsWith("Vanilla", StringComparison.OrdinalIgnoreCase)
            );

            // Build one section per mod‐prefix
            foreach (var prefix in GetModPrefixes())
            {
                BuildSection(
                    prefix,
                    name => name.StartsWith(prefix + ":", StringComparison.OrdinalIgnoreCase)
                );
            }

            // Build "Other Mods" for layers without a colon
            var others2 = LayersSystem.LayerStates.Keys
                           .Where(n => !n.StartsWith("Vanilla", StringComparison.OrdinalIgnoreCase) && !n.Contains(':'))
                           .ToList();
            if (others.Count > 0)
            {
                BuildSection(
                    "Other Mods",
                    name => !name.StartsWith("Vanilla", StringComparison.OrdinalIgnoreCase) && !name.Contains(':')
                );
            }

            // remember how many layers we saw, and force the UI to reflow
            _knownCount = LayersSystem.LayerStates.Count;
            list.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // If new layers have appeared or been removed, rebuild
            if (LayersSystem.LayerStates.Count != _knownCount)
            {
                Populate();
            }
        }
    }
}