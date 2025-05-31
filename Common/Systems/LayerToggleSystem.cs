using System.Collections.Generic;

namespace UICustomizer.Common.Systems
{
    public class LayerToggleSystem : ModSystem
    {
        internal static readonly Dictionary<string, bool> LayerStates = [];

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // build dictionary the first time or when new layers appear
            foreach (var l in layers)
                if (!LayerStates.ContainsKey(l.Name))
                    LayerStates[l.Name] = true; // default ON

            // apply user choices (never crash if something disappeared)
            foreach (var l in layers)
                if (LayerStates.TryGetValue(l.Name, out bool show) && !show)
                    l.Active = false;
        }
    }
}
