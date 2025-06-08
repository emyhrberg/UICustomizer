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
        // Handle edit mode state.
        public static bool IsActive { get; private set; } = false;
        public static void SetActiveTrue()
        {
            IsActive = true;
            var sys = ModContent.GetInstance<EditorSystem>();
            sys?.state?.editorPanel?.editorTab?.Populate(); //hotfix for a bug where it wouldnt populate after hide all mode
            SetEditing(true);
        }

        public static void SetActiveFalse()
        {
            IsActive = false;
            var sys = ModContent.GetInstance<EditorSystem>();
            EditorPanel panel = sys?.state?.editorPanel;
            panel?.CancelDrag(); // Force stop dragging
            SetDarknessLevel(0);
        }

        public static bool IsEditing = true;
        public static void ToggleEditing() => IsEditing = !IsEditing;
        public static void SetEditing(bool editing) => IsEditing = editing;

        public static void ToggleActive()
        {
            if (IsActive)
                SetActiveFalse();
            else
                SetActiveTrue();
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

            // Exit edit mode if it was active
            if (IsActive)
                SetActiveFalse();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            userInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // Dark mode overlay at bottom
            int firstVanillaLayer = layers.FindIndex(layer => layer.Name == "Vanilla: Interface Logic 1");
            if (firstVanillaLayer != -1)
            {
                layers.Insert(firstVanillaLayer, new LegacyGameInterfaceLayer(
                    "UICustomizer: Dark",
                    () =>
                    {
                        DrawDarkOverlay();
                        return true;
                    },
                    InterfaceScaleType.UI));
            }

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

        # region Dark Mode Overlay
        private static float DarknessLevel = 0.0f;
        public static void SetDarknessLevel(float num) => DarknessLevel = num;
        public static float GetDarknessLevel() => DarknessLevel;


        private static void DrawDarkOverlay()
        {
            // Draw a dark overlay covering the entire screen with the given darkness level
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * DarknessLevel);
        }
        # endregion
    }
}
