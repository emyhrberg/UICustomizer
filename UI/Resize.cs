using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;

namespace UICustomizer.UI
{
    public class Resize : UIImageButton
    {
        public bool draggingResize;
        private float clickOffsetX, clickOffsetY;

        public event Action<float> OnDragX;
        public event Action<float> OnDragY;

        public Resize(Asset<Texture2D> texture) : base(texture)
        {
            // Position the button inside the bottom-right corner
            HAlign = 1f;
            VAlign = 1f;
            Left.Set(18, 0);
            Top.Set(16, 0);

            Width.Set(35, 0f);
            Height.Set(35, 0f);

            OnLeftMouseDown += (evt, _) =>
            {
                draggingResize = true;
                clickOffsetY = evt.MousePosition.Y - GetDimensions().Y;
                clickOffsetX = evt.MousePosition.X - GetDimensions().X;
            };

            OnLeftMouseUp += (evt, listeningElement) =>
            {
                draggingResize = false;
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (draggingResize)
            {
                if (!Main.mouseLeft)
                {
                    draggingResize = false;
                }
                else
                {
                    // Y-offset
                    float newTop = Main.MouseScreen.Y - clickOffsetY;
                    float offsetY = newTop - GetDimensions().Y;
                    OnDragY?.Invoke(offsetY);

                    // X-offset
                    float newLeft = Main.MouseScreen.X - clickOffsetX;
                    float offsetX = newLeft - GetDimensions().X;
                    OnDragX?.Invoke(offsetX);
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
    }
}