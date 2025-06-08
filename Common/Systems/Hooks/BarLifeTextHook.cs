using System;
using MonoMod.Cil;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks
{
    public class BarLifeTextHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            IL_HorizontalBarsPlayerResourcesDisplaySet.DrawLifeBarText += InjectLifeTextOffset;
        }

        public override void Unload()
        {
            IL_HorizontalBarsPlayerResourcesDisplaySet.DrawLifeBarText -= InjectLifeTextOffset;
        }

        private void InjectLifeTextOffset(ILContext il)
        {
            try
            {
                ILCursor c = new(il);

                // Target: Vector2 vector = topLeftAnchor + new Vector2(130f, -20f);
                if (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdarg(1), // topLeftAnchor
                    i => i.MatchLdcR4(130f),
                    i => i.MatchLdcR4(-20f),
                    i => i.MatchNewobj<Vector2>(),
                    i => i.MatchCall(typeof(Vector2), "op_Addition")))
                {
                    c.EmitDelegate<Func<Vector2, Vector2>>(vec =>
                        new Vector2(vec.X + OffsetX, vec.Y + OffsetY));
                    // Log.Info("Successfully hooked life bar text offset");
                }
                else
                {
                    // Log.Warn("Failed to find life bar text offset");
                }
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }
    }
}