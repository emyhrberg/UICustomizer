using System;
using MonoMod.Cil;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks
{
    public class BarManaTextHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            IL_HorizontalBarsPlayerResourcesDisplaySet.DrawManaText += InjectManaTextOffset;
        }

        public override void Unload()
        {
            IL_HorizontalBarsPlayerResourcesDisplaySet.DrawManaText -= InjectManaTextOffset;
        }

        private void InjectManaTextOffset(ILContext il)
        {
            try
            {
                ILCursor c = new(il);

                // Target: Vector2 vector = new Vector2(Main.screenWidth - num, 65f);
                // The IL pattern is: ldloca.s 5, ldsfld screenWidth, ldloc.1, sub, conv.r4, ldc.r4 65, call Vector2::.ctor
                if (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdloca(5), // ldloca.s 5 (address of vector local variable)
                    i => i.MatchLdsfld("Terraria.Main", "screenWidth"),
                    i => i.MatchLdloc(1), // num (180)
                    i => i.MatchSub(),
                    i => i.MatchConvR4(),
                    i => i.MatchLdcR4(65f),
                    i => i.MatchCall<Vector2>(".ctor"))) // Vector2 constructor call
                {
                    // After the constructor call, the vector is stored in local 5
                    // We need to load it, modify it, and store it back
                    c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc_S, (byte)5);
                    c.EmitDelegate<Func<Vector2, Vector2>>(vec =>
                        new Vector2(vec.X + OffsetX, vec.Y + OffsetY));
                    c.Emit(Mono.Cecil.Cil.OpCodes.Stloc_S, (byte)5);
                    // Log.Info("Successfully hooked mana text position");
                }
                else
                {
                    // Log.Warn("Failed to find mana text position");
                }
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }
    }
}