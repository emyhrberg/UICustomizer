using System;
using Terraria.GameContent;
using UICustomizer.Common.Systems.Hooks;

namespace UICustomizer.Common.Systems
{
    public class DragSystem : ModSystem
    {
        private Vector2 _mouseStart;
        private Vector2 _offsetStart;
        private Func<Rectangle>? _dragSource;   // null = no drag in progress

        //  Update logic. Runs every frame before drawing happens
        public override void PostUpdateInput()
        {
            // If edit mode is not active, do nothing
            if (!UICustomizerSystem.EditModeActive)
                return;

            OpenChatIfEditModeActive();

            HandleDrag(ChatBounds, ref ChatHook.OffsetX, ref ChatHook.OffsetY);
            HandleDrag(HotbarBounds, ref HotbarHook.OffsetX, ref HotbarHook.OffsetY);
        }

        private void OpenChatIfEditModeActive()
        {
            if (UICustomizerSystem.EditModeActive)
            {
                if (!Main.drawingPlayerChat)               // chat is closed → open it
                    Main.OpenPlayerChat();                 // vanilla helper
            }
            else if (Main.drawingPlayerChat)               // left edit-mode → tidy up
                Main.ClosePlayerChat();
        }

        private void HandleDrag(Func<Rectangle> getBoundsUI,
                        ref float offsetX, ref float offsetY)
        {
            float uiScale = Main.UIScale;                   
            Vector2 mouseUI = Main.MouseScreen / uiScale;   // convert once

            /* start drag */
            if (_dragSource is null &&
                Main.mouseLeft &&
                getBoundsUI().Contains(mouseUI.ToPoint()))
            {
                _dragSource = getBoundsUI;
                _mouseStart = mouseUI;                      // store in UI units
                _offsetStart = new Vector2(offsetX, offsetY);
                Main.LocalPlayer.mouseInterface = true;
            }

            /* update drag */
            if (_dragSource == getBoundsUI)
            {
                Vector2 deltaUI = mouseUI - _mouseStart;     // already de-scaled
                offsetX = _offsetStart.X + deltaUI.X;
                offsetY = _offsetStart.Y + deltaUI.Y;

                if (!Main.mouseLeft)
                    _dragSource = null;
            }
        }

        public static bool MouseInBounds(Rectangle bounds)
        {
            float uiScale = Main.UIScale;
            Vector2 mouseUI = Main.MouseScreen / uiScale;   // convert once

            return bounds.Contains(mouseUI.ToPoint());
        }

        public static Rectangle ChatBounds()
        {
            // vanilla: centre horizontally, a bit above the bottom toolbar
            int w = TextureAssets.TextBack.Width();
            int h = TextureAssets.TextBack.Height();
            int x = (int)(30 + ChatHook.OffsetX);
            int y = (int)(700 + ChatHook.OffsetY);
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle HotbarBounds()
        {
            int slot = (int)(52f * Main.inventoryScale);    // vanilla slot size
            int w = slot * 20-180;
            int h = slot*2+10;
            int x = (int)(20 + HotbarHook.OffsetX);
            int y = (int) (HotbarHook.OffsetY);
            return new Rectangle(x, y, w, h);
        }
    }
}