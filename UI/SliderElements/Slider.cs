using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;

namespace UICustomizer.UI.SliderElements
{
    public sealed class Slider : SliderBase
    {
        const int BLIP_COUNT = 167;          // vanilla colour-bar width
        readonly int _pixels;                // requested pixel width

        readonly Func<float> _get;
        readonly Action<float> _set;
        readonly Func<float, Color> _col;

        public Slider(Func<float> getter,
                      Action<float> setter,
                      Func<float, Color> colour,
                      int width = 167)
        {
            _pixels = Math.Max(1, width);
            _get = getter ?? (() => 0f);
            _set = setter ?? (_ => { });
            _col = colour ?? (t => Color.Lerp(Color.Black, Color.White, t));

            Width.Set(_pixels, 0);
            Height.Set(20, 0);
            Left.Set(-_pixels-10, 1f);          // right-align inside parent
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            if (!Main.mouseLeft) CurrentLockedSlider = null;
            CurrentAimedSlider = null;

            var d = GetDimensions();
            float sc = _pixels / (float)BLIP_COUNT;          // scale factor → requested width
            var pos = new Vector2(d.X + d.Width +20, d.Y + 20);

            bool inBar;
            float newVal = DrawBar(sb, pos, sc, _get(), UsageLevel, out inBar);

            if (inBar || CurrentLockedSlider == this)
            {
                CurrentAimedSlider = this;
                if (PlayerInput.Triggers.Current.MouseLeft &&
                    CurrentLockedSlider == this)
                    _set(newVal);
            }

            if (CurrentAimedSlider != null && CurrentLockedSlider == null)
                CurrentLockedSlider = CurrentAimedSlider;
        }

        float DrawBar(SpriteBatch sb, Vector2 pos, float sc, float norm,
                      SliderUsageLevel lvl, out bool inBar)
        {
            var barTex = TextureAssets.ColorBar.Value;
            var blipTex = TextureAssets.ColorBlip.Value;
            var hiTex = TextureAssets.ColorHighlight.Value;
            var knobTex = TextureAssets.ColorSlider.Value;

            int barH = (int)(barTex.Height * sc);
            pos.X -= _pixels;                               // right -> left

            var rBar = new Rectangle((int)pos.X, (int)(pos.Y - barH * .5f), _pixels, barH);
            sb.Draw(barTex, rBar, Color.White);

            float innerX = rBar.X + 5f * sc;
            float innerY = rBar.Y + 4f * sc;

            // coloured strip
            for (int i = 0; i < BLIP_COUNT; i++)
            {
                float t = i / (float)BLIP_COUNT;
                sb.Draw(blipTex,
                        new Vector2(innerX + i * sc, innerY),
                        null, _col(t), 0, default, sc, 0, 0);
            }

            var click = new Rectangle((int)innerX - 9,
                                      (int)innerY - 9,
                                      _pixels - 10 + 18,         // inner width + padding
                                      barH - 8 + 18);

            bool hover = click.Contains(Main.mouseX, Main.mouseY) &&
                         lvl != SliderUsageLevel.OtherElementIsLocked;

            if (hover || lvl == SliderUsageLevel.SelectedAndLocked)
                sb.Draw(hiTex, rBar, Main.OurFavoriteColor);

            // knob
            float knobX = innerX + (_pixels - 10) * norm;   // -10 ⇒ inner padding
            sb.Draw(knobTex,
                    new Vector2(knobX, innerY + 4f * sc),
                    null, Color.White, 0,
                    knobTex.Size() * .5f,
                    sc, 0, 0);

            float ratio = MathHelper.Clamp((Main.mouseX - click.X) / (float)click.Width, 0, 1);
            inBar = hover && !IgnoresMouseInteraction;
            return ratio;
        }
    }
}
