using System;
using System.Linq;
using Iced.Intel;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using UICustomizer.Common.Configs;
using UICustomizer.Common.Systems.Hooks;
using UICustomizer.Helpers;
using UICustomizer.Helpers.Layouts;
using UICustomizer.UI;

namespace UICustomizer.Common.Systems
{
    public class DragSystem : ModSystem
    {
        private Vector2 _mouseStart;
        private Vector2 _offsetStart;
        private Func<Rectangle> _dragSource;   // null = no drag in progress

        // Send text timer
        private static DateTime lastWarningSent = DateTime.UtcNow;

        //  Update logic. Runs every frame before drawing happens
        public override void PostUpdateInput()
        {
            base.PostUpdateInput();

            if (!EditorSystem.IsActive) return;

            EditorSystem sys = ModContent.GetInstance<EditorSystem>();
            if (sys == null || sys.state == null) return;

            // If dragging the UIEditorPanel, dont drag anything else
            // if (sys.state.editorPanel.dragging || sys.state.editorPanel.resize.draggingResize)
            // return;

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

            if (Main.recBigList) // recipe big list is showing (a.k.a. crafting window)
                HandleDrag(CraftingWindowBounds, ref CraftWindowHook.OffsetX, ref CraftWindowHook.OffsetY);

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
                HandleDrag(FancyLifeTextBounds, ref FancyLifeTextHook.OffsetX, ref FancyLifeTextHook.OffsetY);
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
            Vector2 mouseUI = Main.MouseScreen / Main.UIScale;
            Rectangle boundsRect = bounds();

            /* start drag */
            if (_dragSource is null && Main.mouseLeft && boundsRect.Contains(mouseUI.ToPoint()))
            {
                Log.Info($"Dragging element at {mouseUI} with bounds {boundsRect}");
                // CHECK EDIT MODE FIRST - before any drag setup
                if (!EditorSystem.IsEditing)
                {
                    bool someTimeElapsed = DateTime.UtcNow - lastWarningSent >= TimeSpan.FromMilliseconds(500);
                    if (someTimeElapsed)
                    {
                        lastWarningSent = DateTime.UtcNow;

                            if (!Conf.C.ShowCombatTextTooltips) return;
                            CombatText.NewText(Main.LocalPlayer.getRect(), Color.Red, "Please enter edit mode to drag the element.");
                        }
                        return; // Exit early - don't set up drag
                    }
                    _dragSource = bounds;
                    _mouseStart = mouseUI;                      // store in UI units
                    _offsetStart = new Vector2(offsetX, offsetY);
                    if (Conf.C.DisableItemUseWhileDragging)
                    {
                        Main.LocalPlayer.mouseInterface = true;
                    }

                    // Force switch to active layout
                    LayoutHelper.CurrentLayoutName = "Active";
                    var sys = ModContent.GetInstance<EditorSystem>();
                    sys.state.editorPanel.layoutsTab.Populate();
                }

            /* update drag (new offset for the element by modifying its offset using ref) */
            if (_dragSource == bounds)
            {
                Vector2 deltaUI = mouseUI - _mouseStart;

                offsetX = _offsetStart.X + deltaUI.X;
                offsetY = _offsetStart.Y + deltaUI.Y;

                //if (UIEditorSettings.ClampToScreen)
                //{
                //    ClampToScreen(ref offsetX, ref offsetY, bounds);
                //}
                // only snap if the user has Snap enabled
                if (EditorTabSettings.SnapToEdges)
                {
                    SnapToEdges(ref offsetX, ref offsetY, bounds, threshold: EditorTabSettings.SnapThreshold);
                }

                if (!Main.mouseLeft)
                {
                    // End drag
                    _dragSource = null;
                    LayoutHelper.SaveActiveLayout();
                }
            }
        }

        #region Drag Helpers

