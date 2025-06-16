using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using UICustomizer.UI.EditMode;

namespace UICustomizer.Common.States
{
    public class EditModeState : UIState
    {
        //public EditorPanel editorPanel;

        public EditModeState()
        {
            // The entire panel that contains all the UI elements for editing.


            // The "Edit Mode" label
            EditModeLabel editModeLabel = new("Edit Mode", 1.1f, false);
            editModeLabel.VAlign = 0.5f;
            editModeLabel.HAlign = 0.5f;
            Append(editModeLabel);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
    }
}