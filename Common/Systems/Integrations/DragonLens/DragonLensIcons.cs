using DragonLens.Core.Systems.ThemeSystem;
using DragonLens.Core.Systems.ToolbarSystem;

namespace UICustomizer.Common.Systems.Integrations.DragonLens
{
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DragonLensIcons : ModSystem
    {
        public override void PostSetupContent()
        {
            //AHHHHHHHHHHHHHHHHHHHHHH
            AddIcons();
        }

        public static void AddIcons()
        {
            if (ModLoader.TryGetMod("DragonLens", out _))
            {
                foreach (var provider in ThemeHandler.allIconProviders.Values)
                {
                    // assign (overwrites if the key exists already) – never throws
                    provider.icons["UIEditor"] = Ass.DragonLensToolIcon.Value;
                }

                // rebuild toolbars *after* icons (and tools) have been injected
                ModContent.GetInstance<ToolbarHandler>().OnModLoad();
            }
        }

        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();

            // AddIcons();
            // This is a workaround to ensure that the icons are added after the mod is loaded.
        }
    }
}

