using System;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using UICustomizer.Common.Configs;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    public class MainMenuTextColorHook : ModSystem
    {
        public static class DefaultMenuColours
        {
            public static readonly Color Fill = new(142,142,142);         // #8E8E8E
            public static readonly Color Outline = Color.Black;          // #000000
            public static readonly Color Hover = new(255, 215, 0); // #FFD700 (Gold)
        }

        public static Color FillColor;
        public static Color OutlineColor;
        public static Color HoverColor;
        public override void Load()
        {
            var cfg = Conf.C;

            // 1) start with the hard defaults
            FillColor = DefaultMenuColours.Fill;
            OutlineColor = DefaultMenuColours.Outline;
            HoverColor = DefaultMenuColours.Hover;

            // 2) overwrite only if the user actually stored something
            if (ColorHelper.TryParseHex(cfg?.FillColor, out var fill))
                FillColor = fill;

            if (ColorHelper.TryParseHex(cfg?.OutlineColor, out var outline))
                OutlineColor = outline;

            if (ColorHelper.TryParseHex(cfg?.HoverColor, out var hover))
                HoverColor = hover;

            Main.QueueMainThreadAction(() => IL_Main.DrawMenu += ModifyColors);
        }

        public override void Unload() => Main.QueueMainThreadAction(() => IL_Main.DrawMenu -= ModifyColors);

        private static bool ModifyColor(ref Color color, int r, int g, int b, int a, float interpolator)
        {
            //Log.Info($"[MainMenuTextColorHook] Vanilla menu colour R:{r} G:{g} B:{b} A:{a}");

            // If nothing is set, return here
            if (FillColor == default)
            {
                //Log.Warn("MainMenuFillTextColorHook.OutlineColor is default, skipping color modification.");
                return false;
            }

            // If config is not default, use it
            //FillColor = ColorHelper.HexToColor(Conf.C.FillColor);
            //HoverColor = ColorHelper.HexToColor(Conf.C.HoverColor);

            color = Color.Lerp(FillColor, HoverColor, interpolator);
            return true;
        }

        private void ModifyColors(ILContext il)
        {
            IL.Edit(il, c =>
            {
                // My edit
                c.GotoNext(MoveType.Before, i => i.MatchStloc(177));
                c.EmitPop();
                c.EmitLdsfld(typeof(MainMenuTextColorHook).GetField(nameof(OutlineColor)));

                c.Index = 0; // Reset here to make room for Zoe's edit <3

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
            });
        }
    }
}