        [Obsolete("Use SnapToEdges instead. The new method has a threshold parameter and is more flexible.")]
        private void ClampToScreen(ref float offsetX, ref float offsetY, Func<Rectangle> bounds)
        {
            var r = bounds();
            if (r.Left < 0) offsetX -= r.Left;
            else if (r.Right > Main.screenWidth) offsetX -= (r.Right - Main.screenWidth);
            if (r.Top < 0) offsetY -= r.Top;
            else if (r.Bottom > Main.screenHeight) offsetY -= (r.Bottom - Main.screenHeight);
        }

        private static void SnapToEdges(ref float offsetX, ref float offsetY, Func<Rectangle> bounds, int threshold)
        {
            var r = bounds();
            // horizontal snap
            if (Math.Abs(r.Left) <= threshold)
                offsetX -= r.Left;
            else if (Math.Abs(r.Right - Main.screenWidth) <= threshold)
                offsetX -= (r.Right - Main.screenWidth);

            // vertical snap
            if (Math.Abs(r.Top) <= threshold)
                offsetY -= r.Top;
            else if (Math.Abs(r.Bottom - Main.screenHeight) <= threshold)
                offsetY -= (r.Bottom - Main.screenHeight);
        }

        #endregion

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
                w += 801;
            }
            int h = TextureAssets.TextBack.Height();
            int x = (int)(78 + ChatHook.OffsetX);
            int y = (int)(Main.screenHeight - 36 + ChatHook.OffsetY);
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle HotbarBounds()
        {
            int w = 440;               // vanilla 10×44-slot bar
            int h = 76;
            int x = (int)(20) + (int)HotbarHook.OffsetX; // 20-px edge gap /u
            int y = (int)(-3) + (int)HotbarHook.OffsetY;

            return new Rectangle(x, y, w, h);
        }

