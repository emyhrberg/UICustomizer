using UICustomizer.Common.Systems.Hooks;

namespace UICustomizer.UI.Tabs
{
    public sealed class LayoutsTab : Tab
    {
        public LayoutsTab() : base("Layouts")
        {

        }

        protected override void Populate()
        {
            Gap(2);
            list.Add(new Button("Default", "No offsets applied", 0, () => ApplyLayout("Default")));
            list.Add(new Button("HBCenter", "Hotbar centered", 0, () => ApplyLayout("HBCenter")));
        }

        private static void ApplyLayout(string layoutName)
        {
            if (layoutName == "Default") HotbarHook.OffsetX = HotbarHook.OffsetY = 0;
            else if (layoutName == "HBCenter") HotbarHook.OffsetX = 100;
        }
    }
}
