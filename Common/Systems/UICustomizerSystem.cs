using System.Collections.Generic;
using UICustomizer.Helpers.Layouts;
using UICustomizer.UI;

namespace UICustomizer.Common.Systems
{
    public enum TabType
    {
        Editor,
        Themes,
        Layers
    }

    [Autoload(Side = ModSide.Client)]
    public class UICustomizerSystem : ModSystem
    {
        // public static Tab LastSelectedTab { get; set; }
        public static TabType? LastSelectedTabType { get; private set; } = null;
        public static void SetLastSelectedTab(TabType t) => LastSelectedTabType = t;

        // Handle edit mode state.
        public static bool EditModeActive { get; private set; } = false;
        public static void EnterEditMode()
        {
            EditModeActive = true;
        }
        public static void ExitEditMode()
        {
            EditModeActive = false;

            // Get panel
            var sys = ModContent.GetInstance<UICustomizerSystem>();
            if (sys == null || sys.state == null)
                return;
            Panel panel = sys.state.panel;

            // Force stop dragging
            panel.CancelDrag();
        }

        // UI components
        public UserInterface userInterface;
        public UICustomizerState state;

        public override void OnModLoad()
        {
            base.OnModLoad();

            DefaultLayouts.CreateAllDefaultLayouts();
        }

        public override void OnWorldLoad()
        {
            base.OnWorldLoad();
            userInterface = new UserInterface();
            state = new UICustomizerState();
            userInterface.SetState(state);

            // Apply last selected layout
            string lastLayoutName = FileHelper.LoadLastLayoutName();
            LayoutHelper.ApplyLayout(lastLayoutName);

            // Exit edit mode if it was active
            if (EditModeActive)
                ExitEditMode();
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
