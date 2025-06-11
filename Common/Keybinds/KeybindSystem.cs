using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;
using Terraria.ModLoader;
using UICustomizer.Common.Systems;

namespace UICustomizer.Common.Keybinds
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind EditPanelToggle;
        public static ModKeybind LayerPanelToggle;

        public override void Load()
        {
            EditPanelToggle = KeybindLoader.RegisterKeybind(Mod, "Toggle Edit Panel", Keys.N);
            LayerPanelToggle = KeybindLoader.RegisterKeybind(Mod, "Toggle Layer Panel", Keys.M);
        }
    }

    public class KeybindPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.EditPanelToggle.JustPressed)
            {
                EditorSystem.ToggleActive();
            }

            if (KeybindSystem.LayerPanelToggle.JustPressed)
            {
                LayerSystem.ToggleActive();
            }
        }
    }
}
