using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    public static class SimpleSlider
    {
        private static int sliderIndex;
        private static int heldSlider = -1; // -1 means no slider is being dragged

        /// <summary>
        /// Resets the slider system for a new frame. Call this once before drawing any sliders.
        /// </summary>
        public static void BeginFrame()
        {
            sliderIndex = 0;
            if (!Main.mouseLeft)
            {
                heldSlider = -1;
            }
        }

        public static float Draw(SpriteBatch sb, int x, int y, string label, float currentRatio)
        {
            int currentIndex = sliderIndex;
            sliderIndex++; // Prepare for the next slider

            // Size & position
            int sliderX = 80;
            Rectangle size = new(x + sliderX, y, 150, 20);
            x += sliderX;
            Vector2 pos = new(x, y);

            bool IsMouseHovering = Main.MouseScreen.Between(pos, pos + size.Size());

            // Update the slider holding state
            if (Main.mouseLeft && IsMouseHovering && heldSlider == -1)
            {
                heldSlider = currentIndex;
            }
            else
            {
                heldSlider = -1;
            }


            // Update the slider value
            bool isHeld = heldSlider == currentIndex;
            if (isHeld && Main.mouseLeft)
            {
                // Always use the current mouse X position
                float mouseX = Main.MouseScreen.X;
                currentRatio = MathHelper.Clamp((mouseX - size.X) / size.Width, 0f, 1f);
            }

            // Draw text
            string val = $"{label}: {Math.Round(currentRatio * 255)}";
            int textX = 15;
            //Log.Info("x" + x);
            Utils.DrawBorderString(Main.spriteBatch, val, new Vector2(x + textX - sliderX, y), Color.White);

            // Draw outline
            Texture2D sliderTex = Ass.Slider.Value;
            DrawBar(sb, sliderTex, size, Color.White);

            // Draw yellow hover
            Texture2D sliderOutlineTex = Ass.SliderHighlight.Value;
            if (isHeld || IsMouseHovering)
                DrawBar(sb, sliderOutlineTex, size, Main.OurFavoriteColor);

            // Draw inner gradient bar
            size.Inflate(-4, -4);
            sb.Draw(Ass.Gradient.Value, size, Color.White);

            // Draw blip
            Texture2D blip = TextureAssets.ColorSlider.Value;
            Vector2 blipOrigin = blip.Size() * 0.5f;
            Vector2 blipPosition = new(size.X + (currentRatio * size.Width), size.Center.Y);
            sb.Draw(blip, blipPosition, null, Color.White, 0f, blipOrigin, 1f, SpriteEffects.None, 0f);

            return currentRatio;
        }

        public static void DrawBar(SpriteBatch sb, Texture2D texture, Rectangle dimensions, Color color)
        {
            if (texture == null) return;
            sb.Draw(texture, new Rectangle(dimensions.X, dimensions.Y, 6, dimensions.Height), new Rectangle(0, 0, 6, texture.Height), color);
            sb.Draw(texture, new Rectangle(dimensions.X + 6, dimensions.Y, dimensions.Width - 12, dimensions.Height), new Rectangle(6, 0, 2, texture.Height), color);
            sb.Draw(texture, new Rectangle(dimensions.X + dimensions.Width - 6, dimensions.Y, 6, dimensions.Height), new Rectangle(8, 0, 6, texture.Height), color);
        }
    }
}
