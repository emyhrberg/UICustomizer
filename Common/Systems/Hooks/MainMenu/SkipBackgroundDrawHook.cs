using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    public class SkipBackgroundDrawHook : ModSystem
    {
        public static bool IsDrawing = true;

        private Hook _bgHook;      // DrawBackground  (underground layers)
        private Hook _drawBGHook;  // DrawBG          (surface‑sky)

        public override void Load()
        {
            // ----- underground / menu backdrop -----
            var m = typeof(Main).GetMethod("DrawBackground",
                                           BindingFlags.NonPublic | BindingFlags.Instance);
            if (m != null)
                _bgHook = new Hook(m,
                    new Action<Action<Main>, Main>((orig, self) =>
                    {
                        if (IsDrawing)
                            orig(self);
                    }));

            // ----- surface‑sky while playing -----
            var m2 = typeof(Main).GetMethod("DrawBG",
                                            BindingFlags.NonPublic | BindingFlags.Instance);
            if (m2 != null)
                _drawBGHook = new Hook(m2,
                    new Action<Action<Main>, Main>((orig, self) =>
                    {
                        if (IsDrawing)
                            orig(self);
                    }));
        }

        public override void Unload()
        {
            _bgHook?.Dispose();
            _drawBGHook?.Dispose();
        }
    }
}
