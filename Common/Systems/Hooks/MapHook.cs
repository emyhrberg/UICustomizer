using System;
using MonoMod.Cil;

namespace UICustomizer.Common.Systems.Hooks
{
    public class MapHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load() => IL_Main.DrawMap += InjectMapOffset;
        public override void Unload() => IL_Main.DrawMap -= InjectMapOffset;

        private void InjectMapOffset(ILContext il)
        {
            try
            {
                Log.Info("Patching MapHook");

                ILCursor c = new(il);

                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchNewobj<Vector2>()))
                {
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