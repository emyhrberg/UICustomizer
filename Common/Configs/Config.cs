using System;
using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using UICustomizer.Common.Systems.MainMenu;

namespace UICustomizer.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("MainMenu")]

        [DefaultValue(true)]
        public bool EditMainMenu = true;

        [DefaultValue(true)]
        public bool ShowMainMenu = true;

        [CustomModConfigItem(typeof(ColorTagConfigElement))]
        public string FillColor;

        [CustomModConfigItem(typeof(ColorTagConfigElement))]
        public string NormalColor;

        [CustomModConfigItem(typeof(ColorTagConfigElement))]
        public string HoverColor;

        [Header("Misc")]

        [DefaultValue(true)]
        public bool ShowMessageWhenEnteringWorld;

        [DefaultValue(true)]
        public bool ShowCombatTextTooltips;

        [DefaultValue(true)]
        public bool DisableItemUseWhileDragging;

        public override void OnChanged()
        {
            base.OnChanged();

            if (ModContent.GetInstance<Config> == null)
            {
                Log.Info("Config is null for some reason");
                return;
            }

            // Update eye toggle
            var sys = ModContent.GetInstance<MainMenuSystem>();
            if (sys == null)
            {
                Log.Info("MainMenuSystem is null for some reason");
                return;
            }

            sys.state.eyeToggle.isOn = Conf.C.ShowMainMenu;

            //var mainmenu = ModContent.GetInstance<MainMenuDraw>();
            //if (mainmenu == null) return;
            //mainmenu.rRatio = NormalColor.R / 255;
            //mainmenu.gRatio = NormalColor.G / 255;
            //mainmenu.bRatio = NormalColor.B / 255;
            //Log.Info("red conf" + Conf.C.NormalColor);
        }
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

        // Instance of the Config class
        // Use it like 'Conf.C.YourConfigField' for easy access to the config values
        public static Config C
        {
            get
            {
                try
                {
                    return ModContent.GetInstance<Config>();
                }
                catch (Exception ex)
                {
                    Log.Error("Error getting config instance: " + ex.Message);
                    return null;
                }
            }
        }
    }
}