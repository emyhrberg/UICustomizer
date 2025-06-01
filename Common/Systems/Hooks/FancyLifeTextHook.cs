using System;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks
{
    public class FancyLifeTextHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            IL_FancyClassicPlayerResourcesDisplaySet.DrawLifeBarText += InjectLifeBarTextOffset;
        }

        public override void Unload()
        {
            IL_FancyClassicPlayerResourcesDisplaySet.DrawLifeBarText -= InjectLifeBarTextOffset;
        }

        private void InjectLifeBarTextOffset(ILContext il)
        {
            try
            {
                ILCursor c = new(il);

                if (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdcR4(130f),
                    i => i.MatchLdcR4(-24f),
                    i => i.MatchNewobj<Vector2>()))
                {
                    c.EmitDelegate<Func<Vector2, Vector2>>(offset => new Vector2(offset.X + OffsetX, offset.Y + OffsetY));
                    // Log.Info("Hooked fancy life bar text offset");
                }
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }
    }
}