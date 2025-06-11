using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using UICustomizer.Common.Configs;
using UICustomizer.Common.Systems;

namespace UICustomizer.UI
{
    public class DraggablePanel : UIPanel
    {
        // Dragging
        public Vector2 offset;
        public bool dragging { get; private set; }

        // Called by children to stop a drag
        public void CancelDrag() => dragging = false;

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);

            // If the mouse‐target (or any of its parents) is a Scrollbar, do NOT start a panel‐drag.
            UIElement target = evt.Target;
            while (target != null)
            {
                if (target is Scrollbar || target is CloseButton || target is CheckboxElement ||
                    target is CheckboxEyeElement || target is Button)
                    return;
                target = target.Parent;
            }

            if (ContainsPoint(evt.MousePosition))
            {
                DragStart(evt);
            }
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            if (dragging)
            {
                DragEnd(evt);
            }
        }

        private void DragStart(UIMouseEvent evt)
        {
            offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
            dragging = true;
        }

        private void DragEnd(UIMouseEvent evt)
        {
            Vector2 endMousePosition = evt.MousePosition;
            dragging = false;
            //Left.Set(endMousePosition.X - offset.X, 0f);
            //Top.Set(endMousePosition.Y - offset.Y, 0f);
            //Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            //if (!EditorSystem.IsActive || !LayerSystem.IsActive) return;

            base.Update(gameTime);

            if (ContainsPoint(Main.MouseScreen))
            {
                if (Conf.C.DisableItemUseWhileDragging)
                {
                    Main.LocalPlayer.mouseInterface = true;
                }
            }

            if (ZenSlider.IsAnySliderHeld) return;

            if (dragging)
            {
                float rawX = Main.mouseX - offset.X;
                float rawY = Main.mouseY - offset.Y;

                var dims = GetDimensions();
                float panelW = dims.Width;
                float panelH = dims.Height;

                float clampedX = Math.Clamp(rawX, 0, Main.screenWidth - panelW);
                float clampedY = Math.Clamp(rawY, 0, Main.screenHeight - panelH);

                // 4) Apply!
                Left.Set(rawX, 0f);
                Top.Set(rawY, 0f);
                Recalculate();
            }

            var parentSpace = Parent.GetDimensions().ToRectangle();
            if (!GetDimensions().ToRectangle().Intersects(parentSpace))
            {
                Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
                Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
                Recalculate();
            }
        }
    }
}
