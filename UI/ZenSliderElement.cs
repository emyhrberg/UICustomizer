using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace UICustomizer.UI
{
    internal class ZenSliderElement : UIElement
    {
        private readonly string labelText;
        public UIText Label;
        public ZenSlider Slider;
        public float Min { get; }
        public float Max { get; }
        private readonly float step;
        private readonly Action<float> onValueChanged;
        private float lastValue;
        private string tooltip;

        public ZenSliderElement(string labelText, string tooltip, float min, float max, float defaultValue, float step = 0.01f, Action<float> onValueChanged = null)
        {
            Min = min;
            Max = max;
            this.tooltip = tooltip;
            this.labelText = labelText;
            this.step = step;
            this.onValueChanged = onValueChanged;

            Width.Set(0, 1f);
            Height.Set(30, 0);

            Label = new UIText("", 0.35f, true)
            {
                Left = { Pixels = 6 },
                VAlign = 0.5f
            };
            Append(Label);

            Slider = new ZenSlider()
            {
                Left = { Pixels = 110 },
                VAlign = 0.5f
            };
            float ratio = MathHelper.Clamp((defaultValue - Min) / (Max - Min), 0f, 1f);
            Slider.Ratio = ratio;
            lastValue = Min + ratio * (Max - Min);
            Append(Slider);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float raw = Min + Slider.Ratio * (Max - Min);
            float snapped = (float)Math.Round((raw - Min) / step) * step + Min;
            snapped = MathHelper.Clamp(snapped, Min, Max);
            Slider.Ratio = (snapped - Min) / (Max - Min);

            if (Math.Abs(snapped - lastValue) > float.Epsilon)
            {
                lastValue = snapped;
                onValueChanged?.Invoke(snapped);
            }

            // **SHOW** either percent or raw depending on your range:
            if (Max <= 1f)
            {
                int pct = (int)(snapped * 100f);
                Label.SetText($"{labelText}: {pct}%");
            }
            else
            {
                // no decimals, just integer value
                Label.SetText($"{labelText}: {(int)snapped}");
            }
        }

        //public override void Draw(SpriteBatch spriteBatch)
        //{
        //    base.Draw(spriteBatch);

        //    if (Label.IsMouseHovering && !string.IsNullOrEmpty(tooltip))
        //    {
        //        UICommon.TooltipMouseText(tooltip);
        //    }
        //}
    }
}
