using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using UICustomizer.EditMode.System;

namespace UICustomizer.EditMode.UI
{
    public class EditToggleButton : UIImage
    {
        public EditToggleButton() : base(Ass.EditorIcon)
        {
            // Size
            ImageScale = 1f;
            Top.Set(260, 0);
            Left.Set(Main.GameMode == GameModeID.Creative ? 65 : 30, 0);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            var sys = ModContent.GetInstance<EditSystem>();
            sys.Toggle();
        }

        public override void Draw(SpriteBatch sb)
        {
            if (!Main.playerInventory) return;

            base.Draw(sb);

            Rectangle r = new(74, 268, 28, 28);
            //sb.Draw(TextureAssets.MagicPixel.Value, r, Color.White*0.5f); 
            bool hover = r.Contains(Main.MouseScreen.ToPoint());

            if (hover)
            {
                SetImage(Ass.EditorIconSmallHover);
                Main.LocalPlayer.mouseInterface = true; // disable item use

                var sys = ModContent.GetInstance<EditSystem>();
                if (sys.Enabled)
                    Main.hoverItemName = "Close UI Editor";
                else
                    Main.hoverItemName = "Open UI Editor";
            }
            else
                SetImage(Ass.EditorIconSmall);
        }
    }
}