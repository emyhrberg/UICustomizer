using System;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using Mono.Cecil.Cil;
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
            public static readonly Color Fill = new(142, 142, 142);         // #8E8E8E
            public static readonly Color Outline = Color.Black;          // #000000
            public static readonly Color Hover = new(255, 215, 0); // #FFD700 (Gold)
        }

        public static Color FillColor;
        public static Color OutlineColor;
        public static Color HoverColor;
        public static bool IsDrawing = true;

        // Extra customization position and scale
        public static float Scale;
        public static float OffsetX;
        public static float OffsetY;
        public override void Load()
        {
            var cfg = Conf.C;

            // 1) start with the hard defaults
            FillColor = DefaultMenuColours.Fill;
            OutlineColor = DefaultMenuColours.Outline;
            HoverColor = DefaultMenuColours.Hover;

            Scale = cfg?.MainMenuTextColor.Scale ?? 1f;
            OffsetX = cfg?.MainMenuTextColor.OffsetX ?? 0f;
            OffsetY = cfg?.MainMenuTextColor.OffsetY ?? 0f;

            IsDrawing = cfg?.MainMenuDraw.DrawText ?? true;

            Log.Info("found fill: " + cfg?.MainMenuTextColor.FillColor);

            // 2) overwrite only if the user actually stored something
            if (ColorHelper.TryParseHex(cfg?.MainMenuTextColor.FillColor, out var fill))
                FillColor = fill;

            if (ColorHelper.TryParseHex(cfg?.MainMenuTextColor.OutlineColor, out var outline))
                OutlineColor = outline;

            if (ColorHelper.TryParseHex(cfg?.MainMenuTextColor.HoverColor, out var hover))
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
                // Append num51 with OffsetX for hoverposX
                // Append num52 with OffsetY for hoverposY

                // Modify vector3 for hover pos
                //while (c.TryGotoNext(MoveType.After, i => i.MatchStloc(188)))
                //{
                //    c.Emit(OpCodes.Ldloca, 188);
                //    c.EmitDelegate<Action<Vector2>>(vector =>
                //    {
                //        vector.X += OffsetX;
                //        vector.Y += OffsetY;
                //    });
                //    c.Emit(OpCodes.Stloc, 188); // Store result back to local 188
                //}

                // Match exact vector2 vector3 hover pos
                while (c.TryGotoNext(MoveType.After,
                    i => true,
                    i => true,
                    i => i.MatchLdloc(26),
                    i => i.MatchLdloc(173),
                    i => true,
                    i => true,
                    i => i.MatchLdloc(22),
                    i => i.MatchLdloc(173),
                    i => true,
                    i => true,
                    i => true,
                    i => i.MatchStloc(188)))
                {
                    Log.Info("Found hover pos vector2 modification");
                    c.EmitLdloca(188);
                    c.EmitDelegate((ref Vector2 v) => { v = new Vector2(v.X + OffsetX, v.Y + OffsetY); });
                }
                c.Index = 0;

                //var multiplyOp = typeof(Vector2).GetMethod("op_Multiply", [typeof(Vector2), typeof(float)]);
                //while (c.TryGotoNext(i => i.MatchCall(multiplyOp)))
                //{
                //    c.Index++;
                //    c.EmitDelegate<Func<Vector2, Vector2>>(v => new Vector2(v.X + OffsetX, v.Y + OffsetY));
                //    Log.Info("Hover position patched");
                //}
                //c.Index = 0;

                //// Match exact vector2 vector4 hover pos
                //while (c.TryGotoNext(MoveType.After,
                //  i => true,
                //  i => true,
                //  i => true,
                //  i => i.MatchLdloc(26),
                //  i => i.MatchLdloc(173),
                //  i => true,
                //  i => true,
                //  i => i.MatchLdloc(22),
                //  i => i.MatchLdloc(173),
                //  i => true,
                //  i => true
                //))
                //{
                //    Log.Info("Found hover pos vector2 modification");
                //    c.Emit(OpCodes.Ldloc, 188);
                //    c.EmitDelegate<Func<Vector2, Vector2>>(vector =>
                //        new Vector2(vector.X + OffsetX, vector.Y + OffsetY));
                //    c.Emit(OpCodes.Stloc, 188);
                //}
                //c.Index = 0;

                while (c.TryGotoNext(MoveType.After, i => i.MatchCall(out MethodReference meth) && meth.Name == "DrawString"))
                {
                    // Conditionally emit false if IsDrawing is false, effectivelly skipping the call.
                    ILLabel label = il.DefineLabel();
                    c.MarkLabel(label);
                    int oldIndex = c.Index;
                    c.GotoPrev(MoveType.Before, i => i.MatchLdsfld<Main>("spriteBatch"));
                    c.EmitLdsfld(typeof(MainMenuTextColorHook).GetField(nameof(IsDrawing)));
                    c.EmitBrfalse(label);
                    c.Index = oldIndex + 2;
                }
                c.Index = 0;

                // Match all DrawString calls
                while (c.TryGotoNext(MoveType.Before, i => i.MatchCall(out MethodReference meth) && meth.Name == "DrawString"))
                {
                    int drawStringIndex = c.Index;

                    if (c.TryGotoPrev(MoveType.After, i => i.MatchNewobj<Vector2>()))
                        c.EmitDelegate<Func<Vector2, Vector2>>(pos => pos + new Vector2(OffsetX, OffsetY));
                    c.Index = drawStringIndex;

                    if (c.TryGotoPrev(MoveType.Before, i => i.MatchCall(out var meth) && meth.Name == "DrawString"))
                    {
                        if (c.TryGotoPrev(MoveType.After, i => i.MatchLdcI4(0))) // SpriteEffects.None
                        {
                            if (c.TryGotoPrev(MoveType.After, i => i.MatchLdcR4(out _) || i.MatchLdloc(out _))) // Scale
                                c.EmitDelegate<Func<float, float>>(scale => scale * Scale);
                        }
                    }
                    c.Index = drawStringIndex;
                    c.TryGotoNext(MoveType.After, i => i.MatchCall(out var meth) && meth.Name == "DrawString");
                }
                c.Index = 0;

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
