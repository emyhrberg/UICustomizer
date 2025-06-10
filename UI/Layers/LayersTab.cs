using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using UICustomizer.Common.Systems;
using UICustomizer.Helpers;

namespace UICustomizer.UI.Layers
{
    public class LayersTab : Tab
    {
        private int _knownCount = -1;
        private readonly Dictionary<string, bool> expandedSections = new(StringComparer.OrdinalIgnoreCase);

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
            list.ListPadding = 2;
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
            bool isExpanded = expandedSections.TryGetValue(title, out var v)? v : (expandedSections[title] = false);

            Func<string, bool> actualPred = title == "DragonLens"
                ? n => n.StartsWith("BrickAndMortar:", StringComparison.OrdinalIgnoreCase)
                : predicate;

            Func<float> height = () =>
                Math.Max(80, LayerSystem.LayerStates.Count(kv => actualPred(kv.Key)) * 22) + 10;

            var section = new CollapsibleSection(
                title,
                content =>
                {
                    foreach (var kv in LayerSystem.LayerStates.Where(kv => actualPred(kv.Key)).OrderBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase))
                    {
                        var chk = new CheckboxElement(
                            kv.Key,
                            kv.Value,
                            newState => LayerSystem.LayerStates[kv.Key] = newState,
                            width: 0,
                            tooltip: kv.Key.Length > 30 ? kv.Key : null,
                            eye: true,
                            maxWidth: true,
                            height: 20
                        )
                        { Active = true };
                        content.Add(chk);
                    }
                },
                isExpanded,
                onToggle: () => expandedSections[title] = !expandedSections[title],
                contentHeightFunc: height,
                buildHeader: header =>
                {
                    bool allOn = LayerSystem.LayerStates
                        .Where(kv => actualPred(kv.Key))
                        .All(kv => kv.Value);

                    var toggle = new CheckboxElement(
                        "",
                        allOn,
                        newState =>
                        {
                            foreach (var key in LayerSystem.LayerStates.Keys.Where(k => actualPred(k)))
                                LayerSystem.LayerStates[key] = newState;
                            Populate();
                        },
                        width: 20,
                        tooltip: $"Toggle every {title} layer",
                        eye: true
                    )
                    {
                        Active = true,
                        HAlign = 1f,
                        VAlign = 0.5f
                    };
                    toggle.Left.Set(-24, 0);
                    toggle.Top.Set(-4, 0);
                    header.Append(toggle);
                }
            );
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
