using System.Runtime.InteropServices;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    public class LogoColorHook : ModSystem
    {
        public static bool DrawLogo = true;
        //public override void Load() => IL_Main.DrawMenu += SkipDraw;
        //public override void Unload() => IL_Main.DrawMenu -= SkipDraw;
        private void SkipDraw(ILContext il)
        {
            IL.Edit(il, c =>
            {
                //       // if (MenuLoader.MenuOldVanilla.IsSelected)
                //       IL_0606: ldsfld class Terraria.ModLoader.Default.MenuOldVanilla Terraria.ModLoader.MenuLoader::MenuOldVanilla

                //       IL_060b: callvirt instance bool Terraria.ModLoader.ModMenu::get_IsSelected()
                //       IL_0610: brfalse IL_070b

                //   // Main.spriteBatch.Draw(TextureAssets.Logo3.Value, new Vector2(Main.screenWidth / 2, 100f), new Rectangle(0, 0, TextureAssets.Logo.Width(), TextureAssets.Logo.Height()), color2, this.logoRotation, new Vector2(TextureAssets.Logo.Width() / 2, TextureAssets.Logo.Height() / 2), this.logoScale, SpriteEffects.None, 0f);
                //       IL_0615: ldsfld class [FNA]
                //       Microsoft.Xna.Framework.Graphics.SpriteBatch Terraria.Main::spriteBatch

                //       IL_061a: ldsfld class [ReLogic] ReLogic.Content.Asset`1<class [FNA] Microsoft.Xna.Framework.Graphics.Texture2D> Terraria.GameContent.TextureAssets::Logo3
                //       IL_061f: callvirt instance !0 class [ReLogic] ReLogic.Content.Asset`1<class [FNA] Microsoft.Xna.Framework.Graphics.Texture2D>::get_Value()

                //   IL_0624: ldsfld int32 Terraria.Main::screenWidth
                //   IL_0629: ldc.i4.2
                //IL_062a: div
            });
        }
    }
}
