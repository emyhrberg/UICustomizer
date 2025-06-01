using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace UICustomizer.Helpers
{
    /// <summary>
    /// To add a new asset, simply add a new field like:
    /// public static Asset<Texture2D> MyAsset;
    /// </summary>
    public class LoadAssets : ModSystem
    {
        public override void Load()
        {
            _ = Ass.Initialized;
        }
    }

    public static class Ass
    {
        // Add assets here
        public static Asset<Texture2D> Check;
        public static Asset<Texture2D> Uncheck;
        public static Asset<Texture2D> Pluss;
        public static Asset<Texture2D> Minuss;
        public static Asset<Texture2D> Resize;

        // This will automatically initialize the assets
        public static bool Initialized { get; set; }

        static Ass()
        {
            foreach (FieldInfo field in typeof(Ass).GetFields())
            {
                if (field.FieldType == typeof(Asset<Texture2D>))
                {
                    field.SetValue(null, RequestAsset(field.Name));
                }
            }
        }

        private static Asset<Texture2D> RequestAsset(string path)
        {
            string modName = "UICustomizer"; // Replace this with your mod's folder name
            return ModContent.Request<Texture2D>($"{modName}/Assets/" + path, AssetRequestMode.AsyncLoad);
        }
    }
}