        public static Rectangle BuffBounds()
        {
            int w = 440;
            int h = 55;

            // Set width
            if (EditorTabSettings.FitBounds)
            {
                int c = 0; // buff count active
                foreach (var b in Main.LocalPlayer.buffType)
                {
                    if (b != 0) // 0 means empty buff slot, not 0 means no buff is there => count it
                    {
                        c++;
                    }
                }



                if (c == 1) w = 55;
                else if (c == 2) w = 95;
                else if (c > 1)
                {
                    w = 41 * c + 7;
                    if (w > 39 * 11) w = 39 * 11 + 7;
                }

                if (c >= 11)
                    h = 36 * 3; // double rows
                if (c >= 22)
                    h = 36 * 4 + 10;
            }


            int x = (int)(20 + BuffHook.OffsetX);
            int y = (int)(52 + 21 + BuffHook.OffsetY);
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle MapBounds()
        {
            float s = Main.MapScale;

            int w = (int)(258 * s);
            int h = (int)(265 * s);
            int x = (int)(Main.screenWidth / Main.UIScale) - 300 + (int)(MapHook.OffsetX);
            int y = 80 + (int)(MapHook.OffsetY);
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle InfoAccsBounds()
        {
            int h = 35;

            if (EditorTabSettings.FitBounds)
            {
                int shown = 0;

                // WRITE EXCEPTION??
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


                if (Main.playerInventory)
                    h = 35;
                else if (shown > 1)
                {
                    h = shown * 26;
                }
            }

            int w = 255;
            int x = (int)(Main.screenWidth - 300 + InfoAccsHook.OffsetX);
            int y = 347 + (int)InfoAccsHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle ClassicLifeBounds()
        {
            int w = 263;
            int h = 78;
            int x = (int)(Main.screenWidth - 305 + ClassicLifeHook.OffsetX);
            int y = 4 + (int)ClassicLifeHook.OffsetY;
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

        public static Rectangle FancyLifeBounds()
        {
            int w = 255;
            int h = 78;
            int x = (int)(Main.screenWidth - 300 + FancyLifeHook.OffsetX);
            int y = 4 + (int)FancyLifeHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle FancyLifeTextBounds()
        {
            int w = 120;
            int h = 30;
            int x = (int)(Main.screenWidth - 230 + FancyLifeTextHook.OffsetX);
            int y = -4 + (int)FancyLifeTextHook.OffsetY;
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
            int w = 280;
            int h = 80;
            int x = (int)(Main.screenWidth - 310 + HorizontalBarsHook.OffsetX);
            int y = 6 + (int)HorizontalBarsHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle BarLifeTextBounds()
        {
            int w = 120;
            int h = 30;
            int x = (int)(Main.screenWidth - 235 + BarLifeTextHook.OffsetX);
            int y = -4 + (int)BarLifeTextHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle BarManaTextBounds()
        {
            int w = 135;
            int h = 30;
            int x = (int)(Main.screenWidth - 248 + BarManaTextHook.OffsetX);
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
            int h = 100;
            int w = 125; // default width
            int y = (int)(570 + CraftingHook.OffsetY);

            if (EditorTabSettings.FitBounds)
            {
                // Variables
                int heightCount = Main.numAvailableRecipes;
                int currLine = Main.focusRecipe;
                int bot = heightCount - 1 - currLine; // distance from currLine to bottom line

                // --- Height ---
                if (heightCount <= 3)
                {
                    h += 80;
                    y -= 120 / 2;
                    if (currLine == 0)
                    {
                        y += 60;
                    }
                    if (bot == 0)
                    {

                    }
                }
                else if (heightCount > 3)
                {
                    // max height
                    h += 570;
                    y -= 255;

                    if (currLine == 0)
                    {
                        y += 240;
                        h -= 240;
                    }
                    if (bot == 0)
                    {
                        h -= 240;
                    }
                }

                // --- Width ---

                int recipeIndex = Main.availableRecipe[currLine];  // index in Main.recipe[]
                Recipe recipe = Main.recipe[recipeIndex];

                // WRITE ACCESS CRASH HERE?!
                int widthCount = recipe.requiredItem.Count(item => !item.IsAir); // unique items required to craft

                if (widthCount > 1) w += 10 * widthCount;
            }

            int x = (int)(20 + CraftingHook.OffsetX);
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle AccessoriesBounds()
        {
            int h = (int)(428);
            int w = (int)(225);
            int x = (int)(Main.screenWidth - 230 + AccessoriesHook.OffsetX);
            int y = 390 + (int)AccessoriesHook.OffsetY;

            if (EditorTabSettings.FitBounds)
            {
                int count = Main.LocalPlayer.GetAmountOfExtraAccessorySlotsToShow();

                h += 43 * count;
                h += 3;

                if (Main.EquipPage == 1) //page with NPCs
                {
                    h += 46 * 4;
                    h += 3;
                }
                else if (Main.EquipPage == 2) //page with equipment, hooks, pets, etc
                {
                    h -= 180;

                    w -= 50;
                    x += 50;
                }
            }

            return new Rectangle(x, y, w, h);
        }

        public static Rectangle CraftingWindowBounds()
        {
            int w = (int)(45);
            int h = (int)(60);

            if (EditorTabSettings.FitBounds)
            {
                // --- Width ---
                int widthCount = Main.numAvailableRecipes;
                if (widthCount > 31) widthCount = 31;
                w += widthCount * 40;

                // --- Height ---
                int extraH = 31;
                int count = Main.numAvailableRecipes;
                if (count > extraH) h += 45;
                if (count > extraH * 2) h += 45;
                if (count > extraH * 3) h += 45;
                if (count > extraH * 4) h += 45;
                if (count > extraH * 5) h += 45;
                if (count > extraH * 6) h += 45;
            }


            int x = (int)(295 + CraftWindowHook.OffsetX);
            int y = 330 + (int)CraftWindowHook.OffsetY;
            return new Rectangle(x, y, w, h);
        }

        #endregion
    }
}