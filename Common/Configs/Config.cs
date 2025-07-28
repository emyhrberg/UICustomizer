using System.ComponentModel;
using Microsoft.Xna.Framework.Graphics;
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

        [Expand(false, false)]
        public MainMenuTextColor MainMenuTextColor = new();

        [Expand(false, false)]
        public MainMenuTime MainMenuTime = new();

        [Expand(false, false)]
        public MainMenuBackground MainMenuBackground = new();

        [Expand(false, false)]
        public MainMenuLogo MainMenuLogo = new();

        [Expand(false, false)]
        public MainMenuDraw MainMenuDraw = new();

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
            Log.Info("eyetoggleon?" + sys.state.eyeToggle.isOn);
            sys.state.eyeToggle.isOn = Conf.C.ShowMainMenu;
            Log.Info("eyetoggleon?" + sys.state.eyeToggle.isOn);

            //var mainmenu = ModContent.GetInstance<MainMenuDraw>();
            //if (mainmenu == null) return;
            //mainmenu.rRatio = OutlineColor.R / 255;
            //mainmenu.gRatio = OutlineColor.G / 255;
            //mainmenu.bRatio = OutlineColor.B / 255;
            //Log.Info("red conf" + Conf.C.OutlineColor);
        }
    }

    public class MainMenuTextColor
    {
        [DefaultValue("#8E8E8E")]
        [CustomModConfigItem(typeof(ColorTagConfigElement))]
        public string FillColor;    // null → new(142,142,142)=#8E8E8E

        [DefaultValue("#000000")]
        [CustomModConfigItem(typeof(ColorTagConfigElement))]
        public string OutlineColor; // null → Color.Black=#000000

        [DefaultValue("#FFD700")]
        [CustomModConfigItem(typeof(ColorTagConfigElement))]
        public string HoverColor;   // null → Main.OurFavoriteColor or new(255,215,0)=#FFD700

        [DefaultValue(1f)]
        [Range(0.1f, 10f)]
        public float Scale = 1f; // Scale for the text

        [DefaultValue(0f)]
        [Range(-1000f, 1000f)]
        public float OffsetX = 0f; // X position offset

        [DefaultValue(0f)]
        [Range(-1000f, 1000f)]
        public float OffsetY = 0f; // Y position offset
    }

    public class MainMenuTime
    {
        [DefaultValue(false)]
        public bool IsPaused;

        // Time in ticks int
        [DefaultValue(0)]
        [Range(0f, 86400f)]
        public float Time;

        // Speed in ticks int
        [DefaultValue(1)]
        [Range(0f, 100f)]
        public float Speed;

        // Parallax speed in int
        [DefaultValue(5)]
        [Range(0f, 100f)]
        public float ParallaxSpeed;
    }

    public class MainMenuBackground
    {
        [Range(0.0f, 10f)]
        [DefaultValue(1f)]
        public float Scale = 1f;

        [Range(0.0f, 6.28f)]
        [DefaultValue(0f)]
        public float Rotation = 0f;

        [DefaultValue(0f)]
        [Range(-1000f, 1000f)]
        public float OffsetX = 0f; // X position offset

        [DefaultValue(0f)]
        [Range(-1000f, 1000f)]
        public float OffsetY = 0f; // Y position offset

        [DefaultValue("#FFFFFF")]
        [CustomModConfigItem(typeof(ColorTagConfigElement))]
        public string Color = "#FFFFFF"; // White by default

        public string BackgroundFileName;
    }
    public class MainMenuLogo
    {
        [Range(0.0f, 10f)]
        [DefaultValue(1f)]
        public float Scale = 1f;

        [Range(0.0f, 6.28f)]
        [DefaultValue(0f)]
        public float Rotation = 0f;

        [DefaultValue(0f)]
        [Range(-1000f, 1000f)]
        public float OffsetX = 0f; // X position offset

        [DefaultValue(0f)]
        [Range(-1000f, 1000f)]
        public float OffsetY = 0f; // Y position offset

        [DefaultValue("#FFFFFF")]
        [CustomModConfigItem(typeof(ColorTagConfigElement))]
        public string Color = "#FFFFFF"; // White by default

        public string LogoFileName;
    }

    public class MainMenuDraw
    {
        // First column
        [DefaultValue(true)] public bool DrawBackground = true;
        [DefaultValue(true)] public bool DrawSun = true;
        [DefaultValue(true)] public bool DrawSky = true;

        // Second column
        [DefaultValue(true)] public bool DrawLogo = true;
        [DefaultValue(true)] public bool DrawText = true;
        [DefaultValue(true)] public bool DrawStars = true;
        [DefaultValue(true)] public bool DrawVersion = true;
    }

    public static class Conf
    {
        public static void Save()
        {
            try { ConfigManager.Save(C); }
            catch { Log.Error("An error occurred while manually saving ModConfig!."); }
        }

        /// <summary> Easy access to the ModConfig instance. </summary>
        public static Config C => ModContent.GetInstance<Config>();
    }
}