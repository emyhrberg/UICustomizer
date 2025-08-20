using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using UICustomizer.MainMenu.Helpers;
using UICustomizer.MainMenu.Hooks;

namespace UICustomizer.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("UIEditor")]

        [DefaultValue("")]
        public string Layout;

        [Header("MainMenu")]

        [DefaultValue(true)]
        public bool EditMainMenu = true;

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

        public override void OnChanged()
        {
            base.OnChanged();

            if (Conf.C == null)
            {
                Log.Error("Config is null OnChanged.");
                return;
            }

            Log.Info("change");

            if (!Conf.C.EditMainMenu)
            {
                var sys = ModContent.GetInstance<MainMenuSystem>();
                if (sys == null)
                {
                    Log.Error("MainMenuSystem is null OnChanged.");
                    return;
                }
                sys.ui.SetState(null);
            }

            ApplyMainMenuTextColor();
            ApplyMainMenuTime();
            ApplyMainMenuBackground();
            ApplyMainMenuLogo();
            ApplyMainMenuDraw();
        }

        private void ApplyMainMenuTextColor()
        {
            Color fillColor = ColorHelper.HexToColor(Conf.C.MainMenuTextColor.FillColor);
            Color outlineColor = ColorHelper.HexToColor(Conf.C.MainMenuTextColor.OutlineColor);
            Color hoverColor = ColorHelper.HexToColor(Conf.C.MainMenuTextColor.HoverColor);

            MainMenuTextColorHook.FillColor = fillColor;
            MainMenuTextColorHook.OutlineColor = outlineColor;
            MainMenuTextColorHook.HoverColor = hoverColor;
        }

        private void ApplyMainMenuTime()
        {
            TimeSpeedHook.Speed = Conf.C.MainMenuTime.Speed;
            ParallaxSpeedHook.Speed = Conf.C.MainMenuTime.ParallaxSpeed;
        }

        private void ApplyMainMenuBackground()
        {
            BackgroundHook.Scale = Conf.C.MainMenuBackground.Scale;
            BackgroundHook.Rotation = Conf.C.MainMenuBackground.Rotation;
            BackgroundHook.OffsetX = Conf.C.MainMenuBackground.OffsetX;
            BackgroundHook.OffsetY = Conf.C.MainMenuBackground.OffsetY;
            BackgroundHook.Color = ColorHelper.HexToColor(Conf.C.MainMenuBackground.Color);
            BackgroundHook.CustomBackgroundTexture = FileUploadHelper.ReadAndCreateTextureFromPath(Conf.C.MainMenuBackground.BackgroundFileName);
        }

        private void ApplyMainMenuLogo()
        {
            LogoHook.Scale = Conf.C.MainMenuLogo.Scale;
            LogoHook.Rotation = Conf.C.MainMenuLogo.Rotation;
            LogoHook.OffsetX = Conf.C.MainMenuLogo.OffsetX;
            LogoHook.OffsetY = Conf.C.MainMenuLogo.OffsetY;
            LogoHook.Color = ColorHelper.HexToColor(Conf.C.MainMenuLogo.Color);
            LogoHook.CustomLogoTexture = FileUploadHelper.ReadAndCreateTextureFromPath(Conf.C.MainMenuLogo.LogoFileName);
        }

        private void ApplyMainMenuDraw()
        {
            SkipSkyDrawHook.IsDrawing = Conf.C.MainMenuDraw.DrawSky;
            SkipSunDrawHook.IsDrawing = Conf.C.MainMenuDraw.DrawSun;
        }
    }

    public class MainMenuTextColor
    {
        [DefaultValue("#8E8E8E")]
        [CustomModConfigItem(typeof(ColorTagConfigElement))]
        public string FillColor = "#8E8E8E";    // null → new(142,142,142)=#8E8E8E

        [DefaultValue("#000000")]
        [CustomModConfigItem(typeof(ColorTagConfigElement))]
        public string OutlineColor = "#000000"; // null → Color.Black=#000000

        [DefaultValue("#FFD700")]
        [CustomModConfigItem(typeof(ColorTagConfigElement))]
        public string HoverColor = "#FFD700";   // null → Main.OurFavoriteColor or new(255,215,0)=#FFD700

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
        [DefaultValue(0f)]
        [Range(0f, 86400f)]
        public float Time = 0f;

        // Speed in ticks int
        [DefaultValue(1f)]
        [Range(0f, 100f)]
        public float Speed = 1f;

        // Parallax speed in int
        [DefaultValue(5f)]
        [Range(0f, 100f)]
        public float ParallaxSpeed = 5f;
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
        [DefaultValue(true)] public bool DrawClouds = true;
        [DefaultValue(true)] public bool DrawLogo = true;
        [DefaultValue(true)] public bool DrawSky = true;

        // Second column
        [DefaultValue(true)] public bool DrawStars = true;
        [DefaultValue(true)] public bool DrawSun = true;
        [DefaultValue(true)] public bool DrawText = true;
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