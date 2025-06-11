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
            if (!EditorSystem.IsActive || !LayerSystem.IsActive) return;

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
                //var p = Parent.GetDimensions().ToRectangle();
                //Log.Info(p.Height.ToString());

                // if stuck, cancel drag with RMB.
                if (Main.mouseRight)
                {
                    dragging = false;
                    return;
                }

                // Clamp left to Main.screenwidth
                float x = Main.mouseX - offset.X;
                float clampX = Math.Min(x, 0);
                Left.Set(x, 0);

                // Clamp top to Main.screenheight
                float y = Main.mouseY - offset.Y;
                float clampY = Math.Max(y, Main.screenHeight);
                Top.Set(y, 0);

                Recalculate();
            }

            //var parentSpace = Parent.GetDimensions().ToRectangle();
            //if (!GetDimensions().ToRectangle().Intersects(parentSpace))
            //{
            //    Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
            //    Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
            //    Recalculate();
            //}
        }
    }
}
