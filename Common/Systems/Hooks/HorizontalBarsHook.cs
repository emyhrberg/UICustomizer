using System;
using MonoMod.Cil;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ModLoader;
using UICustomizer.Helpers;

namespace UICustomizer.Common.Systems.Hooks
{
    public class HorizontalBarsHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            IL_HorizontalBarsPlayerResourcesDisplaySet.Draw += InjectOffset;
        }

        public override void Unload()
        {
            IL_HorizontalBarsPlayerResourcesDisplaySet.Draw -= InjectOffset;
        }

        private void InjectOffset(ILContext il)
        {
            try
            {
                ILCursor c = new(il);

                // Hook life bar panels: ldloc.s 6 (vector) before stfld TopLeftAnchor
                if (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdloca(8),
                    i => i.MatchLdloc(6)))
                {
                    c.EmitDelegate<Func<Vector2, Vector2>>(vec =>
                        new Vector2(vec.X + OffsetX, vec.Y + OffsetY));
                    // // Log.Info("Successfully hooked life bar panels");
                }
                else
                {
                    // // Log.Warn("Failed to find life bar panels");
                }

                // Hook life bar filling: after vector addition but before stfld
                if (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdloca(8),
                    i => i.MatchLdloc(6),
                    i => i.MatchLdcR4(6f),
                    i => i.MatchLdcR4(6f),
                    i => i.MatchNewobj<Vector2>(),
                    i => i.MatchCall(typeof(Vector2), "op_Addition")))
                {
                    c.EmitDelegate<Func<Vector2, Vector2>>(vec =>
                        new Vector2(vec.X + OffsetX, vec.Y + OffsetY));
                    // Log.Info("Successfully hooked life bar filling");
                }
                else
                {
                    // Log.Warn("Failed to find life bar filling");
                }

                // Hook mana bar panels
                if (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdloca(8),
                    i => i.MatchLdloc(9)))
                {
                    c.EmitDelegate<Func<Vector2, Vector2>>(vec =>
                        new Vector2(vec.X + OffsetX, vec.Y + OffsetY));
                    // Log.Info("Successfully hooked mana bar panels");
                }
                else
                {
                    // Log.Warn("Failed to find mana bar panels");
                }

                // Hook mana bar filling
                if (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdloca(8),
                    i => i.MatchLdloc(9),
                    i => i.MatchLdcR4(6f),
                    i => i.MatchLdcR4(6f),
                    i => i.MatchNewobj<Vector2>(),
                    i => i.MatchCall(typeof(Vector2), "op_Addition")))
                {
                    c.EmitDelegate<Func<Vector2, Vector2>>(vec =>
                        new Vector2(vec.X + OffsetX, vec.Y + OffsetY));
                    // Log.Info("Successfully hooked mana bar filling");
                }
                else
                {
                    // Log.Warn("Failed to find mana bar filling");
                }
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }
    }
}