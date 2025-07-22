using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    public class MainMenuFillTextColorHook : ModSystem
    {
        public static Color Color;
        public override void Load() => Main.QueueMainThreadAction(() => IL_Main.DrawMenu += ModifyColors);
        public override void Unload() => Main.QueueMainThreadAction(() => IL_Main.DrawMenu -= ModifyColors);

        private static bool ModifyColor(ref Color color, int r, int g, int b, int a, float interpolator)
        {
            //Color = Color.Lerp(Color, MainMenuHoverTextColorHook.Color, interpolator);
            Log.Info("new color: " + color + ", and Color:" + Color);
            color = MainMenuFillTextColorHook.Color;
            return true;
        }

        private void ModifyColors(ILContext il)
        {
            try
            {
                ILCursor c = new(il);

                int colorIndex = -1;

                int rIndex = -1;
                int gIndex = -1;
                int bIndex = -1;
                int aIndex = -1;

                int hoveredIndex = -1;
                int outerIteratorIndex = -1;
                int innerIteratorIndex = -1;

                int interpolatorIndex = -1;

                ILLabel jumpColorCtorTarget = c.DefineLabel();

                for (int i = 0; i < 5; i++)
                {
                    // Grab relevant color indices.
                    c.GotoNext(MoveType.After,
                        i => i.MatchLdloca(out colorIndex),
                        i => i.MatchLdloc(out rIndex),
                        i => i.MatchConvU1(),
                        i => i.MatchLdloc(out gIndex),
                        i => i.MatchConvU1(),
                        i => i.MatchLdloc(out bIndex),
                        i => i.MatchConvU1(),
                        i => i.MatchLdloc(out aIndex),
                        i => i.MatchConvU1(),
                        i => i.MatchCall<Color>(".ctor"));

                    if (i == 4)
                        break;

                    c.EmitLdloca(colorIndex);

                    c.EmitLdloc(rIndex);
                    c.EmitLdloc(gIndex);
                    c.EmitLdloc(bIndex);
                    c.EmitLdloc(aIndex);

                    c.EmitLdcR4(0f);

                    c.EmitDelegate(ModifyColor);

                    c.EmitPop();
                }

                // Mark this label so we can skip this ctor later.
                c.MarkLabel(jumpColorCtorTarget);

                // Grab the inner iterator to check if were drawing the colored text and not the shadow.
                c.GotoNext(MoveType.After,
                    i => i.MatchLdloc(out innerIteratorIndex),
                    i => i.MatchLdcI4(4),
                    i => i.MatchBneUn(out _));

                // Insert our stuff before the game handles hover color.
                c.GotoPrev(MoveType.Before,
                    i => i.MatchLdloc(out hoveredIndex),
                    i => i.MatchLdloc(out outerIteratorIndex),
                    i => i.MatchBneUn(out _),
                    i => i.MatchLdloc(out _),
                    i => i.MatchLdcI4(4),
                    i => i.MatchBneUn(out _),
                    i => i.MatchLdloc(out interpolatorIndex));

                c.MoveAfterLabels();

                c.EmitLdloca(colorIndex);

                c.EmitLdloc(innerIteratorIndex);

                c.EmitLdloc(rIndex);
                c.EmitLdloc(gIndex);
                c.EmitLdloc(bIndex);
                c.EmitLdloc(aIndex);

                c.EmitLdloc(interpolatorIndex);

                c.EmitLdloc(hoveredIndex);
                c.EmitLdloc(outerIteratorIndex);

                c.EmitDelegate((ref Color color, int i, int r, int g, int b, int a, int interpolator, int hovered, int j) =>
                {
                    if (i != 4)
                        return false;

                    return ModifyColor(ref color, r, g, b, a, hovered == j ? interpolator / 255f : 0);
                });

                c.EmitBrtrue(jumpColorCtorTarget);
            }
            catch (Exception e)
            {
                Log.Error("Err! " + e.Message);
                throw new Exception(e.Message);
            }
        }
    }
}
