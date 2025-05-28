using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace UICustomizer.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("UIPositions")]

        [DefaultValue(0)]
        public float ChatOffsetX;

        [DefaultValue(0)]
        public float ChatOffsetY;

        [DefaultValue(0)]
        public float HotbarOffsetX;

        [DefaultValue(0)]
        public float HotbarOffsetY;
    }

    public static class Conf
    {
        public static void Save()
        {
            try
            {
                ConfigManager.Save(C);
            }
            catch
            {
                Log.Error("An error occurred while manually saving ModConfig!.");
            }
        }

        // This is an instance of the Config class
        // Use it like Conf.C.YourConfigField for easy access to the config values
        public static Config C => ModContent.GetInstance<Config>();
    }
}