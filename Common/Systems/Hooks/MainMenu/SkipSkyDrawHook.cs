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
            //IL_Main.DrawMenu += SkipSun;
        }
        public override void Unload()
        {
            //IL_Main.DrawMenu -= SkipSun;
        }
        private void SkipSun(ILContext il)
        {
            IL.Edit(il, c =>
            {
                //spriteBatch.Draw(asset.Value, destinationRectangle, ColorOfTheSkies);
            });
        }
    }
}
