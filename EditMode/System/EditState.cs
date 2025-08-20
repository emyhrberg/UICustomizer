using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace UICustomizer.EditMode.System
{
    public class EditorState : UIState
    {
        public EditorState()
        {
            editorPanel = new();
            Append(editorPanel);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
    }
}