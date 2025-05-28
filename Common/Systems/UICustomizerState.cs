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
        public SliderPanel sliderPanel;

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

            sliderPanel = new(
                title: "UI Scale",
                min: 0.5f,
                max: 2f,
                defaultValue: Main.UIScale,
                onValueChanged: value => Main.UIScale = value,
                increment: 0.01f,
                textSize: 0.8f,
                hover: "Adjust the UI scale",
                valueFormatter: value => $"{value * 100f:0}%"
            );
            Append(sliderPanel);
        }

        // Draw black background with lower opacity to show we're in edit mode.
        // TODO: Exclude all UI that can be moved from this black drawing.
        // Reference:
        // https://github.com/ScalarVector1/DragonLens/blob/master/Content/GUI/ToolbarState.cs#L132
        public override void Draw(SpriteBatch sb)
        {
            if (UICustomizerSystem.EditModeActive)
            {
                // Draw black hover effect
                Texture2D tex = Terraria.GameContent.TextureAssets.MagicPixel.Value;
                sb.Draw(tex, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), null, Color.Black * 0.65f);

                // Draw chat
                DrawHelper.DrawDebugHitboxOutline(sb, DragHelper.ChatBounds(), Color.Red);
                DrawHelper.DrawDebugText(sb, DragHelper.ChatBounds().Location.ToVector2(), "Chat");

                // Draw hotbar
                DrawHelper.DrawDebugHitboxOutline(sb, DragHelper.HotbarBounds(), Color.Red);
                DrawHelper.DrawDebugText(sb, DragHelper.HotbarBounds().Location.ToVector2(), "Hotbar");
            }


            base.Draw(sb);


        }
    }
}