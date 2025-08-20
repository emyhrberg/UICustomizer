using System;
using MonoMod.Cil;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks
{
    public class ClassicLifeHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            IL_ClassicPlayerResourcesDisplaySet.DrawLife += InjectOffset;
        }

        public override void Unload()
        {
            IL_ClassicPlayerResourcesDisplaySet.DrawLife -= InjectOffset;
        }

        private void InjectOffset(ILContext il)
        {
            try
            {
                ILCursor c = new(il);

                // Target ALL occurrences of 500 for X coordinates (both text and hearts)
                while (c.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(500)))
                {
                    c.Remove(); // Remove ldc.i4 500
                    c.EmitLdcI4(500);
                    c.EmitLdsfld(typeof(ClassicLifeHook).GetField(nameof(OffsetX)));
                    c.EmitConvI4();
                    c.EmitAdd();
                }

                // Target Y coordinates for text (6f) - look for specific pattern
                c.Index = 0;
                while (c.TryGotoNext(MoveType.Before,
                    i => i.MatchLdcR4(6f),
                    i => i.MatchNewobj<Vector2>()))
                {
                    c.Remove(); // Remove ldc.r4 6
                    c.EmitLdcR4(6f);
                    c.EmitLdsfld(typeof(ClassicLifeHook).GetField(nameof(OffsetY)));
                    c.EmitAdd();
                }

                // Target heart Y coordinate: 32f
                c.Index = 0;
                while (c.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(32f)))
                {
                    c.Remove(); // Remove ldc.r4 32
                    c.EmitLdcR4(32f);
                    c.EmitLdsfld(typeof(ClassicLifeHook).GetField(nameof(OffsetY)));
                    c.EmitAdd();
                }
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }
    }
}