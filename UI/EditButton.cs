using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using UICustomizer.Common.Systems;

namespace UICustomizer.UI
{
    public class EditButton : UIText  
    {
        private float scale = 0.6f;

        public EditButton() : base("UI Editor", 0.6f, true)
        {
            Top.Set(-98, 1);
            Left.Set(-210, 1);

            Width.Set(200, 0);
            Height.Set(40, 0);

            TextOriginX = 0.5f;
            TextOriginY = 0.5f;

            OnLeftClick += (_, _) =>
            {
                Main.LocalPlayer.mouseInterface = true; // prevents item use
                if (!UICustomizerSystem.EditModeActive)
                    UICustomizerSystem.EnterEditMode();
            };
            OnMouseOver += (_, _) =>
            {
                Main.LocalPlayer.mouseInterface = true; // prevents item use
                SoundEngine.PlaySound(SoundID.MenuTick);
            };
        }

        public override void Update(GameTime gameTime)
        {
            //if (ModLoader.TryGetMod("DragonLens", out _)) return;

            if (UICustomizerSystem.EditModeActive) return;

            //Height.Set(40, 0);
            //TextOriginX = 0.5f;
            //TextOriginY = 0.5f;
            //Top.Set(-98, 1);

            base.Update(gameTime);

            if (IsMouseHovering && scale < 0.75f) scale += 0.02f;
            if (!IsMouseHovering && scale > 0.6f) scale -= 0.02f;

            SetText("UI Editor", scale, true);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //if (ModLoader.TryGetMod("DragonLens", out _)) return;

            Top.Set(-98, 1);
            Width.Set(200, 0);

            //if (Player.SupportedSlotsAccs > 6)
            if (Main.LocalPlayer.extraAccessorySlots >= 1)
            {
                Top.Set(-80, 1);
                Height.Set(30, 0);
            }

            if (UICustomizerSystem.EditModeActive || !Main.playerInventory) return;
            base.Draw(spriteBatch);  
        }
    }
}
