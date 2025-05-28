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

            // Hot reload testing
            //ChatHook.OffsetX = 200;

            HandleDrag(DragHelper.ChatBounds, ref ChatHook.OffsetX, ref ChatHook.OffsetY);
            HandleDrag(DragHelper.HotbarBounds, ref HotbarHook.OffsetX, ref HotbarHook.OffsetY);
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

        private void HandleDrag(Func<Rectangle> bounds, ref float offsetX, ref float offsetY)
        {
            float uiScale = Main.UIScale;
            Vector2 mouseUI = Main.MouseScreen / 1;   // convert once

            /* start drag */
            if (_dragSource is null && Main.mouseLeft && DragHelper.MouseInBounds(bounds()))
            {
                _dragSource = bounds;
                _mouseStart = mouseUI;                      // store in UI units
                _offsetStart = new Vector2(offsetX, offsetY);
                Main.LocalPlayer.mouseInterface = true;
            }

            /* update drag */
            if (_dragSource == bounds)
            {
                Vector2 deltaUI = mouseUI - _mouseStart;     // already de-scaled
                offsetX = _offsetStart.X + deltaUI.X;
                offsetY = _offsetStart.Y + deltaUI.Y;

                if (!Main.mouseLeft)
                    _dragSource = null;
            }
        }
    }
}