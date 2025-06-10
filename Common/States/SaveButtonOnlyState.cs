using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.ID;
using Terraria.UI;
using UICustomizer.Common.Systems;
using UICustomizer.Helpers.Layouts;
using UICustomizer.UI;

namespace UICustomizer.Common.States
{
    public class SaveButtonOnlyState : UIState
    {
        public SaveButtonOnlyButton saveButton;

        public SaveButtonOnlyState()
        {
            saveButton = new(
                text: "Save",
                onClick: () =>
                    {
                        SoundEngine.PlaySound(SoundID.MenuOpen);
                        //string layout = LayoutHelper.CurrentLayoutName;
                        LayoutHelper.SaveLastLayout();

                        SaveButtonOnlySystem.SetHideMode(false);
                        EditorSystem.SetActive(false);
                    },
                tooltip: () => "Save current layout",
                onRightClick: () =>
                {
                    // cancel hide‐all mode and re‐enter edit
                    SaveButtonOnlySystem.SetHideMode(false);
                    EditorSystem.SetActive(true);
                },
                width: 100,
                height: 30
            );

            Append(saveButton);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}