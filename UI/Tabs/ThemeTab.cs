using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.GameContent.UI.States;
using Terraria.Initializers;
using Terraria.IO;
using UICustomizer.Common.Systems;

namespace UICustomizer.UI.Tabs
{
    public sealed class ThemeTab : Tab
    {
        private bool _enabledPacksExpanded = true;
        private bool _disabledPacksExpanded = true;

        private ResourcePackList _allPacks;

        public ThemeTab(Action<Tab> select, Scrollbar bar) : base("Packs", select, bar)
        {
        }
        protected override void Populate()
        {
            list.Clear();

            // Initialize the resource pack list
            _allPacks = AssetInitializer.CreateResourcePackList(Main.instance.Services);

            // Log them
            Log.Info($"Enabled/Disabled Packs: {_allPacks.EnabledPacks.Count()} / {_allPacks.DisabledPacks.Count()}");

            AddCollapsibleHeader(
                text: "Enabled Packs",
                getState: () => _enabledPacksExpanded,
                setState: (state) => _enabledPacksExpanded = state,
                onToggle: () => { } // Remove the callback to avoid double population
            );

            PopulateEnabledPacks();

            AddCollapsibleHeader(
                text: "Disabled Packs",
                getState: () => _disabledPacksExpanded,
                setState: (state) => _disabledPacksExpanded = state,
                onToggle: () => { } // Remove the callback to avoid double population
            );

            PopulateDisabledPacks(); // Fix: was "popu"
        }

        private void PopulateEnabledPacks()
        {
            if (!_enabledPacksExpanded) return;

            var enabledPacks = _allPacks.EnabledPacks.ToList();

            foreach (var pack in enabledPacks)
            {
                // If it's currently enabled, the user wants to disable it:
                string status = pack.IsEnabled ? "disable" : "enable";

                var btn = new Button(
                    text: pack.Name,
                    tooltip: () => $"Click to {status} {pack.Name} resource pack",
                    onClick: () =>
                    {
                        pack.IsEnabled = !pack.IsEnabled;

                        // Recreate the resource pack list
                        // _allPacks = AssetInitializer.CreateResourcePackList(Main.instance.Services);

                        Main.AssetSourceController.UseResourcePacks(_allPacks);
                        Main.NewText($"{pack.Name} {status}d.", pack.IsEnabled ? Color.Green : Color.Red);
                        Populate();
                    },
                    maxWidth: true
                );

                TryAdd(btn);
            }
        }

        private void PopulateDisabledPacks()
        {
            if (!_disabledPacksExpanded) return;

            var disabledPacks = _allPacks.DisabledPacks.ToList();

            foreach (var pack in disabledPacks)
            {
                // If it's disabled, user wants to enable it:
                string status = pack.IsEnabled ? "disable" : "enable";

                var btn = new Button(
                    text: pack.Name,
                    tooltip: () => $"Click to {status} {pack.Name} resource pack",
                    onClick: () =>
                    {
                        pack.IsEnabled = !pack.IsEnabled;
                        // _allPacks = AssetInitializer.CreateResourcePackList(Main.instance.Services);

                        Main.AssetSourceController.UseResourcePacks(_allPacks);
                        Main.NewText($"{pack.Name} resource pack {status}d.", pack.IsEnabled ? Color.Green : Color.Red);
                        Populate();
                    },
                    maxWidth: true
                );

                TryAdd(btn);
            }
        }
    }
}
