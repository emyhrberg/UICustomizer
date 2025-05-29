using System;
using MonoMod.Cil;

namespace UICustomizer.Common.Systems.Hooks
{
    public class InfoAccsHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            Main.QueueMainThreadAction(() => IL_Main.DrawInfoAccs += InjectInfoAccsOffset);
        }

        public override void Unload()
        {
            Main.QueueMainThreadAction(() => IL_Main.DrawInfoAccs -= InjectInfoAccsOffset);
        }

        private void InjectInfoAccsOffset(ILContext il)
        {
            Log.Info("IL info accs patching...");

            try
            {
                ILCursor c = new(il);
                int count = 0;
                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchNewobj<Vector2>()))
                {
                    Log.Info("ILcount: " + count);
                    count++;
                    c.EmitDelegate((Vector2 pos) =>
                    {
                        return new Vector2(
                            pos.X + OffsetX,
                            pos.Y + OffsetY
                        );
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