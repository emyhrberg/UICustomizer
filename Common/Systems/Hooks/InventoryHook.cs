using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using UICustomizer.Helpers;

namespace UICustomizer.Common.Systems.Hooks
{
    public class InventoryHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            IL_Main.DrawInventory += InventoryNog;
            IL_Main.DrawEmoteBubblesButton += InjectBestiaryIconOffset;
            IL_Main.DrawBestiaryIcon += InjectBestiaryIconOffset;
            IL_Main.DrawTrashItemSlot += InjectTrashItemOffset;
        }
        public override void Unload()
        {
            IL_Main.DrawInventory -= InventoryNog;
            IL_Main.DrawEmoteBubblesButton -= InjectBestiaryIconOffset;
            IL_Main.DrawBestiaryIcon -= InjectBestiaryIconOffset;
            IL_Main.DrawTrashItemSlot -= InjectTrashItemOffset;
        }

        private void InventoryNog(ILContext il)
        {
            IL.Edit(il, c =>
            {
                // Inventory string
                InjectOffsetAtLdcR4(c, 40f, nameof(OffsetX), true); // Inventory X
                InjectOffsetAtLdcR4(c, 0f, nameof(OffsetY), true); // Inventory Y

                // Main inventory slots
                InjectOffsetAtStloc(c, 34, nameof(OffsetX));  // Inventory X
                InjectOffsetAtStloc(c, 35, nameof(OffsetY));  // Inventory Y
                InjectOffsetAtStloc(c, 38, nameof(OffsetX));  // Achievement X
                InjectOffsetAtStloc(c, 39, nameof(OffsetY));  // Achievement Y

                // Coin slots
                InjectOffsetAtStloc(c, 159, nameof(OffsetX)); // Coin slots X
                InjectOffsetAtStloc(c, 160, nameof(OffsetY)); // Coin slots Y

                // Ammo slots
                InjectOffsetAtStloc(c, 163, nameof(OffsetX)); // Ammo slots X
                InjectOffsetAtStloc(c, 164, nameof(OffsetY)); // Ammo slots Y

                // Quickstack chest icon
                InjectOffsetAtStloc(c, 173, nameof(OffsetX)); // Quickstack X
                InjectOffsetAtStloc(c, 174, nameof(OffsetY)); // Quickstack Y

                // Text labels (hardcoded floats)
                InjectOffsetAtLdcR4(c, 496f, nameof(OffsetX)); // Coins text X
                InjectOffsetAtLdcR4(c, 532f, nameof(OffsetX)); // Ammo text X
                InjectOffsetAtLdcR4(c, 84f, nameof(OffsetY));  // Base Y for both texts

                // Sort inventory chests
                c.GotoNext(MoveType.Before, i => i.MatchStloc(25));
                c.GotoNext(MoveType.After, i => i.MatchCall(out _));

                c.EmitLdloc(25);
                c.EmitLdsfld(typeof(InventoryHook).GetField(nameof(OffsetX)));
                c.EmitConvI4();
                c.EmitAdd();
                c.EmitStloc(25);

                c.EmitLdloc(26);
                c.EmitLdsfld(typeof(InventoryHook).GetField(nameof(OffsetY)));
                c.EmitConvI4();
                c.EmitAdd();
                c.EmitStloc(26);
            });
        }

        private void InjectTrashItemOffset(ILContext il)
        {
            IL.Edit(il, c =>
            {
                // X offset: ldc.i4 448
                c.GotoNext(MoveType.After,
                    i => i.MatchLdcI4(448));
                {
                    c.EmitDelegate<Func<int, int>>(offset => offset + (int)OffsetX);
                }

                // Y offset: ldc.i4 258
                c.GotoNext(MoveType.After,
                    i => i.MatchLdcI4(258));
                {
                    c.EmitDelegate<Func<int, int>>(offset => offset + (int)OffsetY);
                }
            });
        }

        private static void InjectBestiaryIconOffset(ILContext il)
        {
            Log.Info("Injecting bestiary icon offset");

            IL.Edit(il, c =>
            {
                int counter = 0;
                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchCallOrCallvirt<Rectangle>(".ctor")))
                {
                    counter++;
                    Log.Info("Counter: " + counter);

                    if (counter == 2)
                    {
                        Log.Info("Found bestiary icon rectangle constructor");
                        // This is the rectangle for the bestiary icon
                        if (c.TryGotoPrev(i => i.MatchLdloca(out int index)))
                        {
                            int localIndex = -1;
                            if (c.TryGotoPrev(i => i.MatchLdloca(out localIndex)))
                            {
                                // Move forward to the second Rectangle constructor call
                                Log.Info("Found local index: " + localIndex);
                                c.GotoNext(MoveType.After, i => i.MatchCallOrCallvirt<Rectangle>(".ctor"));
                                c.GotoNext(MoveType.After, i => i.MatchCallOrCallvirt<Rectangle>(".ctor"));
                                Log.Info("Found bestiary icon rectangle constructor at local index: " + localIndex);
                            }

                            c.Emit(OpCodes.Ldloca_S, (byte)localIndex);
                            c.EmitDelegate((ref Rectangle r) =>
                            {
                                r = new Rectangle(
                                    r.X + (int)OffsetX,
                                    r.Y + (int)OffsetY,
                                    r.Width,
                                    r.Height
                                );
                            });
                        }
                    }
                }
            });
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

        private static void InjectOffsetAtLdcI4(ILCursor c, float num, string offsetFieldName)
        {
            c.Index = 0;
            while (c.TryGotoNext(MoveType.After, i => i.MatchLdcI4((int)num)))
            {
                c.EmitLdsfld(typeof(InventoryHook).GetField(offsetFieldName));
                c.EmitConvR4();
                c.EmitAdd();
                Log.Info($"Successfully injected {offsetFieldName} at ldc.i4 {num}");
            }
            c.Index = 0;
        }

        private static void InjectOffsetAtLdcR4(ILCursor c, float num, string offsetFieldName, bool specialInventoryString = false)
        {
            c.Index = 0;
            int counter = 0;
            while (c.TryGotoNext(MoveType.After, i => i.MatchLdcR4(num)))
            {
                if (counter >= 1 && specialInventoryString) break;

                c.EmitLdsfld(typeof(InventoryHook).GetField(offsetFieldName));
                c.EmitAdd();
                Log.Info($"Successfully injected {offsetFieldName} at ldc.r4 {num}");
                counter++;
            }
            c.Index = 0;
        }
    }
}
