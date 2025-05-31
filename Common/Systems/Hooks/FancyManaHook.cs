using System;
using MonoMod.Cil;
using Terraria.GameContent.UI.ResourceSets;

namespace UICustomizer.Common.Systems.Hooks
{
    public class FancyManaHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            IL_FancyClassicPlayerResourcesDisplaySet.DrawManaBar += InjectOffset;
            IL_FancyClassicPlayerResourcesDisplaySet.DrawManaText += InjectOffset;
        }

        public override void Unload()
        {
            IL_FancyClassicPlayerResourcesDisplaySet.DrawManaBar -= InjectOffset;
            IL_FancyClassicPlayerResourcesDisplaySet.DrawManaText -= InjectOffset;
        }

        private void InjectOffset(ILContext il)
        {
            try
            {
                ILCursor c = new(il);



                // while (c.TryGotoNext(MoveType.After,
                //     i => i.MatchNewobj<Vector2>()))
                // {
                //     c.EmitDelegate((Vector2 pos) =>
                //     {
                //         return new Vector2(
                //             pos.X + OffsetX,
                //             pos.Y + OffsetY
                //         );
                //     });
                // }
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }
    }
}