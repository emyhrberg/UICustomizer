using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;
using Terraria.ModLoader;
using UIEditor.Core.IngameEditor.Systems;
using UIEditor.Core.LayersEditor;

namespace UIEditor.Common.Keybinds;
public class KeybindSystem : ModSystem
{
    public ModKeybind EditPanelToggle;
    public ModKeybind LayersPanelToggle;

    public override void Load()
    {
        EditPanelToggle = KeybindLoader.RegisterKeybind(Mod, "Toggle Edit Panel", Keys.N);
        LayersPanelToggle = KeybindLoader.RegisterKeybind(Mod, "Toggle Layers Panel", Keys.M);
    }
}

public class KeybindPlayer : ModPlayer
{
    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        KeybindSystem keybindSystem = ModContent.GetInstance<KeybindSystem>();

        if (keybindSystem.EditPanelToggle.JustPressed)
            EditSystem.ToggleActive();

        if (keybindSystem.LayersPanelToggle.JustPressed)
            LayerSystem.ToggleActive();
    }
}