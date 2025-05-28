using System.Collections.Generic;
using UICustomizer.Common.Systems.Hooks;

namespace UICustomizer.Common.Systems
{
    public class DragSystem : ModSystem
    {
        private bool _dragging;
        private Vector2 _mouseStart;
        private Vector2 _offsetStart;

        //  Update logic. Runs every frame before drawing happens
        public override void PostUpdateInput()
        {
            // If edit mode is not active, do nothing
            //if (!UICustomizerSystem.EditModeActive)
                //return;

            ChatDrag();
        }

        private void ChatDrag()
        {
            Vector2 mouse = Main.MouseScreen;

            // Log status of mouseleft, release, and dragging and chatbounds.contains
            Log.SlowInfo($"Dragging: {_dragging}, ChatBounds.Contains: {ChatBounds().Contains(mouse.ToPoint())}");

            // 1) Start dragging when the player presses LMB inside the chat box
            if (!_dragging && Main.mouseLeft && ChatBounds().Contains(mouse.ToPoint()))
            {
                _dragging = true;
                _mouseStart = mouse;
                _offsetStart = new Vector2(ChatPosHook.OffsetX, ChatPosHook.OffsetY);

                // Stop items from being used while we are holding the chat
                Main.LocalPlayer.mouseInterface = true;
            }

            // 2) While dragging, update the offsets every frame
            if (_dragging)
            {
                Vector2 delta = mouse - _mouseStart;
                ChatPosHook.OffsetX = _offsetStart.X + delta.X;
                ChatPosHook.OffsetY = _offsetStart.Y + delta.Y;

                // Release as soon as the button comes up
                if (!Main.mouseLeft)
                    _dragging = false;
            }

            // 3) Clamp so the whole box stays on‑screen
            int pad = 20;                              // keep 20‑px margin
            ChatPosHook.OffsetX = MathHelper.Clamp(ChatPosHook.OffsetX,
                                                   -Main.screenWidth + pad,
                                                    Main.screenWidth - pad);
            ChatPosHook.OffsetY = MathHelper.Clamp(ChatPosHook.OffsetY,
                                                   -Main.screenHeight + pad,
                                                    0);               // never go below bottom
        }

        private static Rectangle ChatBounds()
        {
            int width = (int)(Main.screenWidth * 0.60f);   // vanilla chat ≈ 60 % width
            int height = 38;                                // input line height
            int x = 20 + (int)ChatPosHook.OffsetX;          // left padding + offset
            int y = Main.screenHeight - height - 28         // 28 px vanilla margin
                    + (int)ChatPosHook.OffsetY;
            return new Rectangle(x, y, width, height);
        }
    }
}