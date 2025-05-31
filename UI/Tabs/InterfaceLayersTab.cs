using System.Linq;
using UICustomizer.Common.Systems;

namespace UICustomizer.UI.Tabs
{
    public sealed class InterfaceLayersTab : Tab
    {
        public InterfaceLayersTab() : base("Layers")
        {

        }

        protected override void Populate()
        {
            Gap(2);
            foreach (var name in LayerToggleSystem.LayerStates.Keys.OrderBy(k => k))
            {
                Checkbox cb = null;
                cb = new Checkbox(name, "", () =>
                {
                    LayerToggleSystem.LayerStates[name] = cb.state == CheckboxState.Checked;
                });
                list.Add(cb);
            }
        }
    }
}
