
using System;
using Microsoft.Xna.Framework;
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
            _box = new CheckboxBox(!_isChecked ? Ass.CheckInactive : Ass.CheckActive); // cheeky hehe
            _box.Left.Set(0, 0);
            _box.Top.Set(0, 0);

            OnMouseOver += (_, _) =>
            {
                    //_box.SetImage(ASS.HOVER)
            };
            OnMouseOut += (_, _) =>
            {
                    //_box.SetImage(_isChecked ? TODO
            };

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

            Top.Set(0, 0);

            if (IsMouseHovering && !string.IsNullOrEmpty(_tooltip))
                UICommon.TooltipMouseText(_tooltip);
        }
    }
    public class CheckboxBox : UIImageButton
    {
        private bool _isHovering;
        private float _hoverTimer;
        private static readonly float HoverFadeTime = 0.2f;
        private float opacity;

        public CheckboxBox(Asset<Texture2D> texture) : base(texture)
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
            Vector2 pos = Parent.GetDimensions().Position();
            pos += new Vector2(0, -3);
            sb.Draw(_texture.Value, pos, Color.White * opacity);
        }
    }

}