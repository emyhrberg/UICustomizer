using DragonLens.Content.GUI;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace UICustomizer.UI.MainMenuElements
{
    public class TabButton : UIElement
    {
        public Asset<Texture2D> _backPanelTexture;

        public Asset<Texture2D> _backPanelHighlightTexture;

        public Asset<Texture2D> _backPanelBorderTexture;

        public Color _color;

        private bool _selected;

        private string _tooltip;

        public TabButton(string tooltip)
        {
            _tooltip = tooltip;
            _color = Color.White; // default color

            // Set textures
            _backPanelTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/SmallPanel");
            _backPanelHighlightTexture = Ass.SmallPanelHighlight;
            _backPanelBorderTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/SmallPanelBorder");

            // Set size
            Width.Set(_backPanelTexture.Width(), 0f);
            Height.Set(_backPanelTexture.Height(), 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            Vector2 position = dimensions.Position() + new Vector2(dimensions.Width, dimensions.Height) / 2f;
            spriteBatch.Draw(_backPanelTexture.Value, position, null, Color.White, 0f, _backPanelTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            _ = Color.White;
            if (IsMouseHovering)
            {
                spriteBatch.Draw(_backPanelBorderTexture.Value, position, null, Color.White, 0f, _backPanelBorderTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
                UICommon.TooltipMouseText(_tooltip);
            }

            if (_selected)
            {
                spriteBatch.Draw(_backPanelHighlightTexture.Value, position, null, Color.White, 0f, _backPanelHighlightTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }

            // Draw first char centered in the panel
            string first = _tooltip.Substring(0, 1);
            Vector2 charPos = position -= new Vector2(5, 10);

            Utils.DrawBorderString(spriteBatch, first, position, _color); 

            //spriteBatch.Draw(_texture.Value, position, null, _color, 0f, _texture.Size() / 2f, 1f, SpriteEffects.None, 0f);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public void SetColor(Color color)
        {
            _color = color;
        }

        public void SetSelected(bool selected)
        {
            _selected = selected;
        }
    }
}
