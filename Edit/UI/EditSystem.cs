using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using UICustomizer.Common.States;
using UICustomizer.Helpers.Layouts;

namespace UICustomizer.Common.Systems
{
    [Autoload(Side = ModSide.Client)]
    public class EditSystem : ModSystem
    {
        // UI components
        public UserInterface ui;
        public EditorState state;

        public static bool IsActive { get; private set; } = false;

        public static void SetActive(bool active)
        {
            IsActive = active;
            var sys = ModContent.GetInstance<EditSystem>();

            if (active)
            {
                sys.ui.SetState(sys.state);
                sys.state.editorPanel.editorTab.Populate(); //hotfix for a bug where it wouldnt populate after hide all mode
            }
            else
            {
                sys.ui.SetState(null);
            }
        }

        public static void ToggleActive()
        {
            // Switch between active and inactive.
            if (IsActive)
                SetActive(false);
            else
                SetActive(true);
            Log.Info("State: " + IsActive);
        }



        public override void OnModLoad()
        {
            DefaultLayouts.CreateAllDefaultLayouts();
        }

        public override void OnWorldLoad()
        {
            ui = new UserInterface();
            state = new EditorState();

            ui.SetState(null);

            // Apply last selected layout
            string lastLayoutName = FileHelper.LoadLastLayoutName();
            LayoutHelper.ApplyLayout(lastLayoutName);

            //SetActive(true); // DEBUG MODE
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (ui?.CurrentState != null)
                ui?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // Main overlay
            int mouseText = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
            if (mouseText != -1)
            {
                layers.Insert(mouseText, new LegacyGameInterfaceLayer(
                    name: "UICustomizer: EditSystem",
                    drawMethod: () =>
                    {
                        if (ui.CurrentState != null)
                        {
                            ui?.Draw(Main.spriteBatch, new GameTime());
                            return true;
                        }
                        return false;
                    },
                    scaleType: InterfaceScaleType.UI));
            }
        }
    }
}