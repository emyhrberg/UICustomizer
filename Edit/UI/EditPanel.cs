using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using UICustomizer.EditMode.System;

namespace UICustomizer.EditMode.UI
{
    public class EditPanel : UIPanel
    {
        private UIPanel positionsPanel;
        private UIPanel layoutsPanel;
        private UIPanel settingsPanel;

        public EditPanel()
        {
            Width.Set(60, 0);
            Height.Set(200, 0);
            Top.Set(120, 0);
            Left.Set(40, 0);

            // Base colors (dark blue like Journey Mode UI)
            BorderColor = new Color(89, 116, 213, 255) * 0.9f;
            BackgroundColor = new Color(73, 94, 171) * 0.9f;

            SetPadding(6);

            // Create 3 icon buttons
            var positionsButton = CreateIconButton(Ass.P.Value, 0, TogglePositions);
            var layoutsButton = CreateIconButton(Ass.O.Value, 40, ToggleLayouts);
            var settingsButton = CreateIconButton(Ass.S.Value, 80, ToggleSettings);

            Append(positionsButton);
            Append(layoutsButton);
            Append(settingsButton);

            // Create sub-panels (hidden initially)
            positionsPanel = CreateSubPanel("I am a positions panel");
            layoutsPanel = CreateSubPanel("I am a layouts panel");
            settingsPanel = CreateSubPanel("I am a settings panel");
        }

        private UIImage CreateIconButton(Texture2D tex, float topOffset, UIElement.MouseEvent onClick)
        {
            var button = new UIImage(tex)
            {
                Width = new StyleDimension(32, 0),
                Height = new StyleDimension(32, 0),
                Top = new StyleDimension(topOffset, 0),
                HAlign = 0.5f
            };
            button.OnLeftClick += onClick;
            return button;
        }

        private UIPanel CreateSubPanel(string labelText)
        {
            var panel = new UIPanel
            {
                Width = new StyleDimension(200, 0),
                Height = new StyleDimension(120, 0),
                Top = new StyleDimension(20, 0),
                Left = new StyleDimension(70, 0),
                BorderColor = new Color(89, 116, 213, 255) * 0.9f,
                BackgroundColor = new Color(73, 94, 171) * 0.9f
            };

            var text = new UIText(labelText)
            {
                HAlign = 0.5f,
                VAlign = 0.5f
            };

            panel.Append(text);
            panel.Remove(); // start hidden
            Append(panel);
            return panel;
        }

        private void TogglePositions(UIMouseEvent evt, UIElement listeningElement) => TogglePanel(positionsPanel);
        private void ToggleLayouts(UIMouseEvent evt, UIElement listeningElement) => TogglePanel(layoutsPanel);
        private void ToggleSettings(UIMouseEvent evt, UIElement listeningElement) => TogglePanel(settingsPanel);

        private void TogglePanel(UIPanel panel)
        {
            // Hide all first
            positionsPanel.Remove();
            layoutsPanel.Remove();
            settingsPanel.Remove();

            // Show selected
            Append(panel);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var sys = ModContent.GetInstance<EditSystem>();
            if (!sys.Enabled) return;

            base.Draw(spriteBatch);
        }
    }
}
