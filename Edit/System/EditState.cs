using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using UICustomizer.EditMode.UI;

namespace UICustomizer.EditMode.System
{
    public class EditState : UIState
    {
        public EditToggleButton toggleButton;
        public EditPanel panel;

        public EditState()
        {
            // Add the button below the inventory
            toggleButton = new();
            Append(toggleButton);

            // Add the main panel
            panel = new();
            Append(panel);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
