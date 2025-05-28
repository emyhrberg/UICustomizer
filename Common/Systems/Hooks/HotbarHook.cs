using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace UICustomizer.Common.Systems.Hooks
{
    public class HotbarPosHook : ModSystem
    {
        public static float OffsetX = 0f;
        public static float OffsetY = 0f;

        public override void Load()
        {
            IL_Main.GUIHotbarDrawInner += InjectOffset;
        }

        public override void Unload()
        {
            IL_Main.GUIHotbarDrawInner -= InjectOffset;
        }

        private static void InjectOffset(ILContext il)
        {
            var vec2Ctor = typeof(Vector2).GetConstructor([typeof(float), typeof(float)]);
            var vec2Add = typeof(Vector2).GetMethod("op_Addition",
                            BindingFlags.Public | BindingFlags.Static,
                            null, [typeof(Vector2), typeof(Vector2)], null);

            var fldX = typeof(HotbarPosHook).GetField(nameof(OffsetX));
            var fldY = typeof(HotbarPosHook).GetField(nameof(OffsetY));

            ILCursor c = new(il);

            // For every “new Vector2(...)” in the method:
            while (c.TryGotoNext(i => i.MatchNewobj(vec2Ctor)))
            {
                c.Index++;                         // insert *after* the original Vector2
                c.Emit(OpCodes.Ldsfld, fldX);      // push OffsetX (float)
                c.Emit(OpCodes.Ldsfld, fldY);      // push OffsetY (float)
                c.Emit(OpCodes.Newobj, vec2Ctor);  // new Vector2(OffsetX, OffsetY)
                c.Emit(OpCodes.Call, vec2Add);     // original + offset
            }
        }
    }
}