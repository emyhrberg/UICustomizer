using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;
using UICustomizer.Helpers;

namespace UICustomizer.UI;

public sealed class ZenSlider : UIElement
{
    public ZenSlider()
    {
        Width.Set(0, 1f);
        Height.Set(16, 0f);

        InnerColor = Color.Gray;
    }

    public Color InnerColor;

    public static bool IsAnySliderHeld = false;
    public bool IsHeld = false;

    public float Ratio;

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        if (Main.alreadyGrabbingSunOrMoon)
            return;

        base.LeftMouseDown(evt);
        if (evt.Target == this)

        {
            IsHeld = true;
            IsAnySliderHeld = true;
        }
    }

    public override void LeftMouseUp(UIMouseEvent evt)
    {
        base.LeftMouseUp(evt);
        IsHeld = false;
        IsAnySliderHeld = false;
    }

    public override void MouseOver(UIMouseEvent evt)
    {
        if (Main.alreadyGrabbingSunOrMoon)
            return;

        base.MouseOver(evt);
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        Left.Set(120, 0);
        Width.Set(-125, 1);
        CalculatedStyle dims = GetDimensions();

        // Dispite how impossible it should be I'm doing this to be extra safe.
        if (IsHeld && !Main.alreadyGrabbingSunOrMoon)
        {
            float num = UserInterface.ActiveInstance.MousePosition.X - dims.X;
            Ratio = MathHelper.Clamp(num / dims.Width, 0f, 1);
        }

        Texture2D slider = Ass.Slider.Value;
        Texture2D sliderOutline = Ass.SliderHighlight.Value;

        Rectangle size = dims.ToRectangle();

        DrawBar(spriteBatch, slider, size, Color.White);
        if (IsHeld || IsMouseHovering)
            DrawBar(spriteBatch, sliderOutline, size, Main.OurFavoriteColor);

        size.Inflate(-4, -4);
        spriteBatch.Draw(TextureAssets.MagicPixel.Value, size, InnerColor);

        Texture2D blip = TextureAssets.ColorSlider.Value;

        Vector2 blipOrigin = blip.Size() * 0.5f;
        Vector2 blipPosition = new(size.X + (Ratio * size.Width), size.Center.Y);

        spriteBatch.Draw(blip, blipPosition, null, Color.White, 0f, blipOrigin, 1f, 0, 0f);
    }

    public static void DrawBar(SpriteBatch spriteBatch, Texture2D texture, Rectangle dimensions, Color color)
    {
        spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y, 6, dimensions.Height), new Rectangle(0, 0, 6, texture.Height), color);
        spriteBatch.Draw(texture, new Rectangle(dimensions.X + 6, dimensions.Y, dimensions.Width - 12, dimensions.Height), new Rectangle(6, 0, 2, texture.Height), color);
        spriteBatch.Draw(texture, new Rectangle(dimensions.X + dimensions.Width - 6, dimensions.Y, 6, dimensions.Height), new Rectangle(8, 0, 6, texture.Height), color);
    }
}