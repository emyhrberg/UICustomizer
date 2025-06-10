using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using UICustomizer.Common.Configs;
using UICustomizer.Common.Systems;

namespace UICustomizer.UI.Editor
{
    public class EditButton : UIText
    {
        private float scale = 0.6f;

        public EditButton() : base("Edit", 0.6f, true)
        {
            Top.Set(-98, 1);
            Left.Set(-210, 1);

            Width.Set(200, 0);
            Height.Set(40, 0);

            TextOriginX = 0.5f;
            TextOriginY = 0.5f;

            OnLeftClick += (_, _) =>
            {
                if (!Conf.C.ShowEditButton) return;

                Main.LocalPlayer.mouseInterface = true; // prevents item use
                EditorSystem.ToggleActive();
            };
            OnMouseOver += (_, _) =>
            {
                if (!Conf.C.ShowEditButton) return;

                Main.LocalPlayer.mouseInterface = true; // prevents item use
                SoundEngine.PlaySound(SoundID.MenuTick);
            };
        }

        public override void Update(GameTime gameTime)
        {
            if (!Main.playerInventory) return;
            if (!Conf.C.ShowEditButton) return;

            base.Update(gameTime);

            if (IsMouseHovering && scale < 0.75f) scale += 0.02f;
            if (!IsMouseHovering && scale > 0.6f) scale -= 0.02f;

            SetText("Edit", scale, true);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Conf.C.ShowEditButton) return;
            if (!Main.playerInventory) return;

            Top.Set(-98, 1);
            Width.Set(200, 0);

            if (Main.LocalPlayer.extraAccessorySlots >= 1)
            {
                Top.Set(-80, 1);
                Height.Set(30, 0);
            }

            base.Draw(spriteBatch);
        }
    }
}
