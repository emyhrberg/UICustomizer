using System;
using MonoMod.Cil;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks
{
    public class ClassicManaHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            IL_ClassicPlayerResourcesDisplaySet.DrawMana += InjectOffset;
        }

        public override void Unload()
        {
            IL_ClassicPlayerResourcesDisplaySet.DrawMana -= InjectOffset;
        }

        private void InjectOffset(ILContext il)
        {
            try
            {
                ILCursor c = new(il);

                // Find 800 ldc.i4 and add OffsetX
                // Moves mana text X
                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdcI4(800)))
                {
                    c.EmitDelegate((int value) =>
                    {
                        return (int)(value + OffsetX);
                    });
                }

                // Find 6 ldc.r4
                // Moves mana text Y
                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdcR4(6f)))
                {
                    c.EmitDelegate((float value) =>
                    {
                        return value + OffsetY;
                    });
                }

                // 775 ldc.i4
                // Moves mana stars X
                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdcI4(775)))
                {
                    c.EmitDelegate((int value) =>
                    {
                        return (int)(value + OffsetX);
                    });
                }

                // ldc.i4.s 30
                // Moves mana stars Y
                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdcI4(30)))
                {
                    c.EmitDelegate((int value) =>
                    {
                        return value + (int)OffsetY;
                    });
                }
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }
    }
}