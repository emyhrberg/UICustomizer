using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using UICustomizer.Common.States;
using UICustomizer.UI.Editor;

namespace UICustomizer.Common.Systems
{
    public class LayersSystem : ModSystem
    {
        public static readonly Dictionary<string, bool> LayerStates = [];

        // UI components
        private UserInterface userInterface;
        public LayersState layersState;

        // Handle LayersPanel active
        public static bool IsActive { get; private set; } = false;
        public static void SetActive(bool active) => IsActive = active;
        public static void ToggleActive() => SetActive(!IsActive);

        public override void OnWorldLoad()
        {
            base.OnWorldLoad();
            userInterface = new UserInterface();
            layersState = new LayersState();
            userInterface.SetState(layersState);
        }
        public override void UpdateUI(GameTime gameTime)
        {
            userInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // build dictionary the first time or when new layers appear
            foreach (var l in layers)
                if (!LayerStates.ContainsKey(l.Name))
                    LayerStates[l.Name] = true; // default ON

            // apply user choices (never crash if something disappeared)
            foreach (var l in layers)
                if (LayerStates.TryGetValue(l.Name, out bool show) && !show)
                    l.Active = false;

            // Main overlay
            int mouseText = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
            if (mouseText != -1)
            {
                layers.Insert(mouseText, new LegacyGameInterfaceLayer(
                    "UICustomizer: EditorSystem",
                    () =>
                    {
                        userInterface?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}
