using System;
using System.Collections.Generic;
using MonoMod.Cil;

namespace UICustomizer.Common.Systems.Hooks
{
    /// <summary>
    /// Accessories AND armor
    /// </summary>
    public class AccessoriesHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            IL_Main.DrawInventory += AccessoriesNog;
        }
        public override void Unload()
        {
            IL_Main.DrawInventory -= AccessoriesNog;
        }

        private void AccessoriesNog(ILContext il)
        {
            var c = new ILCursor(il);

            // Inject offset for armor
            InjectOffsetAtStloc(c, 88, nameof(OffsetX)); // Armor X
            InjectOffsetAtStloc(c, 89, nameof(OffsetY)); // Armor Y

            // Inject offset for vanity
            InjectOffsetAtStloc(c, 98, nameof(OffsetX)); // Vanity X
            InjectOffsetAtStloc(c, 99, nameof(OffsetY)); // Vanity Y
        }

        private static void InjectOffsetAtStloc(ILCursor c, int localIndex, string offsetFieldName)
        {
            if (c.TryGotoNext(MoveType.Before, i => i.MatchStloc(localIndex)))
            {
                c.EmitLdsfld(typeof(InventoryHook).GetField(offsetFieldName));
                c.EmitConvI4(); // Convert float to int32
                c.EmitAdd();
                Log.Info($"Successfully injected {offsetFieldName} at stloc.{localIndex}");
            }
            else
            {
                Log.Error($"Could not find stloc.{localIndex} for {offsetFieldName}");
            }
        }

        private void InjectAccessoriesOffset(ILContext il)
        {
            IL.Edit(il, c =>
            {
                int counter = 0;

                List<int> run =
                [
                    // 9, // Achievement button confirmed / top 3 armors?
                            // 10, // Another achievement button / top 3 armors?
                            11, // Map icon / top 3 dyes?
                            12,
                            13, // 3 armor/vanity slots
                            14, // 3 armor / dye slots or dye slots
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
