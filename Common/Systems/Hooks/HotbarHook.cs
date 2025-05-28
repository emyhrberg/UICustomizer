using System;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace UICustomizer.Common.Systems.Hooks
{
    public class HotbarHook : ModSystem
    {
        public static float OffsetX = 0f;
        public static float OffsetY = 0f;

        public override void Load()
        {
            IL_Main.GUIHotbarDrawInner += InjectOffset;
            IL_Main.GUIHotbarDrawInner += ModifyHotbarItemPosition;
        }

        public override void Unload()
        {
            IL_Main.GUIHotbarDrawInner -= InjectOffset;
            IL_Main.GUIHotbarDrawInner -= ModifyHotbarItemPosition;
        }

        private void ModifyHotbarItemPosition(ILContext il)
        {
            try
            {
                ILCursor c = new(il);

                // This is a safer approach to getting the value of a local rather than hardcoding the index, this should be much safer when other edits/game updates are involved. (blame lioncake for this)
                int positionIndex = -1;

                c.GotoNext(MoveType.After,
                  i => i.MatchLdloca(out positionIndex), // we match our positionIndex with the IL I guess
                  i => i.MatchLdcR4(236f)); // yes this matches with the IL

                // Match to after `((Vector2)(ref val))..ctor(236f - (FontAssets.MouseText.Value.MeasureString(text) / 2f).X, 0f);` or whatever slop the decompiler spat out.
                c.GotoNext(MoveType.After,
                  i => i.MatchLdcR4(0f), // why 0f here
                  i => i.MatchCall<Vector2>(".ctor")); // Vector2 constructor here, why

                // Use ldloca to load a ref of the local, making it easier to modify with our delegate.
                c.EmitLdloca(positionIndex); // Here we load local variable with position.

                // delegate is a reference to the function and we modify it
                c.EmitDelegate((ref Vector2 position) =>
                {
                    position += new Vector2(OffsetX, OffsetY);
                });
            }
            catch (Exception innerException)
            {
                throw new ILPatchFailureException(Mod, il, innerException);
            }
        }

        private static void InjectOffset(ILContext il)
        {
            var vec2Ctor = typeof(Vector2).GetConstructor([typeof(float), typeof(float)]);
            var vec2Add = typeof(Vector2).GetMethod("op_Addition",
                            BindingFlags.Public | BindingFlags.Static,
                            null, [typeof(Vector2), typeof(Vector2)], null);

            var fldX = typeof(HotbarHook).GetField(nameof(OffsetX));
            var fldY = typeof(HotbarHook).GetField(nameof(OffsetY));

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