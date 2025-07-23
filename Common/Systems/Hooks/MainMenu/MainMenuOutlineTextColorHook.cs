using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using UICustomizer.Common.Configs;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    public class MainMenuOutlineTextColorHook : ModSystem
    {
        public static Color OutlineColor;
        public override void Load()
        {
            if (OutlineColor == default)
            {
                //Log.Warn("MainMenuFillTextColorHook.OutlineColor is default, skipping color modification.");
                return;
            }

            // If config is not default, use it
            OutlineColor = ColorHelper.HexToColor(Conf.C.OutlineColor);

            Main.QueueMainThreadAction(() => IL_Main.DrawMenu += EditAllMenuTextColors);
        }
        public override void Unload()
        {

            Main.QueueMainThreadAction(() => IL_Main.DrawMenu -= EditAllMenuTextColors);
        }
        private void EditAllMenuTextColors(ILContext il)
        {
            IL.Edit(il, c =>
            {
                c.GotoNext(MoveType.Before, i => i.MatchStloc(177));
                c.EmitPop();
                c.EmitLdsfld(typeof(MainMenuOutlineTextColorHook).GetField(nameof(OutlineColor)));
            });
        }
    }
}
