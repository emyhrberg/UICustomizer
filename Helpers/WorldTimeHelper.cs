using System;
using Terraria;

namespace UICustomizer.Helpers
{
    public static class WorldTimeHelper
    {
        public static double ConvertToTotalTime()
        {
            if (Main.dayTime)
            {
                return Main.time;
            }
            else
            {
                return Main.dayLength + Main.time;
            }
        }

        public static void SetTime(float ratio)
        {
            // 0 … 1  → always stay inside one 24-hour cycle
            ratio = (ratio % 1f + 1f) % 1f;

            const float dayTicks = 54000f;   // 4 h 30 m … 7 h 30 m  (sunrise → sunset)
            const float totalTicks = 86400f;   // 24 hours

            float ticks = ratio * totalTicks;

            if (ticks < dayTicks)              // day phase
            {
                Main.dayTime = true;
                Main.time = ticks;          //   0 … 53999
            }
            else                               // night phase
            {
                Main.dayTime = false;
                Main.time = ticks - dayTicks;   //   0 … 32399
            }
        }

        public static float GetRatioFromTime()
        {
            const float dayTicks = 54000f;
            const float totalTicks = 86400f;

            double ticks = Main.time + (Main.dayTime ? 0 : dayTicks);   // add night offset
            return (float)(ticks / totalTicks);                         // 0 … 1
        }

        /// <remarks>Terraria’s clock starts at 4 : 30 AM.</remarks>
        public static string GetFormattedTime()
        {
            const double dayTicks = 54000.0;      // sunrise → sunset
            double ticks = Main.time + (Main.dayTime ? 0 : dayTicks);

            //   ticks → hours since 4 : 30
            double hours24 = 4.5 + ticks / 3600.0;   // 3600 ticks = 1 h
            hours24 %= 24.0;                         // wrap past midnight

            int h = (int)hours24;
            int m = (int)Math.Round((hours24 - h) * 60.0);

            if (m == 60) { m = 0; h = (h + 1) % 24; }

            string ampm = h >= 12 ? "PM" : "AM";
            int h12 = h % 12; if (h12 == 0) h12 = 12;

            return $"{h12}:{m:D2} {ampm}";
        }

    }
}
