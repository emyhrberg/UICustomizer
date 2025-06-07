// using System;
// using System.Collections.Generic;
// using System.Reflection;
// using Microsoft.Xna.Framework.Graphics;
// using Mono.Cecil.Cil;
// using MonoMod.Cil;

// namespace UICustomizer.Common.Systems.Hooks
// {
//     public class OldInventoryHook : ModSystem
//     {
//         public static float OffsetX = 0;
//         public static float OffsetY = 150;

//         public override void Load()
//         {
//             // Working
//             IL_Main.DrawEmoteBubblesButton += InjectBestiaryIconOffset;
//             IL_Main.DrawBestiaryIcon += InjectBestiaryIconOffset;
//             IL_Main.DrawTrashItemSlot += InjectTrashItemOffset;
//             IL_Main.DrawInventory += InjectInventoryOffset;

//             // In progress
//             // IL_Main.DrawInventory += InjectDrawOnlyOffset;

//             // IL_Main.DrawInventory += Nog;
//         }
//         public override void Unload()
//         {
//             // // Working
//             IL_Main.DrawEmoteBubblesButton -= InjectBestiaryIconOffset;
//             IL_Main.DrawBestiaryIcon -= InjectBestiaryIconOffset;
//             IL_Main.DrawTrashItemSlot -= InjectTrashItemOffset;
//             IL_Main.DrawInventory -= InjectInventoryOffset;

//             // // In progress
//             // IL_Main.DrawInventory -= InjectDrawOnlyOffset;

//             // IL_Main.DrawInventory -= Nog;
//         }

//         private void Nog(ILContext il)
//         {
//             Log.Info("Injecting nog offset");

//             IL.Edit(il, c =>
//             {
//                 c.GotoNext(MoveType.Before, i => i.MatchStloc(34));
//                 var offsetXField = typeof(InventoryHook).GetField(nameof(OffsetX));
//                 c.EmitLdsfld(offsetXField);
//                 c.EmitAdd();

//                 c.GotoNext(MoveType.Before, i => i.MatchStloc(35));
//                 var offsetYField = typeof(InventoryHook).GetField(nameof(OffsetY));
//                 c.EmitLdsfld(offsetYField);
//                 c.EmitAdd();
//             });
//         }

//         private void InjectDrawOnlyOffset(ILContext il)
//         {
//             var c = new ILCursor(il);
//             int count = 0;

//             while (c.TryGotoNext(MoveType.After,
//                 i => i.MatchCall<ItemSlot>(nameof(ItemSlot.Draw))))
//             {
//                 count++;
//                 Log.Info($"Found ItemSlot.Draw at position {c.Index} (count: {count})");

//                 if (c.TryGotoPrev(i => i.MatchNewobj<Vector2>()))
//                 {
//                     c.EmitDelegate((Vector2 pos) =>
//                     {
//                         return pos + new Vector2(OffsetX, OffsetY);
//                     });
//                 }
//                 else
//                 {
//                     break; // Exit the loop if no matching newobj<Vector2> is found
//                 }

//                 c.GotoNext(MoveType.After, i => i.MatchCall<ItemSlot>(nameof(ItemSlot.Draw)));
//             }
//         }

//         private void InjectInventoryOffset(ILContext il)
//         {
//             IL.Edit(il, c =>
//             {
//                 int counter = 0;
//                 List<int> info =
//                 [
//                     // 1,2,3 // Father to a lot of building vector2s that affect everything with an unwanted offset
//                     4, // "Inventory" header string
//                     5, // ALL ItemSlots (5x10)
//                     6,7,8, // Radial DPad?
//                     9, // Achievement button confirmed
//                        // 10, // Another achievement button
//                        // 11, // Map icon
//                         12, // Link point navigator
//                         13, // Item slot again?
//                         14, // Item slot hover?
//                        // 15, // Item slot dye top 3 confirmed!   
//                        16, // Idk item slot
//                        // 17 Achieve pos
//                         18, // Link point navigator again
//                         19, // Chat manager, and draw savings before that
//                         20, // Reforge?
//                         21, // Link point navigator again
//                        22, // Chat
//                        // 23 Reforge
//                        // 24 Guide craft menu
//                        // 25 Available recipes (probably hammer)
//                        // 26 Available recipes (probably selected item)
//                        // 27 Available recipes again (probably hammer/preview crafted item)
//                        // 28 Crafting window
//                        // 29 Craft up (NO. probably Crafting Window)
//                        // 30 Craft down (NO. probably Coins string)
//                        // 31 Recipes
//                        32, // Coins text confirmed!
//                        // 33 -
//                        34, // String
//                         35, // Item slot
//                        36, // Item slot
//                         37, // Link point navigator
//                        38, // Chest stack
//                         39, // Link point navigator
//                     40, // Sort mouse over
//                 ];

//                 List<int> skip =
//                 [
//                     9, // Achievement button confirmed / top 3 armors?
//                     10, // Another achievement button / top 3 armors?
//                     11, // Map icon / top 3 dyes?
//                     12,
//                     13, // 3 armor/vanity slots
//                     14, // 3 armor / dye slots or dye slots
//                     15, // item slot dye 3
//                         16, // item slot dye 3?
//                         // 17,
//                         // 23,24,25,26,27,28,29,30 // crafting related. one of these is coins though
//                     23,24,25,26,27,28,29 // crafting related.
//                 ];

