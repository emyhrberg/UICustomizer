using Terraria.GameContent.UI.States;
using UICustomizer.Common.Configs;
using UICustomizer.Common.Systems.Hooks.MainMenu;
using static UICustomizer.Common.Systems.MainMenu.MainMenuState;

namespace UICustomizer.UI.MainMenuElements
{
    internal class HueSlider : ZoeSlider
    {
        public HueSlider(TextColorType type)
        {
            Top.Set(40, 0);
            InnerTexture = Ass.SliderHueGradient;

            void updateColor(float value)
            {
                Color updatedColor = UICharacterCreation.ScaledHslToRgb(value, 1f, 0.5f);
                var _ = type switch
                {
                    TextColorType.Fill => MainMenuTextColorHook.FillColor = updatedColor,
                    TextColorType.Outline => MainMenuOutlineTextColorHook.OutlineColor = updatedColor,
                    TextColorType.Hover => MainMenuTextColorHook.HoverColor = updatedColor,
                    _ => throw new System.NotImplementedException(),
                };
            }

            OnDrag += updateColor;
            OnValueAppliedOnMouseUp += (v) =>
            { 
                Color updatedColor = UICharacterCreation.ScaledHslToRgb(v, 1f, 0.5f);

                string hex = UICharacterCreation.GetHexText(updatedColor);

                var _ = type switch
                {
                    TextColorType.Fill => Conf.C.FillColor = hex,
                    TextColorType.Outline => Conf.C.OutlineColor = hex,
                    TextColorType.Hover => Conf.C.HoverColor = hex,
                    _ => throw new System.NotImplementedException(),
                };
                Conf.Save();
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
