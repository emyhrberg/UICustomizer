using System;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;

namespace UICustomizer.UI
{
    internal class OnOffText : UIText
    {
        private readonly Func<bool> getter;
        private readonly Action<bool> setter;
        private readonly string label;

        public OnOffText(string label, Func<bool> getter, Action<bool> setter, float textScale = 1, bool large = false)
            : base("", textScale, large)
        {
            this.getter = getter;
            this.setter = setter;
            this.label = label;

            TextColor = Color.Gray;
            Left.Set(1, 0);
            HAlign = 0.5f;

            OnLeftClick += (_, _) => ToggleState();
            OnMouseOver += (_, _) =>
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                TextColor = Color.White;
            };
            OnMouseOut += (_, _) =>
            {
                TextColor = Color.Gray;
            };

            UpdateText();
        }

        private void ToggleState()
        {
            bool newState = !getter();
            setter(newState);
            UpdateText();
        }

        private void UpdateText()
        {
            SetText($"{label}: {(getter() ? "On" : "Off")}");
        }
    }
}
