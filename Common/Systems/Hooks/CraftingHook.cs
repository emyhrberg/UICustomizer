using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using UICustomizer.Helpers;

namespace UICustomizer.Common.Systems.Hooks
{
    public class CraftingHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        // delete this
        private delegate void orig_DrawGuideCraftText(int adjY, Color craftingTipColor, out int inventoryX, out int inventoryY);

        public override void Load()
        {
            IL_Main.DrawInventory += CraftingNog;
        }
        public override void Unload()
        {
            IL_Main.DrawInventory -= CraftingNog;
        }

        // private void OnDrawGuideCraftText(orig_DrawGuideCraftText orig, int adjY, Color craftingTipColor, out int inventoryX, out int inventoryY)
        // {
        //     orig(adjY, craftingTipColor, out inventoryX, out inventoryY);
        //     inventoryX += (int)OffsetX;
        //     inventoryY += (int)OffsetY;
        // }

        private void CraftingNog(ILContext il)
        {
            var c = new ILCursor(il);

            // Target the "Crafting" text DrawString Vector2 construction
            // Look for: new Vector2(76f, 414 + num54)
            if (c.TryGotoNext(MoveType.After,
                i => i.MatchLdcR4(76f),           // ldc.r4 76 (X coordinate)
                i => i.MatchLdcI4(414),           // ldc.i4 414 
                i => i.MatchLdloc(13),            // ldloc.s 13 (num54)
                i => i.MatchAdd(),                // add (414 + num54)
                i => i.MatchConvR4()))            // conv.r4 (convert to float)
            {
                // At this point stack has: [76f] [414+num54 as float]
                // Add Y offset first (top of stack)
                c.EmitLdsfld(typeof(CraftingHook).GetField(nameof(OffsetY)));
                c.EmitAdd();

                // Now we need to modify the X coordinate (second on stack)
                // Store the modified Y temporarily
                var tempLocal = il.Method.Body.Variables.Count;
                il.Method.Body.Variables.Add(new VariableDefinition(il.Module.TypeSystem.Single));

                c.EmitStloc(tempLocal);    // Store modified Y
                c.EmitLdsfld(typeof(CraftingHook).GetField(nameof(OffsetX)));
                c.EmitAdd();               // Add OffsetX to X coordinate
                c.EmitLdloc(tempLocal);    // Reload modified Y

                // Log.Info("Successfully injected offsets for 'Crafting' text");
            }

            IL.Edit(il, c =>
            {
                // 106, 107 reforge item (scroll up and down)
                // InjectOffsetAtStloc(c, 106, nameof(OffsetX)); // Reforge X
                // InjectOffsetAtStloc(c, 107, nameof(OffsetY)); // Reforge Y

                // num68, num 69 => 125, 126 recipe item (scroll up and down)
                InjectOffsetAtStloc(c, 125, nameof(OffsetX)); // Recipe X
                InjectOffsetAtStloc(c, 126, nameof(OffsetY)); // Recipe Y

                // num73, num74 => 132, 133
                InjectOffsetAtStloc(c, 131, nameof(OffsetX)); // Craft X
                InjectOffsetAtStloc(c, 132, nameof(OffsetY)); // Craft Y

                // num77, num78 => 138,139
                InjectOffsetAtStloc(c, 138, nameof(OffsetX)); // Hammer X
                InjectOffsetAtStloc(c, 139, nameof(OffsetY)); // Hammer Y
            });
        }

        private static void InjectOffsetAtStloc(ILCursor c, int localIndex, string offsetFieldName)
        {
            if (c.TryGotoNext(MoveType.Before, i => i.MatchStloc(localIndex)))
            {
                c.EmitLdsfld(typeof(CraftingHook).GetField(offsetFieldName));
                c.EmitConvI4(); // Convert float to int32
                c.EmitAdd();
                // Log.Info($"Successfully injected {offsetFieldName} at stloc.{localIndex}");
            }
            else
            {
                Log.Error($"Could not find stloc.{localIndex} for {offsetFieldName}");
            }
        }
    }
}
