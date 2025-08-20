using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI;

internal sealed class Underline : UIElement
{
    private readonly Color color;

    public Underline(float thickness = 2f, float horizontalInset = 6f, Color? colorOverride = null)
    {
        color = colorOverride ?? Color.White;

        Left.Pixels = horizontalInset;
        Width.Percent = 1f;
        Width.Pixels = -horizontalInset * 2f;
        Height.Pixels = thickness;

        IgnoresMouseInteraction = true;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(TextureAssets.MagicPixel.Value, GetDimensions().ToRectangle(), color);
    }
}
