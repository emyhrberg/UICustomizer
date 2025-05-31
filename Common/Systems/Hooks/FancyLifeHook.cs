using System;
using MonoMod.Cil;
using Terraria.GameContent.UI.ResourceSets;

namespace UICustomizer.Common.Systems.Hooks
{
    public class FancyLifeHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            IL_FancyClassicPlayerResourcesDisplaySet.DrawLifeBarText += InjectOffset;
            IL_FancyClassicPlayerResourcesDisplaySet.DrawLifeBar += InjectOffset;
        }

        public override void Unload()
        {
            IL_FancyClassicPlayerResourcesDisplaySet.DrawLifeBarText -= InjectOffset;
            IL_FancyClassicPlayerResourcesDisplaySet.DrawLifeBar -= InjectOffset;
        }

        private void InjectOffset(ILContext il)
        {
            try
            {
                ILCursor c = new(il);

                // Find FIRST 4, with instruction ldc.i4
                if (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdcI4(4)))
                {
                    c.EmitDelegate((int value) => (int)(value + OffsetX));
                }

                // Find FIRST 15f, with ldc.r4
                if (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdcR4(15f)))
                {
                    c.EmitDelegate((float value) => value + OffsetY);
                }

                // while (c.TryGotoNext(MoveType.After,
                //     i => i.MatchNewobj<Vector2>()))
                // {
                //     c.EmitDelegate((Vector2 pos) =>
                //     {
                //         return new Vector2(
                //             pos.X + OffsetX,
                //             pos.Y + OffsetY
                //         );
                //     });
                // }
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }
    }
}