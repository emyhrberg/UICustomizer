using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    public class SkipSkyDrawHook : ModSystem
    {
        public static bool IsDrawing = true;

        public override void Load()
        {
            IL_Main.DoDraw += SkipDraw;
        }
        public override void Unload()
        {
            IL_Main.DoDraw -= SkipDraw;
        }
        private void SkipDraw(ILContext il)
        {
            IL.Edit(il, c =>
            {
                // find the first SpriteBatch.Draw that paints the sky background
                c.GotoNext(MoveType.Before,
                  i => i.MatchLdsfld<Main>("spriteBatch"), // IL_1009: ldsfld class [FNA]Microsoft.Xna.Framework.Graphics.SpriteBatch Terraria.Main::spriteBatch
                  i => i.MatchLdloc(25), // IL_100e: ldloc.s 25
                  i => true, // IL_1010: callvirt instance !0 class [ReLogic]ReLogic.Content.Asset`1<class [FNA]Microsoft.Xna.Framework.Graphics.Texture2D>
                  i => i.MatchLdloc(26), // IL_1015: ldloc.s 26
                  i => i.MatchLdsfld<Main>("ColorOfTheSkies"), // IL_1017: ldsfld valuetype [FNA]Microsoft.Xna.Framework.Color Terraria.Main::ColorOfTheSkies
                  i => true // IL_101c: callvirt instance void [FNA]Microsoft.Xna.Framework.Graphics.SpriteBatch::Draw
                );

                // Skip unconditionally
                //ILLabel label = il.DefineLabel();
                //c.EmitBr(label);
                //c.Index += 6;
                //c.MarkLabel(label);
                
                // Skip if IsDrawing is false
                ILLabel afterDraw = il.DefineLabel(); // label after the Draw so we can jump over it
                c.EmitLdsfld(typeof(SkipSkyDrawHook).GetField(nameof(IsDrawing))); // push bool
                c.EmitBrfalse(afterDraw); // skip when false
                c.Index += 6; // keep original six instructions
                c.MarkLabel(afterDraw); // then drop here when drawing is disabled
            });
        }

    }
}
