using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using UICustomizer.Helpers;

namespace UICustomizer.Common.Systems.Hooks
{
    /// <summary>
    /// Accessories AND armor
    /// </summary>
    public class AccessoriesHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        private delegate bool orig_SetDrawLocation(AccessorySlotLoader self, int trueSlot, int skip, ref int xLoc, ref int yLoc);

        public override void Load()
        {
            IL_Main.DrawInventory += InjectArmor3Slots;

            MethodInfo methodInfo = typeof(AccessorySlotLoader).GetMethod("SetDrawLocation", BindingFlags.Instance | BindingFlags.NonPublic);
            MonoModHooks.Add(methodInfo, OnSetDrawLocation);

            IL_Main.DrawLoadoutButtons += InjectLoadoutsOffset;
            IL_Main.DrawNPCHousesInUI += InjectNPCHousesOffset;
            IL_Main.DrawPageIcons += InjectPageIconsOffset;
            IL_Main.DrawInventory += InjectEquipSlotsOffset;
        }

        public override void Unload()
        {
            IL_Main.DrawInventory -= InjectArmor3Slots;
            IL_Main.DrawLoadoutButtons -= InjectLoadoutsOffset;
            IL_Main.DrawNPCHousesInUI -= InjectNPCHousesOffset;
            IL_Main.DrawPageIcons -= InjectPageIconsOffset;
            IL_Main.DrawInventory -= InjectEquipSlotsOffset;
        }

        private static void InjectEquipSlotsOffset(ILContext il)
        {
            var c = new ILCursor(il);

            // Target the buff base position adjustments
            if (c.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(247))) // num24 += 247
            {
                c.Remove();
                c.EmitLdcI4(247);
                c.EmitLdsfld(typeof(AccessoriesHook).GetField(nameof(OffsetY)));
                c.EmitConvI4();
                c.EmitAdd();
            }

