using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using UICustomizer.Common.Systems;
using UICustomizer.Helpers;

namespace UICustomizer.UI.Layers
{
    public class LayersTab : Tab
    {
        private int _knownCount = -1;
        private readonly Dictionary<string, bool> expandedSections = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, ToggleAllEyeElement> _sectionToggleAllCheckboxes = [];

        public LayersTab() : base("Layers") { }

        private IEnumerable<string> GetModPrefixes()
        {
            if (LayerSystem.LayerStates == null)
            {
                ModContent.GetInstance<UICustomizer>().Logger.Warn($"{nameof(LayerSystem.LayerStates)} is null in {nameof(GetModPrefixes)}. Returning empty list.");
                return Enumerable.Empty<string>();
            }
            return LayerSystem.LayerStates.Keys
               .Where(n => !n.StartsWith("Vanilla", StringComparison.OrdinalIgnoreCase) && n.Contains(':'))
               .Select(n => n.Split(':')[0] switch { "BrickAndMortar" => "DragonLens", var p => p })
               .Distinct(StringComparer.OrdinalIgnoreCase)
               .OrderBy(p => p, StringComparer.OrdinalIgnoreCase);
        }

        public override void Populate()
        {
            if (LayerSystem.LayerStates == null)
            {
                Log.Error("Layers is null. LayersTab cannot populate.");
                list.Clear(); // Clear any existing items
                _knownCount = -1;
                return;
            }

            // Debug layer info
            int vanillaCount = LayerSystem.LayerStates
                .Where(kv => kv.Key.StartsWith("Vanilla", StringComparison.OrdinalIgnoreCase))
                .Select(kv => $"{kv.Key}: {kv.Value}")
                .ToList().Count;

            int nonVanillaCount = LayerSystem.LayerStates
                .Where(kv => !kv.Key.StartsWith("Vanilla", StringComparison.OrdinalIgnoreCase))
                .Select(kv => $"{kv.Key}: {kv.Value}")
                .ToList().Count;

            Log.Info("Found " + vanillaCount + " vanilla layers and " + nonVanillaCount + " non-vanilla layers.");

            list.Clear();
            list.SetPadding(20);
            list.ListPadding = 0;
            list.Left.Set(-8, 0);
            list.Top.Set(-10, 0);

            BuildSection("Vanilla", n => n.StartsWith("Vanilla", StringComparison.OrdinalIgnoreCase));
            foreach (var prefix in GetModPrefixes())
                BuildSection(prefix, n => n.StartsWith(prefix + ":", StringComparison.OrdinalIgnoreCase));

            var others = LayerSystem.LayerStates.Keys
                .Where(n => !n.StartsWith("Vanilla", StringComparison.OrdinalIgnoreCase) && !n.Contains(':'))
                .ToList();
            if (others.Count > 0)
                BuildSection("Other Mods", n => !n.StartsWith("Vanilla", StringComparison.OrdinalIgnoreCase) && !n.Contains(':'));

            _knownCount = LayerSystem.LayerStates.Count;
            list.Recalculate();
        }

        private void BuildSection(string title, Func<string, bool> predicate)
        {
            bool isExpanded = expandedSections.TryGetValue(title, out var v) ? v : (expandedSections[title] = false);
            Func<string, bool> actualPred = title == "DragonLens"
                ? n => n.StartsWith("BrickAndMortar:", StringComparison.OrdinalIgnoreCase)
                : predicate;
            Func<float> height = () =>
            {
                var count = LayerSystem.LayerStates.Count(kv => actualPred(kv.Key));
                return count == 1
                    ? 35
                    : Math.Max(80, count * 25) + 10;
            };

            var section = new CollapsibleSection(
                title,
                content =>
                {
                    foreach (var kv in LayerSystem.LayerStates.Where(kv => actualPred(kv.Key)).OrderBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase))
                    {
                        var chk = new CheckboxEyeElement(
                            kv.Key,
                            kv.Value,
                            newState => LayerSystem.LayerStates[kv.Key] = newState,
                            width: -5,
                            tooltip: kv.Key.Length > 30 ? kv.Key : null,
                            maxWidth: true,
                            height: 23
                        );
                        content.Add(chk);
                    }
                },
                isExpanded,
                onToggle: () => expandedSections[title] = !expandedSections[title],
                contentHeightFunc: height,
                buildHeader: header =>
                {
                    var relevant = LayerSystem.LayerStates.Where(kv => actualPred(kv.Key)).ToList();
                    int total = relevant.Count;
                    int enabled = relevant.Count(kv => kv.Value);

                    var countText = new UIText($"({enabled}/{total})", 0.3f, true)
                    {
                        VAlign = 0.5f,
                        HAlign = 1f
                    };
                    countText.Left.Set(-49, 0);
                    countText.Top.Set(0, 0);
                    header.Append(countText);

                    bool allOn = relevant.All(kv => kv.Value);
                    var toggleAllEye = new ToggleAllEyeElement(Ass.EyeOpen);
                    toggleAllEye.SetState(allOn);          // ← sync internal flag + image
                    toggleAllEye.OnToggle += newState =>
                    {
                        foreach (var key in LayerSystem.LayerStates.Keys.Where(actualPred))
                            LayerSystem.LayerStates[key] = newState;
                        Populate();
                    };
                    _sectionToggleAllCheckboxes[title] = toggleAllEye;
                    header.Append(toggleAllEye);
                }
            );
            section.SetPadding(0);      // remove the panel’s default 10px on all sides
            //section.ListPadding = 0f;
            list.SetPadding(20f);
            list.Add(section);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (_knownCount != LayerSystem.LayerStates.Count)
            {
                Populate();
            }
        }
    }
}
