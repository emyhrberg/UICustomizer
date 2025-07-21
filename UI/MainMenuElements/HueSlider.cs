using Terraria.GameContent.UI.States;
using UICustomizer.Common.Systems.Hooks.MainMenu;

namespace UICustomizer.UI.MainMenuElements
{
    internal class HueSlider : ZenSlider
    {
        public HueSlider()
        {
            Top.Set(40, 0);
            InnerTexture = Ass.SliderHueGradient;

            static void updateColor(float value)
            {
                Color updatedColor = UICharacterCreation.ScaledHslToRgb(value, 1f, 0.5f);
                MainMenuTextColorHook.MainMenuTextColor = updatedColor;
            }

            OnDrag += updateColor;
            OnValueAppliedOnMouseUp += updateColor;
        }

        public override void Update(GameTime gameTime)
        {
            Top.Set(40, 0);
            base.Update(gameTime);
        }
    }
}
