using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.GameContent.UI.ResourceSets;

namespace UICustomizer.Common.Systems.Hooks
{
    public class FancyManaHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            IL_FancyClassicPlayerResourcesDisplaySet.DrawManaBar += InjectManaBarOffset;
            IL_FancyClassicPlayerResourcesDisplaySet.DrawManaText += InjectManaTextOffset;
        }

        public override void Unload()
        {
            IL_FancyClassicPlayerResourcesDisplaySet.DrawManaBar -= InjectManaBarOffset;
            IL_FancyClassicPlayerResourcesDisplaySet.DrawManaText -= InjectManaTextOffset;
        }

        private void InjectManaBarOffset(ILContext il)
        {
            try
            {
                ILCursor c = new(il);

                // Hook the Vector2 creation: Vector2 vector = new Vector2(Main.screenWidth - 40, 22f);
                // The IL pattern is: ldloca.s 2, ldsfld screenWidth, ldc.i4.s 40, sub, conv.r4, ldc.r4 22, call Vector2::.ctor
                if (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdloca(2), // ldloca.s 2 (address of vector local variable)
                    i => i.MatchLdsfld("Terraria.Main", "screenWidth"),
                    i => i.MatchLdcI4(40),
                    i => i.MatchSub(),
                    i => i.MatchConvR4(),
                    i => i.MatchLdcR4(22f),
                    i => i.MatchCall<Vector2>(".ctor"))) // Vector2 constructor call
                {
                    // After the constructor call, the vector is stored in local 2
                    // We need to load it, modify it, and store it back
                    c.Emit(OpCodes.Ldloc_S, (byte)2);
                    c.EmitDelegate<Func<Vector2, Vector2>>(vec =>
                        new Vector2(vec.X + OffsetX, vec.Y + OffsetY));
                    c.Emit(OpCodes.Stloc_S, (byte)2);
                    // Log.Info("Successfully hooked FancyClassic mana bar position");
                }
                else
                {
                    // Log.Warn("Failed to find FancyClassic mana bar position");
                }
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }

        private void InjectManaTextOffset(ILContext il)
        {
            try
            {
                ILCursor c = new(il);

                // Hook the Vector2 creation in DrawString: new Vector2(Main.screenWidth - num, 6f)
                if (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdsfld("Terraria.Main", "screenWidth"),
                    i => i.MatchLdloc(2), // num variable
                    i => i.MatchSub(),
                    i => i.MatchConvR4(),
                    i => i.MatchLdcR4(6f),
                    i => i.MatchNewobj<Vector2>()))
                {
                    c.EmitDelegate<Func<Vector2, Vector2>>(vec =>
                        new Vector2(vec.X + OffsetX, vec.Y + OffsetY));
                    // Log.Info("Successfully hooked FancyClassic mana text position");
                }
                else
                {
                    // Log.Warn("Failed to find FancyClassic mana text position");
                }
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }
    }
}
