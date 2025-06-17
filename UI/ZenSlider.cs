using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;

namespace UICustomizer.UI;

public sealed class ZenSlider : UIElement
{
    public ZenSlider()
    {
        Width.Set(0, 1f); // Default width, ZenSliderElement will position Left and adjust Width
        Height.Set(16, 0f);
        InnerColor = Color.Gray;
    }

    public Color InnerColor;
    public static bool IsAnySliderHeld = false;
    public bool IsHeld = false;
    public float Ratio;

    public event Action<float> OnValueAppliedOnMouseUp;
    public event Action<float> OnDrag;
    private bool _wasHeldLastFrame = false;

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);
        if (evt.Target == this)
        {
            IsHeld = true;
            IsAnySliderHeld = true;
            _wasHeldLastFrame = true;
        }
    }

   

    public override void MouseOver(UIMouseEvent evt)
    {
        base.MouseOver(evt);
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    public override void LeftMouseUp(UIMouseEvent evt)
    {
        base.LeftMouseUp(evt);
        if (IsHeld)
        {
            var dims = GetDimensions();
            if (dims.Width > 0)
            {
                float num = Main.MouseScreen.X - dims.X;
                Ratio = MathHelper.Clamp(num / dims.Width, 0f, 1f); // <- changed this because UserInterface.ActiveInstance was mis-scaled
                Log.Info($"ZenSlider → LeftMouseUp Ratio: {Ratio:F2}"); // debug
            }
            OnValueAppliedOnMouseUp?.Invoke(Ratio);
        }
        IsHeld = false;
        IsAnySliderHeld = false;
        _wasHeldLastFrame = false;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (IsHeld)
        {
            var dims = GetDimensions();
            float num = Main.MouseScreen.X - dims.X;
            float newRatio = MathHelper.Clamp(num / dims.Width, 0f, 1f); // <- changed this because UserInterface.ActiveInstance was mis-scaled
            if (Math.Abs(newRatio - Ratio) > float.Epsilon)
            {
                Ratio = newRatio;
                Log.Info($"ZenSlider → Dragging Ratio: {Ratio:F2}"); // debug
                OnDrag?.Invoke(Ratio);
            }
        }
    }

    protected override void DrawSelf(SpriteBatch sb)
    {
        CalculatedStyle dims = GetDimensions();
        Rectangle size = dims.ToRectangle();

        Texture2D sliderTex = Ass.Slider.Value;
        Texture2D sliderOutlineTex = Ass.SliderHighlight.Value;

        DrawBar(sb, sliderTex, size, Color.White);
        if (IsHeld || IsMouseHovering)
            DrawBar(sb, sliderOutlineTex, size, Main.OurFavoriteColor);

        Rectangle innerBarArea = size;
        innerBarArea.Inflate(-4, -4);
        sb.Draw(Ass.Gradient.Value, innerBarArea, Color.White);
        //spriteBatch.Draw(TextureAssets.MagicPixel.Value, innerBarArea, InnerColor);

        Texture2D blip = TextureAssets.ColorSlider.Value;
        Vector2 blipOrigin = blip.Size() * 0.5f;
        Vector2 blipPosition = new(innerBarArea.X + (Ratio * innerBarArea.Width), innerBarArea.Center.Y);

        sb.Draw(blip, blipPosition, null, Color.White, 0f, blipOrigin, 1f, SpriteEffects.None, 0f);
    }

    public static void DrawBar(SpriteBatch spriteBatch, Texture2D texture, Rectangle dimensions, Color color)
    {
        if (texture == null) return;
        spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y, 6, dimensions.Height), new Rectangle(0, 0, 6, texture.Height), color);
        spriteBatch.Draw(texture, new Rectangle(dimensions.X + 6, dimensions.Y, dimensions.Width - 12, dimensions.Height), new Rectangle(6, 0, 2, texture.Height), color);
        spriteBatch.Draw(texture, new Rectangle(dimensions.X + dimensions.Width - 6, dimensions.Y, 6, dimensions.Height), new Rectangle(8, 0, 6, texture.Height), color);
    }
}