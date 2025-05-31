using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using UICustomizer.Common.Systems;

namespace UICustomizer.UI
{
    // Credits to ScalarVector1:
    // https://github.com/ScalarVector1/DragonLens/blob/master/Content/GUI/EmergencyCustomizeMenu.cs
    public class EditButton : UIText   // UIText already IS a UIElement
    {
        private float scale = 0.6f;

        public EditButton() : base("Edit UI", 0.6f, true)
        {
            Top.Set(-98, 1);   
            Left.Set(-210, 1);

            Width.Set(200, 0);
            Height.Set(40, 0);

            TextOriginX = 0.5f;
            TextOriginY = 0.5f;

            OnLeftClick += (_, _) =>
            {
                if (!UICustomizerSystem.EditModeActive)
                    UICustomizerSystem.EnterEditMode();
            };
            OnMouseOver += (_, _) => SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void Update(GameTime gameTime)
        {
            if (UICustomizerSystem.EditModeActive) return;

            //Height.Set(40, 0);
            //TextOriginX = 0.5f;
            //TextOriginY = 0.5f;
            //Top.Set(-98, 1);

            base.Update(gameTime);

            if (IsMouseHovering && scale < 0.75f) scale += 0.02f;
            if (!IsMouseHovering && scale > 0.6f) scale -= 0.02f;

            SetText("[Edit UI!]", scale, true);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (UICustomizerSystem.EditModeActive || !Main.playerInventory) return;
            base.Draw(spriteBatch);   // UIText already draws itself
        }
    }
}
