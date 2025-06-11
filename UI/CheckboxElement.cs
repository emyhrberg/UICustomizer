
using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace UICustomizer.UI
{
    /// <summary>
    /// A simple checkbox UI element consisting of a text label and a checkbox box.
    /// </summary>
    public class CheckboxElement : UIElement
    {
        private bool _isChecked;
        private readonly CheckboxBox _box;
        private readonly UIText _label;
        private readonly string _tooltip;
        private readonly Action<bool> _onStateChanged;
        public bool Active = false;
        public bool GetChecked() => _isChecked;

        public CheckboxElement(
            string text,
            bool initialState,
            Action<bool> onStateChanged,
            int width = 50,
            string tooltip = "",
            bool maxWidth=false,
            int height = 30
        )
        {
            _isChecked = initialState;
            _onStateChanged = onStateChanged;
            _tooltip = tooltip;

            Height.Set(height, 0);
            Width.Set(width, 0);

            // Box
            _box = new CheckboxBox(!_isChecked ? Ass.CheckInactive : Ass.CheckActive);
            _box.Left.Set(0, 0);
            _box.Top.Set(0, 0);

            Append(_box);

            // Label
            _label = new UIText(text, 0.34f, true);
            _label.Left.Set(36, 0);
            _label.Top.Set(1, 0);
            _label.VAlign = 0.5f;
            Append(_label);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (!Active) return;
            base.LeftClick(evt);

            _isChecked = !_isChecked;
             _box.SetImage(_isChecked ? Ass.CheckActive : Ass.CheckInactive);
            _onStateChanged?.Invoke(_isChecked);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active) return;
            base.Draw(spriteBatch);

            //Top.Set(0, 0);

            if (IsMouseHovering && !string.IsNullOrEmpty(_tooltip))
                UICommon.TooltipMouseText(_tooltip);
        }
    }
    public class CheckboxBox : UIImageButton
    {
        private float _hoverTimer;
        private static readonly float HoverFadeTime = 0.2f;
        private float opacity;

        public CheckboxBox(Asset<Texture2D> texture) : base(texture)
        {
            VAlign = 0.5f;

            int s = 30;

            Width.Set(s, 0);
            Height.Set(s, 0);
            MaxWidth.Set(s, 0);
            MaxHeight.Set(s, 0);
            Left.Set(-24, 0);
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // advance or reverse the hover timer
            if (Parent is CheckboxElement c && c.IsMouseHovering)
                _hoverTimer = Math.Min(HoverFadeTime, _hoverTimer + dt);
            else
                _hoverTimer = Math.Max(0f, _hoverTimer - dt);

            float t = _hoverTimer / HoverFadeTime;
            opacity = MathHelper.Lerp(0.4f, 1.0f, t);

            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch sb)
        {
            int s = 60;

            //Width.Set(s, 0);
            //Height.Set(s, 0);
            //MaxWidth.Set(s, 0);
            //MaxHeight.Set(s, 0);
            //Left.Set(-12, 0);

            CalculatedStyle dimensions = GetDimensions(); // This will use Width/Height set in constructor

            if (_texture != null && _texture.IsLoaded)
            {
                //if (IsMouseHovering)
                //{
                //    SetImage(Ass.CheckActiveHover);
                //}

                sb.Draw(_texture.Value, dimensions.ToRectangle(), Color.White * 1.0f);
            }
        }
    }

}