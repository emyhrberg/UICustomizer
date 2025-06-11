using Terraria.ModLoader;
using UICustomizer.Common.Systems;

namespace UICustomizer.Common.Commands
{
    public class LayersCommand : ModCommand
    {
        public override string Command => "layers";

        public override string Description => "Toggle layers, UIElements, and resource packs.";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            LayerSystem.ToggleActive();
        }
    }
}