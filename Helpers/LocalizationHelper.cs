namespace UICustomizer.Helpers
{
    // Loc: Short for Localization
    // This class is used to get the localization text for the UICustomizer mod.
    // Is found in en-US.Mods.UICustomizer.json and other localization files.
    public static class Loc
    {
        /// <summary>
        /// Gets the text for the given key from the UICustomizer localization file.
        /// If no localization is found, the key itself is returned.
        /// Reference:
        /// https://github.com/ScalarVector1/DragonLens/blob/master/Helpers/LocalizationHelper.cs
        /// </summary>
        public static string Get(string key, params object[] args)
        {
            if (Terraria.Localization.Language.Exists($"Mods.UICustomizer.{key}"))
            {
                return Terraria.Localization.Language.GetTextValue($"Mods.UICustomizer.{key}", args);
            }
            else
            {
                // Key not found in localization, return the key itself.
                // Remove the "Mods.UICustomizer." prefix if it exists because it doesnt look good.
                string modifiedKey = key.StartsWith("Mods.UICustomizer.") ? key.Substring("Mods.UICustomizer.".Length) : key;
                return modifiedKey;
            }
        }
    }
}