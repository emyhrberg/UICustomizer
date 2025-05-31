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
        [Range(-500f, 500f)]
        [Increment(10)]
        [DrawTicks]
        public float ChatOffsetX;

        [DefaultValue(0)]
        [Range(-500f, 500f)]
        [Increment(10)]
        [DrawTicks]
        public float ChatOffsetY;

        [DefaultValue(0)]
        [Range(-500f, 500f)]
        [Increment(10)]
        [DrawTicks]
        public float HotbarOffsetX;

        [DefaultValue(0)]
        [Range(-500f, 500f)]
        [Increment(10)]
        [DrawTicks]
        public float HotbarOffsetY;

        [DefaultValue(0)]
        [Range(-500f, 500f)]
        [Increment(10)]
        [DrawTicks]
        public float MapOffsetX;

        [DefaultValue(0)]
        [Range(-500f, 500f)]
        [Increment(10)]
        [DrawTicks]
        public float MapOffsetY;

        [DefaultValue(0)]
        [Range(-500f, 500f)]
        [Increment(10)]
        [DrawTicks]
        public float InfoAccsOffsetX;

        [DefaultValue(0)]
        [Range(-500f, 500f)]
        [Increment(10)]
        [DrawTicks]
        public float InfoAccsOffsetY;

        [DefaultValue(0)]
        [Range(-500f, 500f)]
        [Increment(10)]
        [DrawTicks]
        public float FancyLifeOffsetX;

        [DefaultValue(0)]
        [Range(-500f, 500f)]
        [Increment(10)]
        [DrawTicks]
        public float FancyLifeOffsetY;

        [DefaultValue(0)]
        [Range(-500f, 500f)]
        [Increment(10)]
        [DrawTicks]
        public float ClassicLifeOffsetX;

        [DefaultValue(0)]
        [Range(-500f, 500f)]
        [Increment(10)]
        [DrawTicks]
        public float ClassicLifeOffsetY;

        [DefaultValue(0)]
        [Range(-500f, 500f)]
        [Increment(10)]
        [DrawTicks]
        public float ClassicManaOffsetX;

        [DefaultValue(0)]
        [Range(-500f, 500f)]
        [Increment(10)]
        [DrawTicks]
        public float ClassicManaOffsetY;

        [DefaultValue(0)]
        [Range(-500f, 500f)]
        [Increment(10)]
        [DrawTicks]
        public float FancyManaOffsetX;

        [DefaultValue(0)]
        [Range(-500f, 500f)]
        [Increment(10)]
        [DrawTicks]
        public float FancyManaOffsetY;

        [DefaultValue(0)]
        [Range(-500f, 500f)]
        [Increment(10)]
        [DrawTicks]
        public float HorizontalLifeBarOffsetX;

        [DefaultValue(0)]
        [Range(-500f, 500f)]
        [Increment(10)]
        [DrawTicks]
        public float HorizontalLifeBarOffsetY;
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