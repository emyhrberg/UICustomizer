using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI;

namespace UICustomizer.Common.Systems
{
    [Autoload(Side = ModSide.Client)]
    internal class UICustomizerSystem : ModSystem
    {
        // Handle edit mode state.
        public static bool EditModeActive { get; private set; } = false;
        public static void EnterEditMode()
        {
            ModContent.GetInstance<UICustomizerSystem>().uiCustomizerState.saveButton.Active = true;
            ModContent.GetInstance<UICustomizerSystem>().uiCustomizerState.cancelButton.Active = true;
            EditModeActive = true;
            Main.NewText("UI: Editing...", Color.OrangeRed);
        }
        public static void ExitEditMode()
        {
            ModContent.GetInstance<UICustomizerSystem>().uiCustomizerState.saveButton.Active = false;
            ModContent.GetInstance<UICustomizerSystem>().uiCustomizerState.cancelButton.Active = false;
            EditModeActive = false;
            Main.NewText("UI: Saved!", Color.LightGreen);
        }

        // UI.
        public UserInterface userInterface;
        public UICustomizerState uiCustomizerState;

        public override void OnWorldLoad()
        {
            userInterface = new UserInterface();
            uiCustomizerState = new UICustomizerState();
            userInterface.SetState(uiCustomizerState);
        }


        public override void UpdateUI(GameTime gameTime)
        {
            userInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "UICustomizer: UICustomizerSystem",
                    () => { userInterface?.Draw(Main.spriteBatch, new GameTime()); return true; },
                    InterfaceScaleType.UI)
                );

                layers.Insert(index + 1, new LegacyGameInterfaceLayer(
            "UICustomizer: Drag Debug",
            DrawDebugRects,
            InterfaceScaleType.UI));
            }
        }

        private bool DrawDebugRects()
        {
            if (!UICustomizerSystem.EditModeActive) return true;

            SpriteBatch sb = Main.spriteBatch;
            Texture2D px = TextureAssets.MagicPixel.Value;

            void Box(Rectangle r, Color c, string label)
            {
                // thin outline
                sb.Draw(px, new Rectangle(r.Left, r.Top, r.Width, 1), c); // top
                sb.Draw(px, new Rectangle(r.Left, r.Bottom - 1, r.Width, 1), c); // bottom
                sb.Draw(px, new Rectangle(r.Left, r.Top, 1, r.Height), c); // left
                sb.Draw(px, new Rectangle(r.Right - 1, r.Top, 1, r.Height), c); // right

                // draw label
                Vector2 labelPosition = new Vector2(r.Center.X, r.Top - 20); // position above the box
                Utils.DrawBorderString(sb, label, labelPosition, c, 0.8f, 0.5f, 0.5f);
            }

            Box(DragSystem.HotbarBounds(), Color.Cyan * 0.8f, "Hotbar");
            Box(DragSystem.ChatBounds(), Color.Lime * 0.8f, "Chat");

            return true;
        }
    }
}
