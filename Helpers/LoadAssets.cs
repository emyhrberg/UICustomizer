using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace UIEditor.Helpers
{
    /// <summary>
    /// Static class to hold all assets used in the mod.
    /// </summary>
    public static class Ass
    {
        // General assets
        public static Asset<Texture2D> Inventory_Tick_Off;
        public static Asset<Texture2D> Inventory_Tick_On;

        // Edit menu
        public static Asset<Texture2D> DarkPanel;
        public static Asset<Texture2D> EditorIconSmall;
        public static Asset<Texture2D> EditorIconSmallHover;
        public static Asset<Texture2D> Hitbox;

        // Main menu
        public static Asset<Texture2D> Pause;
        public static Asset<Texture2D> Play;
        public static Asset<Texture2D> Reset;
        public static Asset<Texture2D> Slider;
        public static Asset<Texture2D> SliderGradient;
        public static Asset<Texture2D> SliderHighlight;
        public static Asset<Texture2D> SliderTime;
        public static Asset<Texture2D> SmallPanelHighlight;

        // Letters
        public static Asset<Texture2D> C;
        public static Asset<Texture2D> F;
        public static Asset<Texture2D> H;
        public static Asset<Texture2D> O;
        public static Asset<Texture2D> P;
        public static Asset<Texture2D> R;
        public static Asset<Texture2D> S;
        public static Asset<Texture2D> T;

        // This bool automatically initializes all specified assets
        public static bool Initialized { get; set; }

        static Ass()
        {
            foreach (FieldInfo field in typeof(Ass).GetFields())
            {
                if (field.FieldType == typeof(Asset<Texture2D>))
                {
                    string modName = "UIEditor";
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