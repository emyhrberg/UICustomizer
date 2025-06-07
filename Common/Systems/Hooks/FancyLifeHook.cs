using System;
using MonoMod.Cil;
using Terraria.GameContent.UI.ResourceSets;

namespace UICustomizer.Common.Systems.Hooks
{
    public class FancyLifeHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            // Hook the Vector2 constructor in DrawLifeBar method
            IL_FancyClassicPlayerResourcesDisplaySet.DrawLifeBar += InjectLifeBarOffset;
            // Hook the static DrawLifeBarText method
            IL_FancyClassicPlayerResourcesDisplaySet.DrawLifeBarText += InjectLifeBarTextOffset;
        }

        public override void Unload()
        {
            IL_FancyClassicPlayerResourcesDisplaySet.DrawLifeBar -= InjectLifeBarOffset;
            IL_FancyClassicPlayerResourcesDisplaySet.DrawLifeBarText -= InjectLifeBarTextOffset;
        }

        private void InjectLifeBarOffset(ILContext il)
        {
            try
            {
                ILCursor c = new(il);

                // Hook only the main Vector2 position: new Vector2(Main.screenWidth - 300 + 4, 15f)
                if (c.TryGotoNext(MoveType.After,
                    i => i.MatchLdsfld("Terraria.Main", "screenWidth"),
                    i => i.MatchLdcI4(300),
                    i => i.MatchSub(),
                    i => i.MatchLdcI4(4),
                    i => i.MatchAdd(),
                    i => i.MatchConvR4(),
                    i => i.MatchLdcR4(15f),
                    i => i.MatchNewobj<Vector2>()))
                {
                    c.EmitDelegate<Func<Vector2, Vector2>>(pos => new Vector2(pos.X + OffsetX, pos.Y + OffsetY));
                    // Log.Info("Hooked fancy life bar main position");
                }

                // Hook TopLeftAnchor field assignments - this is the key part!
                c.Index = 0;
                int anchorCount = 0;
                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchStfld<ResourceDrawSettings>("TopLeftAnchor")))
                {
                    anchorCount++;
                    // Go back to hook the Vector2 being assigned to TopLeftAnchor
                    c.Index -= 1;
                    c.EmitDelegate<Func<Vector2, Vector2>>(anchor => new Vector2(anchor.X + OffsetX, anchor.Y + OffsetY));
                    c.Index += 1; // Move forward again
                    // Log.Info($"Hooked TopLeftAnchor assignment #{anchorCount}");
                }
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }

        private void InjectLifeBarTextOffset(ILContext il)
        {
            try
            {
                ILCursor c = new(il);

                // Hook the Vector2 constructor in DrawLifeBarText: topLeftAnchor + new Vector2(130f, -24f)
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