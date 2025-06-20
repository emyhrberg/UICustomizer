using System;
using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using UICustomizer.Common.Systems.Hooks.MainMenu;
using UICustomizer.Helpers;

namespace UICustomizer.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("UIEditor")]

        [DefaultValue(true)]
        public bool ShowEditButton;

        [DefaultValue(true)]
        public bool ShowLayersButton;

        [DefaultValue(true)]
        public bool ShowMessageWhenEnteringWorld;

        [DefaultValue(true)]
        public bool ShowCombatTextTooltips;

        [DefaultValue(true)]
        public bool DisableItemUseWhileDragging;

        

        [Header("MainMenu")]

        [DefaultValue(true)]
        public bool EditMainMenu;

        [DefaultValue(typeof(Color), "0, 0, 0, 0")]
        public Color MainMenuTextColor = Color.Black;

        public override void OnChanged()
        {
            base.OnChanged();

            var mainmenu = ModContent.GetInstance<MainMenuDraw>();
            if (mainmenu == null) return;
            mainmenu.rRatio = MainMenuTextColor.R / 255;
            mainmenu.gRatio = MainMenuTextColor.G / 255;
            mainmenu.bRatio = MainMenuTextColor.B / 255;
            Log.Info("red conf" + Conf.C.MainMenuTextColor);
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