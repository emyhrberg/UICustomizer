using System;
using Terraria.GameContent.UI.Elements;
using UICustomizer.Common.Systems.Hooks;

namespace UICustomizer.UI.Tabs
{
    public sealed class LayoutsTab : Tab
    {
        public LayoutsTab(Action<Tab> select, UIScrollbar bar = null)
        : base("Layouts", select, bar)
        {

        }

        protected override void Populate()
        {
            Gap(2);
            list.Add(new Button("Default", "No offsets applied", 0, () => ApplyLayout("Default"), maxWidth: true));
            list.Add(new Button("HBCenter", "Hotbar centered", 0, () => ApplyLayout("HBCenter"), maxWidth: true));
        }

        private static void ApplyLayout(string layoutName)
        {
            if (layoutName == "Default") HotbarHook.OffsetX = HotbarHook.OffsetY = 0;
            else if (layoutName == "HBCenter") HotbarHook.OffsetX = 100;
        }
    }
}
