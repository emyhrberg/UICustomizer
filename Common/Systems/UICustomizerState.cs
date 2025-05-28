using Microsoft.Xna.Framework.Graphics;
using UICustomizer.UI;

namespace UICustomizer.Common.Systems
{
    public class UICustomizerState : UIState
    {
        public SaveButton saveButton;
        public ResetButton cancelButton;
        public EditButton editButton;
        public DisplayPanel displayPanel;

        public UICustomizerState()
        {
            // Initialize UI components here
            saveButton = new();
            Append(saveButton);

            cancelButton = new();
            Append(cancelButton);

            editButton = new();
            editButton.Active = true;
            Append(editButton);

            displayPanel = new();
            displayPanel.Active = true;
            Append(displayPanel);
        }

        // Draw black background with lower opacity to show we're in edit mode.
        // TODO: Exclude all UI that can be moved from this black drawing.
        // Reference:
        // https://github.com/ScalarVector1/DragonLens/blob/master/Content/GUI/ToolbarState.cs#L132
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (UICustomizerSystem.EditModeActive)
            {
                Texture2D tex = Terraria.GameContent.TextureAssets.MagicPixel.Value;
                spriteBatch.Draw(tex, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), null, Color.Black * 0.65f);
            }

            base.Draw(spriteBatch);
        }
    }
}