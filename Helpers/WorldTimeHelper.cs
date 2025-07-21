using Terraria;

namespace UICustomizer.Helpers
{
    public static class WorldTimeHelper
    {
        public static void SetTime(float ratio)
        {
            const float dayDuration = 54000f;
            const float totalCycleDuration = 86400f; // Day (54000) + Night (32400)
            float currentTickInCycle = ratio * totalCycleDuration;

            if (currentTickInCycle < dayDuration)
            {
                Main.dayTime = true;
                Main.time = currentTickInCycle;
            }
            else
            {
                Main.dayTime = false;
                Main.time = currentTickInCycle - dayDuration;
            }
        }

        public static float GetRatioFromTime()
        {
            const float dayDuration = 54000f;
            const float totalCycleDuration = 86400f;

            double currentTimeInCycle = Main.time;
            if (!Main.dayTime)
            {
                currentTimeInCycle += dayDuration;
            }

            return (float)(currentTimeInCycle / totalCycleDuration);
        }
        
        /// <summary>
        /// Get the current formatted time like 04:30 AM or 8:59 PM
        /// </summary>
        public static string GetFormattedTime()
        {
            // Calculate the total time in ticks since the start of the day
            double time = Main.time + (Main.dayTime ? 0 : 54000);

            // Terraria's in-game time starts at 4:30 AM, so add 4.5 hours
            double hours = (time / 3600.0) + 4.5;
            int hour12 = ((int)hours % 12 == 0) ? 12 : (int)hours % 12;
            int minutes = (int)(time % 3600 / 60);
            string period = hours >= 12 ? "PM" : "AM";

            return $"{hour12}:{minutes:D2} {period}";
        }
    }
}
