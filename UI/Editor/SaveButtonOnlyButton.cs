using System;
using Terraria;
using UICustomizer.Common.Systems;

namespace UICustomizer.UI.Editor
{
    public class SaveButtonOnlyButton : Button
    {
        private bool _isPotentialDrag;
        private bool _isDragging;
        private Vector2 _mouseDownPos;
        private Vector2 _dragOffset;
        private const float DragThreshold = 10f;

        public SaveButtonOnlyButton(string text, Action onClick, Func<string> tooltip = null, Action onRightClick = null, bool maxWidth = false, int width = 100, int height = 30)
        : base(text, onClick, tooltip, onRightClick, maxWidth, width, height)
        {
            OnLeftMouseDown += (evt, _) =>
            {
                _isPotentialDrag = true;
                // store the absolute screen position, not the UI-relative one
                _mouseDownPos = Main.MouseScreen;
            };
            OnLeftMouseUp += (evt, _) =>
            {
                _isPotentialDrag = false;
                _isDragging = false;
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // if we've clicked but not yet started dragging, see if we've moved far enough
            if (_isPotentialDrag && !_isDragging)
            {
                Vector2 current = Main.MouseScreen;
                if (Vector2.DistanceSquared(current, _mouseDownPos) > DragThreshold * DragThreshold)
                {
                    _isDragging = true;
                    var dims = GetDimensions();
                    _dragOffset = _mouseDownPos - new Vector2(dims.X, dims.Y);
                }
            }

            // while dragging, reposition the element
            if (_isDragging)
            {
                Vector2 current = Main.MouseScreen;
                Left.Set(current.X - _dragOffset.X, 0f);
                Top.Set(current.Y - _dragOffset.Y, 0f);
                Recalculate();
            }
        }
    }
}