            if (c.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(8))) // num23 += 8  
            {
                c.Remove();
                c.EmitLdcI4(8);
                c.EmitLdsfld(typeof(AccessoriesHook).GetField(nameof(OffsetX)));
                c.EmitConvI4();
                c.EmitAdd();
            }
            // reset
            c.Index = 0;

            // Target: r.X = num23 + l * -47;
            // Find where the rectangle X is calculated and stored
            if (c.TryGotoNext(MoveType.Before,
                i => i.MatchStfld<Rectangle>("X")))
            {
                c.EmitLdsfld(typeof(AccessoriesHook).GetField(nameof(OffsetX)));
                c.EmitConvI4();
                c.EmitAdd();
            }

            // Target: r.Y = num24 + m * 47;
            // Find where the rectangle Y is calculated and stored
            if (c.TryGotoNext(MoveType.Before,
                i => i.MatchStfld<Rectangle>("Y")))
            {
                c.EmitLdsfld(typeof(AccessoriesHook).GetField(nameof(OffsetY)));
                c.EmitConvI4();
                c.EmitAdd();
            }
        }

        private static void InjectPageIconsOffset(ILContext il)
        {
            var c = new ILCursor(il);

            // Target: vector.X += 82f; (the X modification that happens after vector creation)
            if (c.TryGotoNext(MoveType.After,
                i => i.MatchLdcR4(82f)))  // ldc.r4 82
            {
                c.EmitLdsfld(typeof(AccessoriesHook).GetField(nameof(OffsetX)));
                c.EmitAdd(); // Add our offset to the 82f constant
            }

            // For Y offset, target the yPos parameter in the Vector2 constructor
            c.Index = 0;
            if (c.TryGotoNext(MoveType.Before,
                i => i.MatchLdarg(0),     // ldarg.0 (yPos)
                i => i.MatchConvR4(),     // conv.r4  
                i => i.MatchCall<Vector2>(".ctor"))) // Vector2 constructor
            {
                c.Index++; // Move past ldarg.0
                c.EmitLdsfld(typeof(AccessoriesHook).GetField(nameof(OffsetY)));
                c.EmitConvI4();
                c.EmitAdd();
            }
        }

        private static void InjectNPCHousesOffset(ILContext il)
        {
            var c = new ILCursor(il);

            // Target all occurrences of num7 (stloc.13) - X coordinate
            while (c.TryGotoNext(MoveType.Before, i => i.MatchStloc(13)))
            {
                c.EmitLdsfld(typeof(AccessoriesHook).GetField(nameof(OffsetX)));
                c.EmitConvI4();
                c.EmitAdd();
                c.Index++; // Move past the stloc to find the next one
            }

            // Reset cursor and target all occurrences of num8 (stloc.14) - Y coordinate  
            c.Index = 0;
            while (c.TryGotoNext(MoveType.Before, i => i.MatchStloc(14)))
            {
                c.EmitLdsfld(typeof(AccessoriesHook).GetField(nameof(OffsetY)));
                c.EmitConvI4();
                c.EmitAdd();
                c.Index++; // Move past the stloc to find the next one
            }
        }

        private static void InjectLoadoutsOffset(ILContext il)
        {
            var c = new ILCursor(il);

            // Target: int x = Main.screenWidth - 58 + 14;
            // Find where X coordinate is stored (stloc.2)
            if (c.TryGotoNext(MoveType.Before, i => i.MatchStloc(2)))
            {
                c.EmitLdsfld(typeof(AccessoriesHook).GetField(nameof(OffsetX)));
                c.EmitConvI4();
                c.EmitAdd();
            }

            // Target: int num2 = (int)((float)(inventoryTop - 2) + 0f * Main.inventoryScale);
            // Find where Y coordinate is stored (stloc.3) 
            if (c.TryGotoNext(MoveType.Before, i => i.MatchStloc(3)))
            {
                c.EmitLdsfld(typeof(AccessoriesHook).GetField(nameof(OffsetY)));
                c.EmitConvI4();
                c.EmitAdd();
            }
        }

        private static bool OnSetDrawLocation(orig_SetDrawLocation orig, AccessorySlotLoader self, int trueSlot, int skip, ref int a, ref int b)
        {
            // Call the original method first
            bool result = orig(self, trueSlot, skip, ref a, ref b);

            // Add offsets to the x and y locations regardless of the result
            a += (int)OffsetX;
            b += (int)OffsetY;

            return result;
        }

        private void InjectArmor3Slots(ILContext il)
        {
            var c = new ILCursor(il);

            // Accessory slots Y: num20 (stloc 9):
            // InjectOffsetAtStloc(c, 9, nameof(OffsetY)); // Accessory Y

            // Top 3 armor slots: num41, num42 => stloc 88, 89:
            InjectOffsetAtStloc(c, 88, nameof(OffsetX)); // Armor X
            InjectOffsetAtStloc(c, 89, nameof(OffsetY)); // Armor Y

            // Top 3 vanity slots: num48, num49 => stloc 98, 99:
            InjectOffsetAtStloc(c, 98, nameof(OffsetX)); // Vanity X
            InjectOffsetAtStloc(c, 99, nameof(OffsetY)); // Vanity Y

            // Top 3 dyes: num52, num53 => stloc 103, 104:
            InjectOffsetAtStloc(c, 103, nameof(OffsetX)); // Dye X
            InjectOffsetAtStloc(c, 104, nameof(OffsetY)); // Dye Y

            // Mini achievement X: Find first ldc.r4 47f
            c.GotoNext(MoveType.Before, i => i.MatchLdcR4(47f));
            c.EmitLdsfld(typeof(AccessoriesHook).GetField(nameof(OffsetX)));
            c.EmitAdd();
            // Log.Info("Successfully injected OffsetX for mini achievement");

            // Mini achievement Y: Find first ldc.r4 20f
            // Mini achievement Y: Find the Y calculation and add Y offset
            c.Index = 0;
            if (c.TryGotoNext(MoveType.After,
                i => i.MatchLdloc(85),           // ldloc.s 85 (defenseIconPosition)
                i => i.MatchLdfld<Vector2>("Y"), // ldfld float32 Vector2::Y
                i => i.MatchLdcR4(56f),          // ldc.r4 56
                i => i.MatchLdsfld<Main>("inventoryScale"), // ldsfld float32 Main::inventoryScale
                i => i.MatchMul(),               // mul
                i => i.MatchLdcR4(0.5f),         // ldc.r4 0.5
                i => i.MatchMul(),               // mul
                i => i.MatchSub()))              // sub
            {
                // At this point, the Y calculation is complete on the stack
                // Add our Y offset
                c.EmitLdsfld(typeof(AccessoriesHook).GetField(nameof(OffsetY)));
                c.EmitAdd();
                // Log.Info("Successfully injected OffsetY for mini achievement");
            }
            else
            {
                // Log.Error("Could not find mini achievement Y calculation");
            }

            c.Index = 0;
            if (c.TryGotoNext(MoveType.After,
                i => i.MatchLdloc(85),
                i => i.MatchLdfld<Vector2>("X")))
            {
                c.EmitLdsfld(typeof(AccessoriesHook).GetField(nameof(OffsetX)));
                c.EmitAdd();
                // Log.Info("Successfully injected OffsetX for defense icon");
            }

            // Defense icon Y coordinate
            c.Index = 0;
            if (c.TryGotoNext(MoveType.After,
                i => i.MatchLdloc(85),
                i => i.MatchLdfld<Vector2>("Y")))
            {
                c.EmitLdsfld(typeof(AccessoriesHook).GetField(nameof(OffsetY)));
                c.EmitAdd();
                // Log.Info("Successfully injected OffsetY for defense icon");
            }
        }

        private static void InjectOffsetAtStloc(ILCursor c, int localIndex, string offsetFieldName)
        {
            if (c.TryGotoNext(MoveType.Before, i => i.MatchStloc(localIndex)))
            {
                c.EmitLdsfld(typeof(AccessoriesHook).GetField(offsetFieldName));
                c.EmitConvI4(); // Convert float to int32
                c.EmitAdd();
                // Log.Info($"Successfully injected {offsetFieldName} at stloc.{localIndex}");
            }
            else
            {
                // Log.Error($"Could not find stloc.{localIndex} for {offsetFieldName}");
            }
        }
    }
}
