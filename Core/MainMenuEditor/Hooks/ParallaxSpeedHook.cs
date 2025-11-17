using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using UIEditor.Core.Helpers;

namespace UIEditor.Core.MainMenuEditor.Hooks
{
    public class ParallaxSpeedHook : ModSystem
    {
        public static float Speed = 5;

        public override void Load()
        {
            Speed = Conf.C?.MainMenuTime.ParallaxSpeed ?? 5f;

            IL_Main.DrawMenu += ModifyParallaxSpeed;
        }
        public override void Unload()
        {
            IL_Main.DrawMenu -= ModifyParallaxSpeed;
        }
        private void ModifyParallaxSpeed(ILContext il)
        {
            IL.Edit(il, c =>
            {
                c.GotoNext(MoveType.Before,
                    i => i.MatchStsfld<Main>(nameof(Main.MenuXMovement)));

                c.EmitPop();

                c.EmitDelegate(() => Speed);
            });
        }
    }
}
