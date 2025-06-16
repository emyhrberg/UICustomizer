using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Terraria;
using Terraria.Initializers;
using Terraria.IO;
using UICustomizer.Common.Systems;

namespace UICustomizer.UI.Layers
{
    public sealed class PacksTab : Tab
    {
        private bool _enabledPacksExpanded = true;
        private bool _disabledPacksExpanded = true;

        private List<ResourcePack> allPacks;
        private List<ResourcePack> enabledPacks;
        private List<ResourcePack> disabledPacks;

        public PacksTab() : base("Packs")
        {
            ResourcePackList _allPacksList = AssetInitializer.CreateResourcePackList(Main.instance.Services);
            allPacks = _allPacksList.AllPacks.ToList();
        }

        public override void Populate()
        {
            list.Clear();
            list.SetPadding(20);
            list.ListPadding = 0;
            list.Top.Set(-10, 0);
            list.Left.Set(-8, 0);

            // ensure allPacks is loaded
            if (allPacks == null)
            {
                var packList = AssetInitializer.CreateResourcePackList(Main.instance.Services);
                allPacks = packList?.AllPacks?.ToList() ?? new List<ResourcePack>();
            }

            // split into enabled/disabled
            enabledPacks = allPacks.Where(p => p.IsEnabled)
                                   .OrderBy(p => p.SortingOrder)
                                   .ThenBy(p => p.Name)
                                   .ToList();
            disabledPacks = allPacks.Where(p => !p.IsEnabled)
                                    .OrderBy(p => p.Name)
                                    .ToList();

            // helper to build each section
            void AddPackSection(string title, bool initial, Action<bool> setFlag, List<ResourcePack> source, bool enableOnClick)
            {
                CollapsibleSection section = null;
                section = new CollapsibleSection(
                    title,
                    initialState: initial,
                    contentHeightFunc: () => Math.Max(80f, source.Count * 30f + 20f),
                    buildContent: content =>
                    {
                        float y = 0;
                        foreach (var pack in source)
                        {
                            var btn = new Button(
                                text: pack.Name,
                                tooltip: () => enableOnClick
                                    ? $"Click to enable {pack.Name}"
                                    : $"Click to disable {pack.Name}",
                                onClick: () =>
                                {
                                    pack.IsEnabled = enableOnClick;
                                    Main.AssetSourceController.UseResourcePacks(
                                        new ResourcePackList(allPacks.Where(p => p.IsEnabled).ToList())
                                    );
                                    Populate();
                                },
                                onRightClick: () => Process.Start("explorer.exe", pack.FullPath),
                                maxWidth: true
                            );
                            btn.Top.Pixels = y;
                            content.Append(btn);
                            y += 30;
                        }
                    },
                    onToggle: () => setFlag(section.IsExpanded)
                );

                setFlag(section.IsExpanded);
                list.Add(section);
            }

            // Enabled packs section
            AddPackSection(
                title: "Enabled Packs",
                initial: _enabledPacksExpanded,
                setFlag: v => _enabledPacksExpanded = v,
                source: enabledPacks,
                enableOnClick: false // click disables
            );

            // Disabled packs section
            AddPackSection(
                title: "Disabled Packs",
                initial: _disabledPacksExpanded,
                setFlag: v => _disabledPacksExpanded = v,
                source: disabledPacks,
                enableOnClick: true  // click enables
            );

            list.Recalculate();
        }
    }
}
