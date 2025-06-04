using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Initializers;
using Terraria.IO;

namespace UICustomizer.UI.Tabs
{
    public sealed class ThemeTab : Tab
    {
        private bool _enabledPacksExpanded = true;
        private bool _disabledPacksExpanded = true;

        private List<ResourcePack> allPacks;
        private List<ResourcePack> enabledPacks;
        private List<ResourcePack> disabledPacks;

        public ThemeTab(Action<Tab> select, Scrollbar bar) : base("Packs", select, bar)
        {
            ResourcePackList _allPacksList = AssetInitializer.CreateResourcePackList(Main.instance.Services);
            allPacks = _allPacksList.AllPacks.ToList();
        }

        protected override void Populate()
        {
            list.Clear();

            if (allPacks == null)
            {
                ResourcePackList packList = AssetInitializer.CreateResourcePackList(Main.instance.Services);
                if (packList.AllPacks != null)
                {
                    allPacks = packList.AllPacks.ToList();
                }
                else
                {
                    allPacks = [];
                }
                allPacks = packList.AllPacks?.ToList() ?? [];
            }

            list.Clear();

            enabledPacks = allPacks
                .Where(pack => pack.IsEnabled)
                .OrderBy(pack => pack.SortingOrder)
                .ThenBy(pack => pack.Name)
                .ThenBy(pack => pack.Version)
                .ThenBy(pack => pack.FileName)
                .ToList();

            disabledPacks = allPacks
                .Where(pack => !pack.IsEnabled)
                .OrderBy(pack => pack.Name)
                .ThenBy(pack => pack.Version)
                .ThenBy(pack => pack.FileName)
                .ToList();

            Log.Info($"Enabled Packs: {enabledPacks.Count} / {allPacks.Count}");
            // Main.NewText($"Enabled Packs: {enabledPacks.Count} / {allPacks.Count}", Color.LightGreen);

            AddCollapsibleHeader(
                text: "Enabled Packs",
                getState: () => _enabledPacksExpanded,
                setState: (v) => _enabledPacksExpanded = v,
                onToggle: () => { /* no extra action needed—Populate() always rebuilds */ }
            );
            PopulateEnabledPacks();

            AddCollapsibleHeader(
                text: "Disabled Packs",
                getState: () => _disabledPacksExpanded,
                setState: (v) => _disabledPacksExpanded = v,
                onToggle: () => { /* no extra action needed—Populate() always rebuilds */ }
            );
            PopulateDisabledPacks();
        }

        private void PopulateEnabledPacks()
        {
            if (!_enabledPacksExpanded) return;

            // Snapshot so we can mutate enabledPacks inside the click handler:
            var snapshot = enabledPacks.ToList();

            foreach (var pack in snapshot)
            {
                var btn = new Button(
                    text: pack.Name,
                    tooltip: () => $"Click to disable {pack.Name} resource pack",
                    onClick: () =>
                    {
                        pack.IsEnabled = false;

                        enabledPacks.Remove(pack);
                        disabledPacks.Add(pack);
                        Main.AssetSourceController.UseResourcePacks(new ResourcePackList(enabledPacks));

                        Main.NewText($"{pack.Name} disabled.", Color.Red);
                        Populate();
                    },
                    maxWidth: true
                );

                TryAdd(btn);
            }
        }

        private void PopulateDisabledPacks()
        {
            if (!_disabledPacksExpanded)
                return;

            var snapshot = disabledPacks.ToList();

            foreach (var pack in snapshot)
            {
                var btn = new Button(
                    text: pack.Name,
                    tooltip: () => $"Click to enable {pack.Name} resource pack",
                    onClick: () =>
                    {
                        pack.IsEnabled = true;
                        disabledPacks.Remove(pack);
                        enabledPacks.Add(pack);
                        Main.AssetSourceController.UseResourcePacks(new ResourcePackList(enabledPacks));

                        Main.NewText($"{pack.Name} enabled.", Color.Green);
                        Populate();
                    },
                    maxWidth: true
                );

                TryAdd(btn);
            }
        }
    }
}
