using System;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks
{
    public class BuffHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            IL_Main.DrawBuffIcon += InjectBuffOffset;
        }

        public override void Unload()
        {
            IL_Main.DrawBuffIcon -= InjectBuffOffset;
        }

        private void InjectBuffOffset(ILContext il)
        {
            // Log.Info("IL buff patching...");

            try
            {
                ILCursor c = new(il);

                // Find ldarg.2 (x parameter) and add offset
                while (c.TryGotoNext(MoveType.After, i => i.MatchLdarg(2)))
                {
                    c.EmitDelegate<Func<int, int>>(x => x + (int)OffsetX);
                }

                c.Index = 0;
                // Find ldarg.3 (y parameter) and add offset  
                while (c.TryGotoNext(MoveType.After, i => i.MatchLdarg(3)))
                {
                    c.EmitDelegate<Func<int, int>>(y => y + (int)OffsetY);
                }
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }
    }
}