using System;
using Microsoft.Xna.Framework.Input;
using Terraria.GameContent;
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
            HandleDrag(ChatBounds, ref ChatHook.OffsetX, ref ChatHook.OffsetY);
            HandleDrag(HotbarBounds, ref HotbarHook.OffsetX, ref HotbarHook.OffsetY);
            HandleDrag(BuffBounds, ref BuffHook.OffsetX, ref BuffHook.OffsetY);
            HandleDrag(MapBounds, ref MapHook.OffsetX, ref MapHook.OffsetY);
            HandleDrag(InfoAccsBounds, ref InfoAccsHook.OffsetX, ref InfoAccsHook.OffsetY);

            // Resource bars
            // Check which resource set is active and handle dragging accordingly
            string activeSetName = Main.ResourceSetsManager.ActiveSet.DisplayedName;
            if (activeSetName.StartsWith("Classic"))
            {
                HandleDrag(ClassicLifeBounds, ref ClassicLifeHook.OffsetX, ref ClassicLifeHook.OffsetY);
                HandleDrag(ClassicManaBounds, ref ClassicManaHook.OffsetX, ref ClassicManaHook.OffsetY);
            }
            else if (activeSetName == "Fancy")
            {
                HandleDrag(FancyLifeBounds, ref FancyLifeHook.OffsetX, ref FancyLifeHook.OffsetY);
                HandleDrag(FancyManaBounds, ref FancyManaHook.OffsetX, ref FancyManaHook.OffsetY);
            }
            else if (activeSetName == "Fancy 2")
            {
                HandleDrag(FancyLifeBounds, ref FancyLifeHook.OffsetX, ref FancyLifeHook.OffsetY);
                HandleDrag(FancyManaBounds, ref FancyManaHook.OffsetX, ref FancyManaHook.OffsetY);
            }
            else if (activeSetName == "Bars")
            {
                HandleDrag(BarsBounds, ref HorizontalBarsHook.OffsetX, ref HorizontalBarsHook.OffsetY);
            }
            else if (activeSetName == "Bars 2")
            {
                HandleDrag(BarsBounds, ref HorizontalBarsHook.OffsetX, ref HorizontalBarsHook.OffsetY);
                HandleDrag(BarLifeTextBounds, ref BarLifeTextHook.OffsetX, ref BarLifeTextHook.OffsetY);
            }
            else if (activeSetName == "Bars 3")
            {
                HandleDrag(BarsBounds, ref HorizontalBarsHook.OffsetX, ref HorizontalBarsHook.OffsetY);
                HandleDrag(BarLifeTextBounds, ref BarLifeTextHook.OffsetX, ref BarLifeTextHook.OffsetY);
                HandleDrag(BarManaTextBounds, ref BarManaTextHook.OffsetX, ref BarManaTextHook.OffsetY);
            }
        }

        private void HandleDrag(Func<Rectangle> bounds, ref float offsetX, ref float offsetY)
        {
            UICustomizerSystem sys = ModContent.GetInstance<UICustomizerSystem>();
            Checkbox checkboxX = sys.state.panel.editorTab.CheckboxX;
            Checkbox checkboxY = sys.state.panel.editorTab.CheckboxY;

            Vector2 mouseUI = Main.MouseScreen;

            /* start drag */
            if (_dragSource is null && Main.mouseLeft && bounds().Contains(Main.MouseScreen.ToPoint()))
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

                // Force switch to active layout
                LayoutJsonHelper.CurrentLayoutName = "Active";
                sys?.state?.panel?.editorTab?.PopulatePublic();

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
                {
                    // End drag
                    _dragSource = null;
                    LayoutJsonHelper.SaveActiveLayout("Active");
                }
            }
        }

        #region Bounds

        public static Rectangle ChatBounds()
        {
            // vanilla: centre horizontally, a bit above the bottom toolbar
            int w = TextureAssets.TextBack.Width() + 120; // not accurate, its much wider in fullscreen
            //Log.SlowInfo(Main.screenWidth.ToString());
            if (Main.screenWidth > 1000)
            {
                w += 200;
            }
            if (Main.screenWidth > 1800)
            {
                w += 727;
            }
            int h = TextureAssets.TextBack.Height();
            int x = (int)(78 + ChatHook.OffsetX);
            int y = (int)(Main.screenHeight - 86 + ChatHook.OffsetY);
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle HotbarBounds()
        {
            //int slot = (int)(52f * Main.inventoryScale);    // vanilla slot size
            int w = 52 * 10 - 85;
            int h = 52 + 12;
            int x = (int)(20 + HotbarHook.OffsetX);
            int y = 1 + (int)HotbarHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle BuffBounds()
        {
            int w = 52 * 10 - 85;
            int h = 52 + 12;
            int x = (int)(20 + BuffHook.OffsetX);
            int y = (int)(52 + 21 + BuffHook.OffsetY);
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle MapBounds()
        {
            int w = (int)(250 * Main.MapScale);
            int h = (int)(250 * Main.MapScale);
            int x = (int)(Main.screenWidth - 50 - w + MapHook.OffsetX);
            int y = 90 + (int)MapHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle InfoAccsBounds()
        {
            int w = 250;
            int h = 100;
            int x = (int)(Main.screenWidth - 50 - w + InfoAccsHook.OffsetX);
            int y = 340 + (int)InfoAccsHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle ClassicLifeBounds()
        {
            int w = 245;
            int h = 65;
            int x = (int)(Main.screenWidth - 57 - w + ClassicLifeHook.OffsetX);
            int y = 6 + (int)ClassicLifeHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle FancyLifeBounds()
        {
            int w = 250;
            int h = 60;
            int x = (int)(Main.screenWidth - 50 - w + FancyLifeHook.OffsetX);
            int y = 12 + (int)FancyLifeHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle ClassicManaBounds()
        {
            int w = 44;
            int h = 300;
            int x = (int)(Main.screenWidth - 6 - w + ClassicManaHook.OffsetX);
            int y = 6 + (int)ClassicManaHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle FancyManaBounds()
        {
            int w = 40;
            int h = 300;
            int x = (int)(Main.screenWidth - 6 - w + FancyManaHook.OffsetX);
            int y = 6 + (int)FancyManaHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle BarsBounds()
        {
            int w = 250;
            int h = 80;
            int x = (int)(Main.screenWidth - 50 - w + HorizontalBarsHook.OffsetX);
            int y = 12 + (int)HorizontalBarsHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle BarLifeTextBounds()
        {
            int w = 110;
            int h = 30;
            int x = (int)(Main.screenWidth - 118 - w + BarLifeTextHook.OffsetX);
            int y = 1 + (int)BarLifeTextHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle BarManaTextBounds()
        {
            int w = 110;
            int h = 30;
            int x = (int)(Main.screenWidth - 123 - w + BarManaTextHook.OffsetX);
            int y = 60 + (int)BarManaTextHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        #endregion
    }
}