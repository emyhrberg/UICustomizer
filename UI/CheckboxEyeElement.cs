
using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;
using UICustomizer.Helpers;

namespace UICustomizer.UI
{
    /// <summary>
    /// A simple checkbox UI element consisting of a text label and a checkbox box.
    /// </summary>
    public class CheckboxEyeElement : UIPanel
    {
        private bool _isChecked;
        private readonly CheckboxEye eye;
        private readonly UIText _label;
        private readonly string _tooltip;
        private readonly Action<bool> _onStateChanged;
        public bool GetChecked() => _isChecked;

        public CheckboxEyeElement(
            string text,
            bool initialState,
            Action<bool> onStateChanged,
            int width = 50,
            string tooltip = "",
            bool maxWidth = false,
            int height = 30
        )
        {
            _isChecked = initialState;
            _onStateChanged = onStateChanged;
            _tooltip = tooltip;

            Height.Set(height, 0);
            Width.Set(width+4, 1);
            MaxWidth.Set(width+4, 1);
            //Left.Set(-2, 0);

            // Eye
            eye = new CheckboxEye(_isChecked ? Ass.EyeOpen : Ass.EyeClosed);
            Append(eye);

            OnMouseOver += (_, _) =>
            {
                eye.SetImage(_isChecked ? Ass.EyeOpenHover : Ass.EyeClosedHover);
            };
            OnMouseOut += (_, _) =>
            {
                eye.SetImage(_isChecked ? Ass.EyeOpen : Ass.EyeClosed);
            };

            Append(eye);

            // Label
            _label = new UIText(text, 0.34f, true);
            _label.Left.Set(24, 0);
            _label.Top.Set(-1, 0);
            _label.VAlign = 0.5f;
            Append(_label);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            _isChecked = !_isChecked;
            eye.SetImage(_isChecked ? Ass.EyeOpen : Ass.EyeClosed);
            _onStateChanged?.Invoke(_isChecked);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (IsMouseHovering && !string.IsNullOrEmpty(_tooltip))
                UICommon.TooltipMouseText(_tooltip);
        }
    }
    public class CheckboxEye : UIImageButton
    {
        private bool _isHovering;
        private float _hoverTimer;
        private static readonly float HoverFadeTime = 0.2f;
        private float opacity;

        public CheckboxEye(Asset<Texture2D> texture) : base(texture)
        {
            Width.Set(22, 0);
            Height.Set(22, 0);
            MaxWidth.Set(22, 0);
            MaxHeight.Set(22, 0);
            VAlign = 0.5f;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            _isHovering = true;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
            _isHovering = false;
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // advance or reverse the hover timer
            if (_isHovering)
                _hoverTimer = Math.Min(0.5f, _hoverTimer + dt);
            else
                _hoverTimer = Math.Max(0f, _hoverTimer - dt);

            float t = _hoverTimer / 0.5f;
            opacity = MathHelper.Lerp(1f, 0.4f, t);

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            //Vector2 pos = Parent.GetDimensions().Position();
            //pos += new Vector2(3, -6);
            //VAlign = 0.5f;
            //sb.Draw(_texture.Value, pos, Color.White * opacity);
        }
    }

}