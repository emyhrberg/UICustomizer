using MonoMod.Cil;

namespace UICustomizer.Common.Systems.Hooks
{
    public class CraftWindowHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            IL_Main.DrawInventory += WindowNog;
        }
        public override void Unload()
        {
            IL_Main.DrawInventory -= WindowNog;
        }

        private void WindowNog(ILContext il)
        {
            var c = new ILCursor(il);

            IL.Edit(il, c =>
            {
                InjectOffsetAtStloc(c, 143, nameof(OffsetY)); // Crafting Window Y
                InjectOffsetAtStloc(c, 144, nameof(OffsetX)); // Crafting Window X
            });
        }

        private static void InjectOffsetAtStloc(ILCursor c, int localIndex, string offsetFieldName)
        {
            if (c.TryGotoNext(MoveType.Before, i => i.MatchStloc(localIndex)))
            {
                c.EmitLdsfld(typeof(CraftWindowHook).GetField(offsetFieldName));
                c.EmitConvI4(); // Convert float to int32
                c.EmitAdd();
                Log.Info($"Successfully injected {offsetFieldName} at stloc.{localIndex}");
            }
            else
            {
                Log.Error($"Could not find stloc.{localIndex} for {offsetFieldName}");
            }
        }
    }
}
