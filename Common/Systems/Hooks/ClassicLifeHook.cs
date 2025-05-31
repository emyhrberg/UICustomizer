using System;
using MonoMod.Cil;
using Terraria.GameContent.UI.ResourceSets;

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

                // Move health hearts and its text X
                // Find the 500(ldc.i4) and add OffsetX to it
                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdcI4(500)))
                {
                    c.EmitDelegate((int value) => value + (int)OffsetX);
                }

                // Move health hearts Y 
                // Find the 32f (ldc.r4) and add OffsetY to it
                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdcR4(32f)))
                {
                    c.EmitDelegate((float value) => value + OffsetY);
                }

                // Move text Y. Ldc.r4 6
                c.Index = 0; // Reset cursor index to start searching again
                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdcR4(6f)))
                {
                    c.EmitDelegate((float value) => value + OffsetY);
                }
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }
    }
}