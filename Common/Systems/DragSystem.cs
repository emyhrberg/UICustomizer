using System;
using Microsoft.Xna.Framework.Input;
using UICustomizer.Common.Systems.Hooks;
using UICustomizer.UI;

namespace UICustomizer.Common.Systems
{
    public class DragSystem : ModSystem
    {
        private Vector2 _mouseStart;
        private Vector2 _offsetStart;
        private Func<Rectangle>? _dragSource;   // null = no drag in progress

        // Send text timer
        private static DateTime lastWarningSent = DateTime.UtcNow;

        //  Update logic. Runs every frame before drawing happens
        public override void PostUpdateInput()
        {
            base.PostUpdateInput();

            // --- HOT RELOAD TESTING ---
            //ChatHook.OffsetY = -200;
            //MapHook.OffsetX = -100;
            //LifeAndManaHook.OffsetY = 100;

            // If edit mode is not active, do nothing
            if (!UICustomizerSystem.EditModeActive)
                return;


            // Force open chat
            //    if (!Main.drawingPlayerChat)               
            //        Main.OpenPlayerChat();             
            //}
            //else if (Main.drawingPlayerChat)               
            //    Main.ClosePlayerChat();

            // Force close inventory
            if (UICustomizerSystem.EditModeActive && Main.playerInventory)
            {
                //CombatText.NewText(Main.LocalPlayer.getRect(), Color.Red, "Inventory is not available in UI edit mode.");
                Main.playerInventory = false;              // close inventory
                Main.mouseItem = new Item();               // clear mouse item
            }

            // Force exit edit mode on escape
            if (UICustomizerSystem.EditModeActive && Main.keyState.IsKeyDown(Keys.Escape))
            {
                UICustomizerSystem.ExitEditMode();
            }

            // If dragging the UIEditorPanel, return
            UICustomizerSystem sys = ModContent.GetInstance<UICustomizerSystem>();
            if (sys == null || sys.state == null)
                return;

            if (sys.state.panel.dragging || sys.state.panel.resize.draggingResize)
                return;

            // Handle dragging of UI elements
            HandleDrag(DragHelper.ChatBounds, ref ChatHook.OffsetX, ref ChatHook.OffsetY);
            HandleDrag(DragHelper.HotbarBounds, ref HotbarHook.OffsetX, ref HotbarHook.OffsetY);
            HandleDrag(DragHelper.MapBounds, ref MapHook.OffsetX, ref MapHook.OffsetY);
            HandleDrag(DragHelper.InfoAccsBounds, ref InfoAccsHook.OffsetX, ref InfoAccsHook.OffsetY);

            // Resource bars
            HandleDrag(DragHelper.ClassicLifeBounds, ref ClassicLifeHook.OffsetX, ref ClassicLifeHook.OffsetY);
            HandleDrag(DragHelper.FancyLifeBounds, ref FancyLifeHook.OffsetX, ref FancyLifeHook.OffsetY);
            HandleDrag(DragHelper.ClassicManaBounds, ref ClassicManaHook.OffsetX, ref ClassicManaHook.OffsetY);
            HandleDrag(DragHelper.FancyManaBounds, ref FancyManaHook.OffsetX, ref FancyManaHook.OffsetY);
            HandleDrag(DragHelper.BarsBounds, ref HorizontalLifeBarHook.OffsetX, ref HorizontalLifeBarHook.OffsetY);
        }

        private void HandleDrag(Func<Rectangle> bounds, ref float offsetX, ref float offsetY)
        {
            UICustomizerSystem sys = ModContent.GetInstance<UICustomizerSystem>();
            Checkbox checkboxX = sys.state.panel.editorTab.CheckboxX;
            Checkbox checkboxY = sys.state.panel.editorTab.CheckboxY;

            Vector2 mouseUI = Main.MouseScreen;

            /* start drag */
            if (_dragSource is null && Main.mouseLeft && DragHelper.MouseInBounds(bounds()))
            {
                // If neither x nor y is checked, print a warning and return
                bool someTimeElapsed = DateTime.UtcNow - lastWarningSent >= TimeSpan.FromMilliseconds(50);

                if (UICustomizerSystem.EditModeActive &&
                    checkboxX.state == CheckboxState.Unchecked &&
                    checkboxY.state == CheckboxState.Unchecked &&
                    someTimeElapsed)
                {
                    string warnMsg = "Please check any of the X or Y checkboxes to drag the UI element.";
                    CombatText.NewText(Main.LocalPlayer.getRect(), Color.Red, warnMsg);

                    lastWarningSent = DateTime.UtcNow;
                    return;
                }

                _dragSource = bounds;
                _mouseStart = mouseUI;                      // store in UI units
                _offsetStart = new Vector2(offsetX, offsetY);
                Main.LocalPlayer.mouseInterface = true;
            }

            /* update drag (new offset for the element by modifying its offset using ref) */
            if (_dragSource == bounds)
            {
                Vector2 deltaUI = mouseUI - _mouseStart;     // already de-scaled

                // Only drag if the x/y checkbox is checked
                if (checkboxX.state == CheckboxState.Checked)
                {
                    offsetX = _offsetStart.X + deltaUI.X;
                }

                if (checkboxY.state == CheckboxState.Checked)
                {
                    offsetY = _offsetStart.Y + deltaUI.Y;
                }

                if (!Main.mouseLeft)
                    _dragSource = null;
            }
        }
    }
}