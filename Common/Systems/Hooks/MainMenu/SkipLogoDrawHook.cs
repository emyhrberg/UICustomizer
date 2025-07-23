using MonoMod.Cil;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    public class SkipLogoDrawHook : ModSystem
    {
        public static bool IsDrawing = true;
        public override void Load() => IL_Main.DrawMenu += SkipDraw;
        public override void Unload() => IL_Main.DrawMenu -= SkipDraw;
        private void SkipDraw(ILContext il)
        {
            IL.Edit(il, c =>
            {
                // Match to if (MenuLoader.MenuOldVanilla.IsSelected)
                // Matching to Logo3

                c.GotoNext(MoveType.After,
                    i => i.MatchLdsfld(typeof(TextureAssets).GetField("Logo3"))
                );

                // Emit delegate with IsDrawing to skip drawing the logo
            });
        }
    }
}
