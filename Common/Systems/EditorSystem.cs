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
            var sys = ModContent.GetInstance<EditorSystem>();
            if (active)
            {
                IsActive = true;
                // attach the UI tree so it receives input and draws
                //sys.userInterface.SetState(sys.state);
                // hotfix: repopulate after toggling back on
                sys.state.editorPanel.editorTab.Populate();
                SetEditing(true);
            }
            else
            {
                IsActive = false;
                // detach the UI so it no longer handles clicks or draws
                //sys.userInterface.SetState(null);
                var panel = sys.state?.editorPanel;
                panel?.CancelDrag(); // force-stop any drag in progress
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
            SetActive(!IsActive);
        }

        // singleton instance
        public static EditorSystem Instance;

        // UI components
        public UserInterface userInterface;
        public EditorState state;

        public override void OnModLoad()
        {
            base.OnModLoad();
            Instance = this;
            DefaultLayouts.CreateAllDefaultLayouts();
        }

        public override void OnWorldLoad()
        {
            base.OnWorldLoad();

            userInterface = new UserInterface();
            state = new EditorState();
            // don't call SetState here—only attach when SetActive(true) is invoked

            // apply last layout
            string lastLayoutName = FileHelper.LoadLastLayoutName();
            LayoutHelper.ApplyLayout(lastLayoutName);

            //SetActive(true); // DEBUG
        }

        public override void UpdateUI(GameTime gameTime)
        {
            //if (!IsActive) return;
            userInterface.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // only insert our layer when active
            if (!IsActive)
                return;

            int mouseText = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
            if (mouseText != -1)
            {
                layers.Insert(mouseText, new LegacyGameInterfaceLayer(
                    name: "UICustomizer: EditorSystem",
                    drawMethod: () =>
                    {
                        // if we're in "hide all except save button" mode, skip
                        if (SaveButtonOnlySystem.IsHideMode)
                            return true;

                        userInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    scaleType: InterfaceScaleType.UI));
            }
        }
    }
}
