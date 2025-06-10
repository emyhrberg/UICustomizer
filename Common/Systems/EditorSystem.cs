using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using UICustomizer.Common.States;
using UICustomizer.Helpers.Layouts;
using UICustomizer.UI;
using UICustomizer.UI.Editor;

namespace UICustomizer.Common.Systems
{
    [Autoload(Side = ModSide.Client)]
    public class EditorSystem : ModSystem
    {
        #region Active State
        public static bool IsActive { get; private set; } = false;

        public static void SetActive(bool active)
        {
            if (active)
            {
                IsActive = true;
                var sys = ModContent.GetInstance<EditorSystem>();
                sys?.state?.editorPanel?.editorTab?.Populate(); //hotfix for a bug where it wouldnt populate after hide all mode
                SetEditing(true);
            }
            else
            {
                IsActive = false;
                var sys = ModContent.GetInstance<EditorSystem>();
                EditorPanel panel = sys?.state?.editorPanel;
                panel?.CancelDrag(); // Force stop dragging
                DarkSystem.SetDarknessLevel(0);
            }
        }

        #endregion

        #region Editing State

        public static bool IsEditing = false;
        public static void SetEditing(bool editing) => IsEditing = editing;
        #endregion

        public static void ToggleActive()
        {
            // Switch between active and inactive.
            if (IsActive)
                SetActive(false);
            else
                SetActive(true);
        }

        // UI components
        public UserInterface userInterface;
        public EditorState state;

        public override void OnModLoad()
        {
            base.OnModLoad();

            DefaultLayouts.CreateAllDefaultLayouts();
        }

        public override void OnWorldLoad()
        {
            base.OnWorldLoad();
            userInterface = new UserInterface();
            state = new EditorState();
            userInterface.SetState(state);

            // Apply last selected layout
            string lastLayoutName = FileHelper.LoadLastLayoutName();
            LayoutHelper.ApplyLayout(lastLayoutName);

            SetActive(true); // DEBUG MODE
        }

        public override void UpdateUI(GameTime gameTime)
        {
            userInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // Main overlay
            int mouseText = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
            if (mouseText != -1)
            {
                layers.Insert(mouseText, new LegacyGameInterfaceLayer(
                    name: "UICustomizer: EditorSystem",
                    drawMethod: () =>
                    {
                        // hide all except save button mode
                        if (SaveButtonOnlySystem.IsHideMode)
                        {
                            return true;
                        }

                        userInterface?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    scaleType: InterfaceScaleType.UI));
            }
        }
    }
}