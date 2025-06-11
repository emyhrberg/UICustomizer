using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace UICustomizer.Helpers
{
    /// <summary>
    /// Static class to hold all assets used in the mod.
    /// </summary>
    public static class Ass
    {
        // Add assets here
        public static Asset<Texture2D> CheckActive;
        public static Asset<Texture2D> CheckActiveHover;
        public static Asset<Texture2D> CheckInactive;
        public static Asset<Texture2D> CheckInactiveHover;
        public static Asset<Texture2D> Close;
        public static Asset<Texture2D> CloseHover;
        public static Asset<Texture2D> EditorIcon;
        public static Asset<Texture2D> EyeClosed;
        public static Asset<Texture2D> EyeClosedHover;
        public static Asset<Texture2D> EyeOpen;
        public static Asset<Texture2D> EyeOpenHover;
        public static Asset<Texture2D> Hitbox;
        public static Asset<Texture2D> LayersIcon;
        public static Asset<Texture2D> Minus;
        public static Asset<Texture2D> MinusHover;
        public static Asset<Texture2D> Plus;
        public static Asset<Texture2D> PlusHover;
        public static Asset<Texture2D> Resize;
        public static Asset<Texture2D> Slider;
        public static Asset<Texture2D> SliderHighlight;

        // This bool automatically initializes all specified assets
        public static bool Initialized { get; set; }

        static Ass()
        {
            foreach (FieldInfo field in typeof(Ass).GetFields())
            {
                if (field.FieldType == typeof(Asset<Texture2D>))
                {
                    string modName = "UICustomizer";
                    string path = field.Name;
                    var asset = ModContent.Request<Texture2D>($"{modName}/Assets/{path}", AssetRequestMode.AsyncLoad);
                    field.SetValue(null, asset);
                }
            }
        }
    }

    /// <summary>
    /// System that automatically initializes assets
    /// </summary>
    public class LoadAssets : ModSystem
    {
        public override void Load()
        {
            _ = Ass.Initialized;
        }
    }
}