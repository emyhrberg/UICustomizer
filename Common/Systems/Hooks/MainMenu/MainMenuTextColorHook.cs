using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    public class MainMenuTextColorHook : ModSystem
    {
        public static Color MainMenuTextColor;
        public override void Load() => IL_Main.DrawMenu += NogMenu;
        public override void Unload() => IL_Main.DrawMenu -= NogMenu;
        private void NogMenu(ILContext il)
        {
            IL.Edit(il, c =>
            {
                c.GotoNext(MoveType.Before, i => i.MatchStloc(177));
                c.EmitPop();
                c.EmitLdsfld(typeof(MainMenuTextColorHook).GetField(nameof(MainMenuTextColor)));
            });
        }
    }
}
