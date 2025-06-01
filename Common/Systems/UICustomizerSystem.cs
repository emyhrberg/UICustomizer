using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using UICustomizer.UI;

namespace UICustomizer.Common.Systems
{
    [Autoload(Side = ModSide.Client)]
    internal class UICustomizerSystem : ModSystem
    {
        // Handle edit mode state.
        public static bool EditModeActive { get; private set; } = true;
        public static void EnterEditMode()
        {
            EditModeActive = true;

            // Reset position
            var sys = ModContent.GetInstance<UICustomizerSystem>();
            if (sys == null || sys.state == null)
                return;

            sys.state.panel.SetDefaultSizeAndPosition();

            // Force outline and names to be unchecked
            // sys.state.panel.editorTab.CheckboxOutline.SetState(CheckboxState.Unchecked);
            // sys.state.panel.editorTab.CheckboxNames.SetState(CheckboxState.Unchecked);
        }
        public static void ExitEditMode()
        {
            EditModeActive = false;

            // Force stop dragging
            var sys = ModContent.GetInstance<UICustomizerSystem>();
            if (sys == null || sys.state == null)
                return;

            sys.state.panel.CancelDrag();
        }

        // UI components
        public UserInterface userInterface;
        public UICustomizerState state;

        public override void OnModLoad()
        {
            base.OnModLoad();

            LayoutJsonHelper.EnsureDefaultLayoutsExist();
        }

        public override void OnWorldLoad()
        {
            base.OnWorldLoad();
            userInterface = new UserInterface();
            state = new UICustomizerState();
            userInterface.SetState(state);

            // Apply last selected layout
            string lastLayout = LayoutJsonHelper.LoadLastLayoutName();
            if (LayoutJsonHelper.GetLayouts().Contains(lastLayout))
            {
                LayoutJsonHelper.ApplyLayout(lastLayout);
                LayoutJsonHelper.CurrentLayoutName = lastLayout;
            }
            else
            {
                LayoutJsonHelper.ApplyLayout("Default");
                LayoutJsonHelper.CurrentLayoutName = "Default";
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            userInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseText = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
            if (mouseText != -1)
            {
                layers.Insert(mouseText, new LegacyGameInterfaceLayer(
                    "UICustomizer: UI",
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
