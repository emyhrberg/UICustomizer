using System;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks
{
    public class ChatHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            IL_Main.DrawPlayerChat += InjectChatOffset;
            IL_RemadeChatMonitor.DrawChat += InjectChatOffset;
        }

        public override void Unload()
        {
            IL_Main.DrawPlayerChat -= InjectChatOffset;
            IL_RemadeChatMonitor.DrawChat -= InjectChatOffset;
        }

        private void InjectChatOffset(ILContext il)
        {
            try
            {
                ILCursor c = new(il);

                // Find 3 calls to SpriteBatch.Draw, which is where the chat text is drawn.
                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchConvR4(),
                    i => i.MatchNewobj<Vector2>()))
                {
                    c.EmitDelegate((Vector2 pos) => pos + new Vector2(OffsetX, OffsetY));
                }
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }
    }
}