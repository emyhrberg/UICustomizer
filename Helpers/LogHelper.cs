using System;
using System.IO;
using System.Runtime.CompilerServices;
using log4net.Core;
using Terraria.ModLoader;

namespace UICustomizer.Helpers
{
    public static class LogHelper
    {
        // Helper for time to log a message once every x seconds
        private static DateTime lastLogTime = DateTime.UtcNow;

        /// <summary>
        /// Sends a log message with a class name prefix.
        /// If only a message is provided, it defaults to a constant log level of Info.
        /// </summary>
        /// <param name="message">the message to send</param>
        /// <param name="logLevel">the severity level of the message</param>
        /// <param name="seconds">the delay in seconds between which to send messages</param>
        /// <param name="callerFilePath">the file source of the sent message, displayed in log as [CallerFilePath]: Message...</param>
        public static void Log(string message, Level logLevel = default, int seconds = 1, [CallerFilePath] string callerFilePath = "")
        {
            // Ensure mod is active.
            var instance = ModInstance;
            if (instance == null || instance.Logger == null)
                return; // Skip logging if the mod is unloading or null

            // Default to a standard log level if no argument is provided.
            logLevel ??= Level.Info;

            // Extract the class name from the caller's file path.
            string className = Path.GetFileNameWithoutExtension(callerFilePath);

            // Use TimeSpanFactory to create a x-second interval.
            TimeSpan interval = TimeSpan.FromSeconds(seconds);
            bool timeElapsed = DateTime.UtcNow - lastLogTime >= interval;
            if (!timeElapsed)
            {
                return;
            }
            lastLogTime = DateTime.UtcNow;

            // Prepend the class name to the log message.
            string msgToLog = $"[{className}] {message}";

            switch (logLevel?.Name?.ToUpperInvariant())
            {
                case "DEBUG":
                    instance.Logger.Debug(msgToLog);
                    break;
                case "INFO":
                    instance.Logger.Info(msgToLog);
                    break;
                case "WARN":
                    instance.Logger.Warn(msgToLog);
                    break;
                case "ERROR":
                    instance.Logger.Error(msgToLog);
                    break;
                default:
                    instance.Logger.Info(msgToLog);
                    break;
            }
        }



        /// <summary>
        /// Safely gets the instance of the UICustomizer mod.
        /// </summary>
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
                    Console.WriteLine($"Error getting UICustomizer mod instance: {ex.Message}");
                    return null;
                }
            }
        }
    }
}