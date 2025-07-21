using System.Globalization;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.OS;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using UICustomizer.Common.Systems.Hooks.MainMenu;

namespace UICustomizer.UI.MainMenuElements.TagElements
{
    public class PasteTag : UIColoredImageButton
    {
        public PasteTag(Asset<Texture2D> texture, bool isSmall = true) : base(texture, isSmall)
        {
            VAlign = 1f;
            Left.Set(40, 0);

            OnLeftMouseDown += (_, __) =>
            {
                var cb = Platform.Get<IClipboard>();

                // Verify input
                var hex = cb.Value?.Trim().TrimStart('#');
                bool validHex = !string.IsNullOrEmpty(hex) && hex.Length == 6;

                if (validHex && int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var rgb))
                {
                    // Use bitwise operations to extract RGB values 
                    // Highest 8: red, middle 8: green, lowest 8: blue
                    Color updatedColor = new Color(rgb >> 16 & 255, rgb >> 8 & 255, rgb & 255);
                    MainMenuTextColorHook.MainMenuTextColor = updatedColor;
                }
            };
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
    }
}