using System;
using MonoMod.Cil;
using Terraria.GameContent.UI.ResourceSets;

namespace UICustomizer.Common.Systems.Hooks
{
    public class HorizontalLifeBarHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            IL_HorizontalBarsPlayerResourcesDisplaySet.Draw += InjectOffset;
            IL_HorizontalBarsPlayerResourcesDisplaySet.DrawManaText += InjectManaOffset;
        }

        public override void Unload()
        {
            IL_HorizontalBarsPlayerResourcesDisplaySet.Draw -= InjectOffset;
            IL_HorizontalBarsPlayerResourcesDisplaySet.DrawManaText -= InjectManaOffset;
        }

        private void InjectOffset(ILContext il)
        {
            try
            {
                ILCursor c = new(il);

                // Helper comment:
                // 16, ldc.i4.s is X for all except mana text
                // 18 ldc.i4.s is Y for all except mana text
                // 180 ldc.i4 in DrawManaText is X for mana text
                // 65 ldc.r4 in DrawManaText is Y for mana text

                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdcI4(16)))
                {
                    Log.Info($"Injecting X offset: {OffsetX}");
                    c.EmitDelegate((int value) => value + (int)OffsetX);
                }

                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdcI4(18)))
                {
                    Log.Info($"Injecting Y offset: {OffsetY}");
                    c.EmitDelegate((int value) => value + (int)OffsetY);
                }
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }

        private void InjectManaOffset(ILContext il)
        {
            try
            {
                ILCursor c = new(il);

                // Helper comment:
                // 180 ldc.i4 in DrawManaText is X for mana text
                // 65 ldc.r4 in DrawManaText is Y for mana text

                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdcI4(180)))
                {
                    c.EmitDelegate((int value) => value + (int)OffsetX);
                }
                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdcR4(65f)))
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