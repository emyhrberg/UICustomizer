using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Keybinds
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind EditModeKeybind;

        public override void Load()
        {
            EditModeKeybind = KeybindLoader.RegisterKeybind(Mod, "Toggle Edit Mode", Keys.N);
        }
    }

    public class KeybindPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.EditModeKeybind.JustPressed)
                EditModeSystem.ToggleActive();
        }
    }
}