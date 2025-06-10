using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using UICustomizer.UI.Editor;
using UICustomizer.UI.Layers;

namespace UICustomizer.Common.States
{
    public class EditorState : UIState
    {
        public EditorPanel editorPanel;

        public EditorState()
        {
            // The entire panel that contains all the UI elements for editing.
            editorPanel = new();
            Append(editorPanel);

            // The edit button next to settings button.
            EditButton editButton = new();
            Append(editButton);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
    }
}