//                 while (c.TryGotoNext(MoveType.After,
//                     i => i.MatchNewobj<Vector2>()))
//                 {
//                     counter++;

//                     if (skip.Contains(counter))
//                     {
//                         Log.Info("Skipping vector2 at counter: " + counter);
//                         continue;
//                     }

//                     c.EmitDelegate<Func<Vector2, Vector2>>(pos =>
//                     {
//                         return new Vector2(pos.X + OffsetX, pos.Y + OffsetY);
//                     });
//                 }
//             });
//         }

//         private static void InjectBestiaryIconOffset(ILContext il)
//         {
//             Log.Info("Injecting bestiary icon offset");

//             IL.Edit(il, c =>
//             {
//                 int counter = 0;
//                 while (c.TryGotoNext(MoveType.After,
//                     i => i.MatchCallOrCallvirt<Rectangle>(".ctor")))
//                 {
//                     counter++;
//                     Log.Info("Counter: " + counter);

//                     if (counter == 2)
//                     {
//                         Log.Info("Found bestiary icon rectangle constructor");
//                         // This is the rectangle for the bestiary icon
//                         if (c.TryGotoPrev(i => i.MatchLdloca(out int index)))
//                         {
//                             int localIndex = -1;
//                             if (c.TryGotoPrev(i => i.MatchLdloca(out localIndex)))
//                             {
//                                 // Move forward to the second Rectangle constructor call
//                                 Log.Info("Found local index: " + localIndex);
//                                 c.GotoNext(MoveType.After, i => i.MatchCallOrCallvirt<Rectangle>(".ctor"));
//                                 c.GotoNext(MoveType.After, i => i.MatchCallOrCallvirt<Rectangle>(".ctor"));
//                                 Log.Info("Found bestiary icon rectangle constructor at local index: " + localIndex);
//                             }

//                             c.Emit(OpCodes.Ldloca_S, (byte)localIndex);
//                             c.EmitDelegate((ref Rectangle r) =>
//                             {
//                                 r = new Rectangle(
//                                     r.X + (int)OffsetX,
//                                     r.Y + (int)OffsetY,
//                                     r.Width,
//                                     r.Height
//                                 );
//                             });
//                         }
//                     }
//                 }
//             });
//         }

//         // Works fine to my eyes.
//         private void InjectTrashItemOffset(ILContext il)
//         {
//             IL.Edit(il, c =>
//             {
//                 // X offset: ldc.i4 448
//                 c.GotoNext(MoveType.After,
//                     i => i.MatchLdcI4(448));
//                 {
//                     c.EmitDelegate<Func<int, int>>(offset => offset + (int)OffsetX);
//                 }

//                 // Y offset: ldc.i4 258
//                 c.GotoNext(MoveType.After,
//                     i => i.MatchLdcI4(258));
//                 {
//                     c.EmitDelegate<Func<int, int>>(offset => offset + (int)OffsetY);
//                 }
//             });
//         }

//         [Obsolete("Use InjectOffset instead.")]
//         private static void Obs_InjectNum0Offset(ILContext il)
//         {
//             IL.Edit(il, c =>
//             {
//                 int counter = 0;

//                 while (c.TryGotoNext(MoveType.After,
//                     i => i.MatchLdcI4(0)))
//                 {
//                     counter++;

//                     if (counter > 2)
//                     {
//                         // Skip after the first 3 ldc.i4(0) instructions
//                         Log.Info("Skipping ldc.i4(0) at counter: " + counter);
//                         continue;
//                     }

//                     c.EmitDelegate<Func<int, int>>(offset =>
//                     {
//                         // Add the offset to the X and Y coordinates
//                         return offset + (int)OffsetX;
//                     });
//                 }
//             });
//         }


//         [Obsolete("Use InjectBestiaryIconOffset instead.")]
//         private void Obs_InjectEmoteItemOffset(ILContext il)
//         {
//             IL.Edit(il, c =>
//             {
//                 // add to ldloca.s 5
//                 c.GotoNext(MoveType.After,
//                     i => i.MatchLdloca(out _),
//                     i => i.MatchCallOrCallvirt<Rectangle>("get_Center"),
//                     i => i.MatchCall(typeof(Utils).FullName, nameof(Utils.ToVector2)));

//                 c.EmitDelegate<Func<Vector2, Vector2>>(pos =>
//                 {
//                     return new Vector2(pos.X + OffsetX, pos.Y + OffsetY);
//                 });
//             });
//         }


//         [Obsolete("This was too much offset...")]
//         private void Obs_InjectHoverOffset(ILContext il)
//         {
//             IL.Edit(il, c =>
//             {
//                 // Add to num7 ldc.r4 20 #3. while loop it
//                 int counter = 0;
//                 while (c.TryGotoNext(MoveType.After,
//                     i => i.MatchLdcR4(20f)))
//                 {
//                     counter++;

//                     if (counter == 3)
//                     {
//                         // Skip after the first 3 ldc.r4(20f) instructions
//                         Log.Info("Found ldc.r4(20f) at counter: " + counter);
//                         c.EmitDelegate<Func<float, float>>(value =>
//                         {
//                             // Add the offset to the X and Y coordinates
//                             return value + OffsetX;
//                         });
//                     }
//                     else
//                     {
//                         Log.Info("Skipping ldc.r4(20f) at counter: " + counter);
//                     }
//                 }
//             });
//         }
//     }
// }