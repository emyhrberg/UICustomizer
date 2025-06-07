using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace UICustomizer.Helpers
{
    public static class Log
    {
        // Helper for time to log a message once every x seconds
        private static DateTime lastLogTime = DateTime.UtcNow;

        private static Mod ModInstance
        {
            get
            {
                try
                {
                    return ModLoader.GetMod("UICustomizer");
                }
                catch (Exception ex)
                {
                    Error("Error getting mod instance: " + ex.Message);
                    return null;
                }
            }
        }

        private static DateTime lastChatTime = DateTime.UtcNow; // Add separate tracking for chat

        /// <summary>
        /// Prints a message to Terraria's chat system with cooldown.
        /// </summary>
        /// <param name="msg">The message to display</param>
        /// <param name="ms">Cooldown in milliseconds before allowing another message</param>
        public static void ChatSlow(string msg, float ms=1000)
        {
            // Use TimeSpan to create the cooldown interval
            TimeSpan interval = TimeSpan.FromMilliseconds(ms);
            bool timeElapsed = DateTime.UtcNow - lastChatTime >= interval;

            if (timeElapsed)
            {
                Main.NewText(msg);
                lastChatTime = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Log a message once every x second(s)
        /// </summary>
        public static void SlowInfo(string message, int seconds = 1, [CallerFilePath] string callerFilePath = "")
        {
            // Extract the class name from the caller's file path.
            string className = Path.GetFileNameWithoutExtension(callerFilePath);
            var instance = ModInstance;
            if (instance == null || instance.Logger == null)
                return; // Skip logging if the mod is unloading or null

            // Use TimeSpanFactory to create a x-second interval.
            TimeSpan interval = TimeSpan.FromSeconds(seconds);
            bool timeElapsed = DateTime.UtcNow - lastLogTime >= interval;
            if (timeElapsed)
            {
                // Prepend the class name to the log message.
                instance.Logger.Info($"[{className}] {message}");
                lastLogTime = DateTime.UtcNow;
            }
        }

        public static void Info(string message, [CallerFilePath] string callerFilePath = "")
        {
            // Extract the class name from the caller's file path.
            string className = Path.GetFileNameWithoutExtension(callerFilePath);
            var instance = ModInstance;
            if (instance == null || instance.Logger == null)
                return; // Skip logging if the mod is unloading or null

            // Prepend the class name to the log message.
            instance.Logger.Info($"[{className}] {message}");
        }

        public static void Warn(string message)
        {
            var instance = ModInstance;
            if (instance == null || instance.Logger == null)
                return; // Skip logging if the mod is unloading or null

            instance.Logger.Warn(message);
        }

        public static void Error(string message)
        {
            var instance = ModInstance;
            if (instance == null || instance.Logger == null)
                return; // Skip logging if the mod is unloading or null

            instance.Logger.Error(message);
        }
    }
}