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

        [DefaultValue(true)]
        public bool ShowBackToMainMenu = true;

        [CustomModConfigItem(typeof(ColorTagConfigElement))]
        public string FillColor;    // null → new(142,142,142)

        [CustomModConfigItem(typeof(ColorTagConfigElement))]
        public string OutlineColor; // null → Color.Black

        [CustomModConfigItem(typeof(ColorTagConfigElement))]
        public string HoverColor;   // null → Main.OurFavoriteColor or new(255,215,0)

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

            // Config null check
            if (ModContent.GetInstance<Config> == null)
            {
                Log.Info("Config is null in Config::OnChanged for some reason");
                return;
            }

            // Main menu system null check
            var sys = ModContent.GetInstance<MainMenuSystem>();
            if (sys == null)
            {
                Log.Info("MainMenuSystem is null in Config::OnChanged for some reason");
                return;
            }

            // Update eye toggle
            sys.state.eyeToggle.isOn = Conf.C.ShowMainMenu;

            //var mainmenu = ModContent.GetInstance<MainMenuDraw>();
            //if (mainmenu == null) return;
            //mainmenu.rRatio = OutlineColor.R / 255;
            //mainmenu.gRatio = OutlineColor.G / 255;
            //mainmenu.bRatio = OutlineColor.B / 255;
            //Log.Info("red conf" + Conf.C.OutlineColor);
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