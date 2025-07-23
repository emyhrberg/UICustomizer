using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    public class MainMenuOutlineTextColorHook : ModSystem
    {
        public static Color Color;
        public override void Load() => Main.QueueMainThreadAction(() => IL_Main.DrawMenu += EditAllMenuTextColors);
        public override void Unload() => Main.QueueMainThreadAction(() => IL_Main.DrawMenu -= EditAllMenuTextColors);
        private void EditAllMenuTextColors(ILContext il)
        {
            IL.Edit(il, c =>
            {
                c.GotoNext(MoveType.Before, i => i.MatchStloc(177));
                c.EmitPop();
                c.EmitLdsfld(typeof(MainMenuOutlineTextColorHook).GetField(nameof(Color)));
            });
        }
    }
}
