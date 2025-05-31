using UICustomizer.Common.Systems;

namespace UICustomizer.Common.Commands
{
    public class HealthStyleCommand : ModCommand
    {
        public override string Command => "hst";

        public override string Description => "Toggle health style.";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Main.ResourceSetsManager.CycleResourceSet();
            Main.NewText("New health style: " + Main.ResourceSetsManager.ActiveSet.DisplayedName, Color.Green);
        }
    }
}