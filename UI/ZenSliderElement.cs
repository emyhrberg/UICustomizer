using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace UICustomizer.UI
{
    internal class ZenSliderElement : UIElement
    {
        private readonly string labelTextKey;
        public UIText Label;
        public ZenSlider Slider;
        public float Min { get; }
        public float Max { get; }
        private readonly float step;
        private readonly Action<float> onValueChangedCallback;
        private float appliedValue;
        private string tooltipText;

        private readonly bool applyOnReleaseBehavior;
        private float liveDraggingValue;

        public ZenSliderElement(string label, string tooltip, float min, float max, float defaultValue, float step = 0.01f, Action<float> onValueChanged = null, bool applyOnRelease = false)
        {
            Min = min;
            Max = max;
            this.tooltipText = tooltip;
            this.labelTextKey = label;
            this.step = step;
            this.onValueChangedCallback = onValueChanged;
            this.applyOnReleaseBehavior = applyOnRelease;

            Width.Set(0, 1f);
            Height.Set(30, 0);

            Label = new UIText("", 0.85f)
            {
                Left = { Pixels = 6 },
                VAlign = 0.5f,
                TextOriginX = 0f,
                TextOriginY = 0.5f
            };
            Append(Label);

            Slider = new ZenSlider()
            {
                Left = { Pixels = 120 },
                Width = { Pixels = -125, Percent = 1f },
                VAlign = 0.5f
            };

            appliedValue = MathHelper.Clamp(defaultValue, Min, Max);
            liveDraggingValue = appliedValue;
            Slider.Ratio = (appliedValue - Min) / (Max - Min);
            UpdateLabelText();

            if (applyOnReleaseBehavior)
            {
                Slider.OnDrag += HandleSliderDragLive;
                Slider.OnValueAppliedOnMouseUp += HandleSliderValueAppliedOnRelease;
            }
            else
            {
                Slider.OnDrag += HandleSliderDragNormal;
                Slider.OnValueAppliedOnMouseUp += HandleSliderValueAppliedOnRelease; // Also apply on release for normal sliders for click behavior
            }
            Append(Slider);
        }

        private void HandleSliderDragLive(float currentRatio)
        {
            float rawValue = Min + currentRatio * (Max - Min);
            liveDraggingValue = (float)Math.Round((rawValue - Min) / step) * step + Min;
            liveDraggingValue = MathHelper.Clamp(liveDraggingValue, Min, Max);
            UpdateLabelText();
        }

        private void HandleSliderValueAppliedOnRelease(float finalRatio)
        {
            float rawValue = Min + finalRatio * (Max - Min);
            appliedValue = (float)Math.Round((rawValue - Min) / step) * step + Min;
            appliedValue = MathHelper.Clamp(appliedValue, Min, Max);

            if (applyOnReleaseBehavior)
            {
                liveDraggingValue = appliedValue;
            }

            Slider.Ratio = (appliedValue - Min) / (Max - Min);

            onValueChangedCallback?.Invoke(appliedValue);
            UpdateLabelText();
        }

        private void HandleSliderDragNormal(float currentRatio)
        {
            float rawValue = Min + currentRatio * (Max - Min);
            float newSnappedValue = (float)Math.Round((rawValue - Min) / step) * step + Min;
            newSnappedValue = MathHelper.Clamp(newSnappedValue, Min, Max);

            if (Math.Abs(appliedValue - newSnappedValue) > float.Epsilon)
            {
                appliedValue = newSnappedValue;
                liveDraggingValue = appliedValue; // For normal sliders, live is same as applied

                onValueChangedCallback?.Invoke(appliedValue);
                UpdateLabelText();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!applyOnReleaseBehavior && Slider.IsHeld)
            {
                float expectedSnappedRatio = (appliedValue - Min) / (Max - Min);
                if (Math.Abs(Slider.Ratio - expectedSnappedRatio) > float.Epsilon * 10) // Added a slightly larger tolerance
                {
                    Slider.Ratio = expectedSnappedRatio;
                }
            }
        }
        private void UpdateLabelText()
        {
            string textToShow;
            if (applyOnReleaseBehavior)
            {
                string liveText, appliedTextFormatted;

                bool usePercentage = (labelTextKey != null && labelTextKey.Contains("UI Scale")) || (Max - Min <= 1f && Min >= 0f && Max <= 1f);

                if (usePercentage)
                {
                    liveText = $"{(int)(liveDraggingValue * 100f)}%";
                    appliedTextFormatted = $"{(int)(appliedValue * 100f)}%";
                }
                else
                {
                    // For applyOnRelease sliders that are not percentages, check if values are whole numbers
                    liveText = liveDraggingValue % 1 == 0 ? $"{(int)liveDraggingValue}" : $"{liveDraggingValue:F2}";
                    appliedTextFormatted = appliedValue % 1 == 0 ? $"{(int)appliedValue}" : $"{appliedValue:F2}";
                }
                textToShow = $"{labelTextKey}: {liveText} ";
                //textToShow = $"{labelTextKey}: ({liveText}) [{appliedTextFormatted}]";
            }
            else // For normal sliders like Snap Threshold
            {
                bool usePercentage = (Max - Min <= 1f && Min >= 0f && Max <= 1f); // This condition might not apply to Snap if its range is 0-100

                // Check if the Snap slider (or other non-percentage, non-applyOnRelease sliders) has a whole number value
                if (!usePercentage && appliedValue % 1 == 0)
                {
                    textToShow = $"{labelTextKey}: {(int)appliedValue}";
                }
                else if (usePercentage) // Handles the 0-1 range percentage case
                {
                    textToShow = $"{labelTextKey}: {(int)(appliedValue * 100f)}%";
                }
                else // Default for non-whole numbers or other cases
                {
                    textToShow = $"{labelTextKey}: {appliedValue:F2}";
                }
            }
            Label.SetText(textToShow);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (Label.IsMouseHovering && !string.IsNullOrEmpty(tooltipText))
            {
                //Main.instance.MouseText(tooltipText);
                UICommon.TooltipMouseText(tooltipText);
            }
        }
    }
}