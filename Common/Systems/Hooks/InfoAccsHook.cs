using System;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using UICustomizer.Helpers;

namespace UICustomizer.Common.Systems.Hooks
{
    public class InfoAccsHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            IL_Main.DrawInfoAccs += InjectInfoAccsOffset;
            IL_Main.GetInfoAccIconPosition += InjectIconOffset;
        }

        public override void Unload()
        {
            IL_Main.DrawInfoAccs -= InjectInfoAccsOffset;
            IL_Main.GetInfoAccIconPosition -= InjectIconOffset;
        }

        private void InjectIconOffset(ILContext il)
        {
            try
            {
                ILCursor c = new(il);

                // Find ldc.i4 280 for X (Main.screenWidth - 280)
                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdcI4(280)))
                {
                    c.EmitDelegate((int value) => value - (int)OffsetX);
                }

                // Find ldc.i4 261 for Y offset
                c.Index = 0;
                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdcI4(261)))
                {
                    c.EmitDelegate((int value) => value + (int)OffsetY);
                }
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }

        private void InjectInfoAccsOffset(ILContext il)
        {
            // Log.Info("IL info accs text and icon patching...");

            try
            {
                ILCursor c = new(il);
                int count = 0;

                // Find all Vector2 constructors and apply offsets only to specific ones
                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchNewobj<Vector2>()))
                {
                    count++;
                    // Log.Info($"Found Vector2 constructor #{count} in IL_Main.DrawInfoAccs");

                    // Skip the first two Vector2 constructors (these are for icons and already handled by InjectIconOffset)
                    if (count <= 2)
                    {
                        continue;
                    }

                    c.EmitDelegate<Func<Vector2, Vector2>>(vec =>
                    {
                        return new Vector2(vec.X + OffsetX, vec.Y + OffsetY);
                    });
                }
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }
    }
}