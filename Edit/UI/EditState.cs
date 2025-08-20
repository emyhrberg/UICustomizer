using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace UICustomizer.Edit.UI
{
    public class EditorState : UIState
    {
        public EditPanel editorPanel;
        public EditorState()
        {
            // The entire colorPanel that contains all the UI elements for editing.
            editorPanel = new();
            Append(editorPanel);
        }

        public override void Draw(SpriteBatch sb)
        {
            // Debug draw mouse pos 
            //Rectangle rect3 = new(0,0,800,800);
            //Rectangle rect3 = new((int)Main.mouseX, (int)Main.mouseY, 100, 100);
            //sb.Draw(TextureAssets.MagicPixel.Value, rect3, OutlineColor.Red);

            base.Draw(sb);
        }
    }
}