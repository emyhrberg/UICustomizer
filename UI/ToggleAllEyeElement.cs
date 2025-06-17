using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace UICustomizer.UI
{
    public class ToggleAllEyeElement : UIImage
    {
        private bool _isOpen = true;
        public bool IsOpen => _isOpen;
        public event Action<bool>? OnToggle;

        public ToggleAllEyeElement(Asset<Texture2D> texture) : base(texture) 
        {
            Width.Set(texture.Value.Width, 0f);
            Height.Set(texture.Value.Height, 0f);

            Left.Set(-8, 0);
            HAlign = 1f;
            VAlign = 0.5f;
            ImageScale = 1.4f;

            OnMouseOut += (_, _) =>
            {
                SetImage(_isOpen ? Ass.EyeOpen : Ass.EyeClosed);
            };

            OnMouseOver += (_, _) =>
            {
                Texture2D tex = _isOpen ? Ass.EyeOpenHover.Value : Ass.EyeClosedHover.Value;
                SetImage(tex);
            };
        }

        public void SetState(bool open)
        {
            _isOpen = open;
            SetImage(open ? Ass.EyeOpen : Ass.EyeClosed);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            _isOpen = !_isOpen;
            SetImage(_isOpen ? Ass.EyeOpen : Ass.EyeClosed);
            OnToggle?.Invoke(_isOpen);
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
