using UICustomizer.Common.Systems;

namespace UICustomizer.Common.Commands
{
    public class LayerCommand : ModCommand
    {
        public override string Command => "layers";

        public override string Description => "Toggle visibility of layers.";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // Toggle active
            var sys = ModContent.GetInstance<UICustomizerSystem>();
            sys.uiCustomizerState.layerPanel.Active = !sys.uiCustomizerState.layerPanel.Active;
        }
    }
}