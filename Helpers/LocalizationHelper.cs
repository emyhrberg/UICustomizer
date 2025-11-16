namespace UIEditor.Helpers
{
    // Loc: Short for Localization
    // This class is used to get the localization text for the UIEditor mod.
    // Is found in en-US.Mods.UIEditor.json and other localization files.
    public static class Loc
    {
        /// <summary>
        /// Gets the text for the given key from the UIEditor localization file.
        /// If no localization is found, the key itself is returned.
        /// Reference:
        /// https://github.com/ScalarVector1/DragonLens/blob/master/Helpers/LocalizationHelper.cs
        /// </summary>
        public static string Get(string key, params object[] args)
        {
            if (Terraria.Localization.Language.Exists($"Mods.UIEditor.{key}"))
            {
                return Terraria.Localization.Language.GetTextValue($"Mods.UIEditor.{key}", args);
            }
            else
            {
                // Key not found in localization, return the key itself.
                // Remove the "Mods.UIEditor." prefix if it exists because it doesnt look good.
                string modifiedKey = key.StartsWith("Mods.UIEditor.") ? key.Substring("Mods.UIEditor.".Length) : key;
                return modifiedKey;
            }
        }
    }
}