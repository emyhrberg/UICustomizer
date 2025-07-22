using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    public class SkipLogoDrawHook : ModSystem
    {
        public static bool DrawLogo = true;
        public override void Load() => IL_Main.DrawMenu += SkipDraw;
        public override void Unload() => IL_Main.DrawMenu -= SkipDraw;
        private void SkipDraw(ILContext il)
        {
            IL.Edit(il, c =>
            {

            });
        }
    }
}
