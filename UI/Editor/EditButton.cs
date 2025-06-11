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
            Width.Set(60, 0);
            Height.Set(30, 0);
            Top.Set(-40, 1);
            Left.Set(-250, 1);

            TextOriginX = 0.5f;
            TextOriginY = 0.5f;

            OnLeftClick += (_, _) =>
            {
                if (!Conf.C.ShowEditButton) return;

                if (Conf.C.DisableItemUseWhileDragging)
                {
                    Main.LocalPlayer.mouseInterface = true;
                }
                EditorSystem.ToggleActive();
            };
            OnMouseOver += (_, _) =>
            {
                if (!Conf.C.ShowEditButton) return;

                if (Conf.C.DisableItemUseWhileDragging)
                {
                    Main.LocalPlayer.mouseInterface = true;
                }
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

            base.Draw(spriteBatch);
        }
    }
}
