using Terraria.GameContent.UI.States;
using UICustomizer.Common.Systems.Hooks.MainMenu;
using static UICustomizer.Common.Systems.MainMenu.MainMenuState;

namespace UICustomizer.UI.MainMenuElements
{
    internal class HueSlider : ZenSlider
    {
        public HueSlider(TextColorType type)
        {
            Top.Set(40, 0);
            InnerTexture = Ass.SliderHueGradient;

            void updateColor(float value)
            {
                Color updatedColor = UICharacterCreation.ScaledHslToRgb(value, 1f, 0.5f);
                var a = type switch
                {
                    TextColorType.Fill => MainMenuTextColorHook.NormalColor = updatedColor,
                    TextColorType.Outline => MainMenuOutlineTextColorHook.Color = updatedColor,
                    TextColorType.Hover => MainMenuTextColorHook.HoverColor = updatedColor,
                    _ => throw new System.NotImplementedException(),
                };
            }

            OnDrag += updateColor;
            OnValueAppliedOnMouseUp += updateColor;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
