using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using UICustomizer.UI.Editor;

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
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
    }
}