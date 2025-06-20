using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    public class MainMenuHoverTextColorHook : ModSystem
    {
        // Reference Main.OurFavoriteColor (255,231,69)
        public static float R = 255f;
        public static float G = 231f;
        public static float B = 69f;
        public override void Load() => IL_Main.DrawMenu += HoverRGB;
        public override void Unload() => IL_Main.DrawMenu -= HoverRGB;
        private void HoverRGB(ILContext il)
        {
            IL.Edit(il, c =>
            {
                //  c.GotoNext(MoveType.After, i => i.MatchLdcR4(255f),
                //             i => i.MatchLdloc(186));
                //c.EmitPop();
                //c.EmitLdsfld(typeof(MainMenuHoverTextColorHook).GetField(nameof(R)));
                //Log.Info($"R value set to: {R} at index {c.Index}");

                //c.GotoNext(MoveType.After, i => i.MatchLdcR4(215f));
                //c.Index++;
                //c.EmitPop();
                //c.EmitLdsfld(typeof(MainMenuHoverTextColorHook).GetField(nameof(G)));
                //Log.Info($"G value set to: {G} at index {c.Index}");

                //c.GotoNext(MoveType.After, i => i.MatchLdcR4(0f),
                //           i => i.MatchLdloc(186));
                //c.Index++;
                //c.EmitPop();
                //c.EmitLdsfld(typeof(MainMenuHoverTextColorHook).GetField(nameof(B)));
                //Log.Info($"B value set to: {B} at index {c.Index}");
            });
        }
    }
}
