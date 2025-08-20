using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;
using Terraria.ModLoader;
using UICustomizer.EditMode.System;

namespace UICustomizer.Common.Keybinds
{
    public class KeybindSystem : ModSystem
    {
        public ModKeybind EditPanelToggle;

        public override void Load()
        {
            EditPanelToggle = KeybindLoader.RegisterKeybind(Mod, "Toggle Edit Panel", Keys.N);
        }
    }

    public class KeybindPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            KeybindSystem keybindSystem = ModContent.GetInstance<KeybindSystem>();
            var sys = ModContent.GetInstance<EditSystem>();

            if (keybindSystem.EditPanelToggle.JustPressed)
                sys.Toggle();
        }
    }
}