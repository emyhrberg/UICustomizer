using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace UICustomizer.UI.SliderElements
{
    public sealed class SliderElement : UIElement
    {
        private readonly UIText _caption;
        private readonly Slider _slider;
        private readonly string _tip;

        private readonly string _title;
        private readonly float _min, _max;
        private float _norm;
        private readonly float? _step;
        private readonly Action<float> _changed;
        private readonly Func<float, string> _fmt;

        public SliderElement(string title, float min, float max, float start,
                             Action<float> changed = null, float? step = null,
                             float textScale = .4f, string tip = "", Func<float, string> fmt = null)
        {
            _title = title;
            _tip = tip;
            _min = min;
            _max = max;
            _norm = MathHelper.Clamp((start - min) / (max - min), 0, 1);
            _step = step;
            _changed = changed;
            _fmt = fmt;

            Width.Set(-12, 1);
            Height.Set(40, 0);

            // Add slider
            _slider = new Slider(
                () => _norm,
                v =>
                {
                    // 1) convert normalized -> raw
                    float raw = MathHelper.Lerp(_min, _max, v);
                    float snapped = _step.HasValue
                        ? (float)Math.Round(raw / _step.Value) * _step.Value
                        : raw;
                    _norm = (_max - _min) > 0
                        ? (snapped - _min) / (_max - _min)
                        : 0;
                    _changed?.Invoke(snapped);
                    Refresh();
                },
                t => Color.Lerp(Color.Black, Color.White, t)
            );
            Append(_slider);

            // Get value and format it
            float v = MathHelper.Lerp(_min, _max, _norm);
            string s = _fmt != null ? _fmt(v) :
                       _step == 1f ? ((int)Math.Round(v)).ToString() :
                       _step == .1f ? v.ToString("F1") :
                       _step == .01f ? v.ToString("F2") :
                                        v.ToString("F3");

            // Add text
            _caption = new UIText($"{_title}: {s}", textScale, true) { VAlign = 0.5f, Left = { Pixels = -0f} };
            if (!string.IsNullOrEmpty(tip))
            {
                // when you hover *anywhere* over the slider bar itself:
                _slider.OnMouseOver += (_, _) => UICommon.TooltipMouseText(tip);

                // optional: also for the caption text:
                _caption.OnMouseOver += (_, _) => UICommon.TooltipMouseText(tip);
            }
            Append(_caption);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (_caption.IsMouseHovering && !string.IsNullOrEmpty(_tip))
            {
                UICommon.TooltipMouseText(_tip);
            }
        }

        public void SetValue(float v)
        {
            _norm = MathHelper.Clamp((v - _min) / (_max - _min), 0, 1);
            Refresh();
        }

        private void Refresh()
        {
            float v = MathHelper.Lerp(_min, _max, _norm);

            string s = _fmt != null ? _fmt(v) :
                       _step == 1f ? ((int)Math.Round(v)).ToString() :
                       _step == .1f ? v.ToString("F1") :
                       _step == .01f ? v.ToString("F2") :
                                        v.ToString("F3");

            _caption.SetText($"{_title}: {s}");
        }
    }
}
