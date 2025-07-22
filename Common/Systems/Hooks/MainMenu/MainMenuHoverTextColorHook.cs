using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    internal class MainMenuHoverTextColorHook : ModSystem
    {
        private static float R;
        private static float G;
        private static float B=0;
        public static Color Color = Main.OurFavoriteColor; // yellow default!
        public override void Load() => Main.QueueMainThreadAction(() => IL_Main.DrawMenu += ModifyHoverTextColor);
        public override void Unload() => Main.QueueMainThreadAction(() => IL_Main.DrawMenu -= ModifyHoverTextColor);
        private void ModifyHoverTextColor(ILContext il)
        {
            IL.Edit(il, c =>
            {
                //c.GotoNext(MoveType.After, 
                //            i => i.MatchLdcR4(255f),
                //            i => i.MatchLdloc(186));
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
                //c.EmitLdsfld(typeof(MainMenuOutlineTextColor).GetField(nameof(B)));
                //Log.Info($"B value set to: {B} at index {c.Index}");
            });
        }

        public override void PostUpdateEverything()
        {
            Color = new(R, G, B);
            base.PostUpdateEverything();
        }
    }
}
