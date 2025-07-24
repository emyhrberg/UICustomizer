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
        private Hook _renderBGHook;

        public override void Load()
        {
            //var m1 = typeof(Main).GetMethod("RenderBackground",
            //                               BindingFlags.NonPublic | BindingFlags.Instance);

            //if (m1 == null) Log.Info("null RBG");

            //if (m1 != null)
            //    _renderBGHook = new Hook(m1,
            //        new Action<Action<Main>, Main>((orig, self) =>
            //        {
            //            if (IsDrawing)
            //                orig(self);
            //        }));

            //var m = typeof(Main).GetMethod("DrawBackground",
            //                               BindingFlags.NonPublic | BindingFlags.Instance);
            //if (m != null)
            //    _bgHook = new Hook(m,
            //        new Action<Action<Main>, Main>((orig, self) =>
            //        {
            //            if (IsDrawing)
            //                orig(self);
            //        }));

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
            _renderBGHook?.Dispose();
        }
    }
}
