using System;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using UICustomizer.Common.Systems;

namespace UICustomizer.UI.Tabs
{
    public sealed class InterfaceLayersTab : Tab
    {
        private int _knownCount = -1; // forces an initial build

        public InterfaceLayersTab(Action<Tab> select, UIScrollbar bar = null)
            : base("Layers", select, bar)
        {
        }

        private void Build()
        {
            list.Clear();
            Gap(2);

            // 1) Vanilla group ─────────────────────────────────────────
            var vanillaHeader = new UIText("Vanilla", 0.5f, true)
            {
                HAlign = 0f
            };
            list.Add(vanillaHeader);

            var vanillaLayers = LayerToggleSystem.LayerStates
                .Where(kv => kv.Key.StartsWith("Vanilla", StringComparison.OrdinalIgnoreCase))
                .OrderBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase);

            foreach (var kv in vanillaLayers)
            {
                string layerName = kv.Key;
                bool visible = kv.Value;

                Checkbox check = null;
                check = new Checkbox(
                    text: layerName,
                    hover: "",
                    width: 400,
                    onClick: () =>
                    {
                        LayerToggleSystem.LayerStates[layerName] =
                            check.state == CheckboxState.Checked;
                    })
                {
                    state = visible ? CheckboxState.Checked : CheckboxState.Unchecked
                };

                TryAdd(check);
            }

            // 2) Mods group ────────────────────────────────────────────
            var modsHeader = new UIText("Mods", 0.5f, true)
            {
                HAlign = 0f,
                Top = { Pixels = 8 } // small space below the Vanilla section
            };
            Gap(8);
            TryAdd(modsHeader);

            var modLayers = LayerToggleSystem.LayerStates
                .Where(kv => !kv.Key.StartsWith("Vanilla", StringComparison.OrdinalIgnoreCase))
                .OrderBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase);

            foreach (var kv in modLayers)
            {
                string layerName = kv.Key;
                bool visible = kv.Value;

                Checkbox check = null;
                check = new Checkbox(
                    text: layerName,
                    hover: "",
                    width: 400,
                    onClick: () =>
                    {
                        LayerToggleSystem.LayerStates[layerName] =
                            check.state == CheckboxState.Checked;
                    })
                {
                    state = visible ? CheckboxState.Checked : CheckboxState.Unchecked
                };

                TryAdd(check);
            }

            // 3) Finalize the list ─────────────────────────────────────
            Gap(4);
            list.Recalculate();
            _knownCount = LayerToggleSystem.LayerStates.Count;

        }

        protected override void Populate() => Build(); // initial build

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (LayerToggleSystem.LayerStates.Count != _knownCount)
                Build();
        }
    }
}
