using UICustomizer.Common.Systems;

namespace UICustomizer.Common.Commands
{
    public class EditCommand : ModCommand
    {
        public override string Command => "edit";

        public override string Description => "Edit and save new position of all UI.";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // Enter "edit mode"
            if (UICustomizerSystem.EditModeActive)
            {
                UICustomizerSystem.ExitEditMode();
            }
            else
            {
                UICustomizerSystem.EnterEditMode();
            }
        }
    }
}