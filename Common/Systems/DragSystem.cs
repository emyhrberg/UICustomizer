using System;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Terraria.GameContent;
using UICustomizer.Common.Systems.Hooks;
using UICustomizer.Helpers.Layouts;
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

            // If edit mode is not active, do nothing
            if (!UICustomizerSystem.EditModeActive)
                return;

            // Force exit edit mode on escape
            // if (UICustomizerSystem.EditModeActive && Main.keyState.IsKeyDown(Keys.Escape))
            // {
            // UICustomizerSystem.ExitEditMode();
            // }

            // If dragging the UIEditorPanel, return
            UICustomizerSystem sys = ModContent.GetInstance<UICustomizerSystem>();
            if (sys == null || sys.state == null)
                return;

            if (sys.state.panel.dragging || sys.state.panel.resize.draggingResize)
                return;

            // Handle dragging of UI elements
            HandleDrag(MapBounds, ref MapHook.OffsetX, ref MapHook.OffsetY);
            HandleDrag(InfoAccsBounds, ref InfoAccsHook.OffsetX, ref InfoAccsHook.OffsetY);

            if (Main.drawingPlayerChat)
                HandleDrag(ChatBounds, ref ChatHook.OffsetX, ref ChatHook.OffsetY);

            // Handle inventory or hotbar dragging
            if (Main.playerInventory)
            {
                // convert to float
                HandleDrag(InventoryBounds, ref InventoryHook.OffsetX, ref InventoryHook.OffsetY);
                HandleDrag(CraftingBounds, ref CraftingHook.OffsetX, ref CraftingHook.OffsetY);
                HandleDrag(AccessoriesBounds, ref AccessoriesHook.OffsetX, ref AccessoriesHook.OffsetY);


            }
            else
            {
                HandleDrag(HotbarBounds, ref HotbarHook.OffsetX, ref HotbarHook.OffsetY);
                HandleDrag(BuffBounds, ref BuffHook.OffsetX, ref BuffHook.OffsetY);
            }

            if (Main.recBigList)
            {
                HandleDrag(CraftWindowBounds, ref CraftWindowHook.OffsetX, ref CraftWindowHook.OffsetY);
            }

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
                LayoutHelper.CurrentLayoutName = "Active";
                sys?.state?.panel?.editorTab?.Populate();

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
                    LayoutHelper.SaveActiveLayout();
                }
            }
        }

        #region Bounds (hardcoded...)

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
            int w = 440;
            int h = 74;
            int x = (int)(20 + HotbarHook.OffsetX);
            int y = 1 + (int)HotbarHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle BuffBounds()
        {
            int w = 440;
            int h = 36;
            if (BuffLoader.BuffCount > 11)
                h = 36 * 3;

            //Log.ChatSlow(BuffLoader.BuffCount.ToString());

            int x = (int)(20 + BuffHook.OffsetX);
            int y = (int)(52 + 21 + BuffHook.OffsetY);
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle MapBounds()
        {
            int w = (int)(255 * Main.MapScale);
            int h = (int)(255 * Main.MapScale);
            int x = (int)(Main.screenWidth - 300 + MapHook.OffsetX);
            int y = 85 + (int)MapHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle InfoAccsBounds()
        {
            int shown = 0;

            try
            {
                // Safe bounds checking before accessing the array
                if (InfoDisplayLoader.InfoDisplays != null &&
                    InfoDisplayLoader.InfoDisplayCount > 0 &&
                    Main.LocalPlayer?.hideInfo != null)
                {
                    for (int i = 0; i < InfoDisplayLoader.InfoDisplayCount && i < InfoDisplayLoader.InfoDisplays.Count && i < Main.LocalPlayer.hideInfo.Length; i++)
                    {
                        if (InfoDisplayLoader.InfoDisplays[i] != null &&
                            InfoDisplayLoader.InfoDisplays[i].Active() &&
                            !Main.LocalPlayer.hideInfo[i])
                        {
                            shown++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error counting info displays: {ex.Message}");
                shown = 1; // Fallback value
            }

            // Remove the debug line that's spamming chat
            // Main.NewText(shown);

            int h = 30;

            if (Main.playerInventory)
                h = 30;
            else if (shown > 0)
            {
                h = shown * 23;
            }

            int w = 255;
            int x = (int)(Main.screenWidth - 300 + InfoAccsHook.OffsetX);
            int y = 350 + (int)InfoAccsHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle ClassicLifeBounds()
        {
            int w = 255;
            int h = 75;
            int x = (int)(Main.screenWidth - 300 + ClassicLifeHook.OffsetX);
            int y = 6 + (int)ClassicLifeHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle FancyLifeBounds()
        {
            int w = 255;
            int h = 60;
            int x = (int)(Main.screenWidth - 300 + FancyLifeHook.OffsetX);
            int y = 12 + (int)FancyLifeHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle ClassicManaBounds()
        {
            int w = 44;
            int h = 300;
            int x = (int)(Main.screenWidth - 0 - w + ClassicManaHook.OffsetX);
            int y = 6 + (int)ClassicManaHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle FancyManaBounds()
        {
            int w = 40;
            int h = 250;
            int x = (int)(Main.screenWidth - 6 - w + FancyManaHook.OffsetX);
            int y = 12 + (int)FancyManaHook.OffsetY;
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

        public static Rectangle InventoryBounds()
        {
            //int slot = (int)(52f * Main.inventoryScale);    // vanilla slot size
            int w = 548;
            int h = 315;
            int x = (int)(20 + InventoryHook.OffsetX);
            int y = 1 + (int)InventoryHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle CraftingBounds()
        {
            //int slot = (int)(52f * Main.inventoryScale);    // vanilla slot size
            int w = 52;
            int h = 252 + 12;
            int x = (int)(20 + CraftingHook.OffsetX);
            int y = 500 + (int)CraftingHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle AccessoriesBounds()
        {
            int h = (int)(430);


            int w = (int)(225);
            int x = (int)(Main.screenWidth - 230 + AccessoriesHook.OffsetX);
            int y = 390 + (int)AccessoriesHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle CraftWindowBounds()
        {
            int w = (int)(180);
            int h = (int)(200);
            int x = (int)(485 - w + CraftWindowHook.OffsetX);
            int y = 330 + (int)CraftWindowHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        #endregion
    }
}