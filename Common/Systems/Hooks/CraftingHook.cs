using System;
using System.Collections.Generic;
using MonoMod.Cil;

namespace UICustomizer.Common.Systems.Hooks
{
    public class CraftingHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            IL_Main.DrawInventory += InjectInventoryOffset;
        }
        public override void Unload()
        {
            IL_Main.DrawInventory -= InjectInventoryOffset;
        }

        private void InjectInventoryOffset(ILContext il)
        {
            IL.Edit(il, c =>
            {
                int counter = 0;

                List<int> run =
                [
                    // 9, // Achievement button confirmed / top 3 armors?
                            // 10, // Another achievement button / top 3 armors?
                            // 11, // Map icon / top 3 dyes?
                            // 12,
                            // 13, // 3 armor/vanity slots
                            // 14, // 3 armor / dye slots or dye slots
                            // 15, // item slot dye 3
                                // 16, // item slot dye 3?
                                // 17,
                                // 23,24,25,26,27,28,29,30 // crafting related. one of these is coins though
                            23,24,25,26,27,28,29 // crafting related.
                ];

                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchNewobj<Vector2>()))
                {
                    counter++;

                    if (run.Contains(counter))
                    {
                        c.EmitDelegate<Func<Vector2, Vector2>>(pos =>
                    {
                        return new Vector2(pos.X + OffsetX, pos.Y + OffsetY);
                    });
                    }
                }
            });
        }

    }
}
