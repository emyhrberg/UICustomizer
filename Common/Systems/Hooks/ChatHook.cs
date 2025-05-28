using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.GameContent.UI.Chat;

namespace UICustomizer.Common.Systems.Hooks
{
    public class ChatPosHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            IL_Main.DrawPlayerChat += InjectOffset;
            IL_RemadeChatMonitor.DrawChat += InjectOffset;
        }

        public override void Unload()
        {
            IL_Main.DrawPlayerChat -= InjectOffset;
            IL_RemadeChatMonitor.DrawChat -= InjectOffset;
        }

        public override void PostUpdateEverything()
        {
            // Test hot reload changes here (to chat position)
            OffsetX = 50;
            OffsetY = 0;
            base.PostUpdateEverything();
        }

        /// <summary>Adds new Vector2(OffsetX, OffsetY) to every Vector2 literal.</summary>
        private static void InjectOffset(ILContext il)
        {
            var vec2Ctor = typeof(Vector2).GetConstructor([typeof(float), typeof(float)]);
            var vec2Add = typeof(Vector2).GetMethod("op_Addition",
                             BindingFlags.Public | BindingFlags.Static,
                             null, [typeof(Vector2), typeof(Vector2)], null);

            var fldX = typeof(ChatPosHook).GetField(nameof(OffsetX));
            var fldY = typeof(ChatPosHook).GetField(nameof(OffsetY));

            ILCursor c = new(il);

            while (c.TryGotoNext(i => i.MatchNewobj(vec2Ctor)))
            {
                c.Index++;                              // insert right after newobj

                // push new Vector2(OffsetX, OffsetY)
                c.Emit(OpCodes.Ldsfld, fldX);           // float OffsetX
                c.Emit(OpCodes.Ldsfld, fldY);           // float OffsetY
                c.Emit(OpCodes.Newobj, vec2Ctor);       // Vector2(offsetX, offsetY)

                // add the two vectors
                c.Emit(OpCodes.Call, vec2Add);          // original + offset
            }
        }
    }
}