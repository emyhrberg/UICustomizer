using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using UIEditor.Core.Helpers.Layouts;
using UIEditor.Core.IngameEditor.Systems;
using UIEditor.Core.LayersEditor;
using static UIEditor.Core.Helpers.Layouts.ElementHelper;

namespace UIEditor.Core.IngameEditor.UI;
public class EditPanel : BasePanel
{
    // Tabs
    public EditTab editorTab;
    public PositionsTab positionsTab;
    public LayoutsTab layoutsTab;

    protected override Action CloseAction => () => EditSystem.SetActive(false);

    protected override (Tab, Tab, Tab) CreateTabs()
    {
        editorTab = new EditTab();
        PopulateDefaultColors();
        positionsTab = new PositionsTab();
        layoutsTab = new LayoutsTab();
        return (editorTab, positionsTab, layoutsTab);
    }

    // Store colors for each UI element
    private readonly Dictionary<Element, Color> elementColors = [];

    public void PopulateDefaultColors()
    {
        elementColors.Clear();
        foreach (Element ele in Enum.GetValues<Element>())
        {
            elementColors[ele] = ele switch
            {
                Element.Map => Color.Black,
                Element.InfoAccs => Color.Red,
                Element.Chat => Color.Blue,
                Element.Inventory => Color.Blue,
                Element.Crafting => Color.Yellow,
                Element.Accessories => Color.Magenta,
                Element.Hotbar => Color.Cyan,
                Element.Buffs => Color.Purple,
                Element.CraftingWindow => Color.OrangeRed,
                Element.ClassicLife => Color.Pink,
                Element.ClassicMana => Color.Teal,
                Element.FancyLife => Color.Lime,
                Element.FancyLifeText => Color.GreenYellow,
                Element.FancyMana => Color.SkyBlue,
                Element.HorizontalBars => Color.Gold,
                Element.BarLifeText => Color.Silver,
                Element.BarManaText => Color.Brown,
                _ => Color.Red // default to red if element isnt found
            };
        }
    }

    public void PopulateRandomColors()
    {
        elementColors.Clear();
        foreach (Element ele in Enum.GetValues<Element>())
            elementColors[ele] = new Color(Main.rand.Next(40, 256), Main.rand.Next(40, 256), Main.rand.Next(40, 256));
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);
        DrawHitboxes(sb);
    }

    private void DrawHitboxes(SpriteBatch sb)
    {
        // Draw hover around every element
        DrawHitboxOutlineAndText(sb, DragSystem.InfoAccsBounds(), Element.InfoAccs, x: -70, color: elementColors[Element.InfoAccs]);

        if (Main.drawingPlayerChat)
            DrawHitboxOutlineAndText(sb, DragSystem.ChatBounds(), Element.Chat, color: elementColors[Element.Chat]);

        // Draw hotbar or inventory
        if (Main.playerInventory)
        {
            DrawHitboxOutlineAndText(sb, DragSystem.InventoryBounds(), Element.Inventory, x: -75, color: elementColors[Element.Inventory]);
            DrawHitboxOutlineAndText(sb, DragSystem.CraftingBounds(), Element.Crafting, x: -70, color: elementColors[Element.Crafting]);
            DrawHitboxOutlineAndText(sb, DragSystem.AccessoriesBounds(), Element.Accessories, x: -90, color: elementColors[Element.Accessories]);
        }
        else
        {
            DrawHitboxOutlineAndText(sb, DragSystem.HotbarBounds(), Element.Hotbar, x: -55, color: elementColors[Element.Hotbar]);
            DrawHitboxOutlineAndText(sb, DragSystem.BuffBounds(), Element.Buffs, x: -45, color: elementColors[Element.Buffs]);
        }

        if (Main.recBigList)
            DrawHitboxOutlineAndText(sb, DragSystem.CraftingWindowBounds(), Element.CraftingWindow, x: -125, color: elementColors[Element.CraftingWindow]);

        // Draw resource bars. Check which health and mana style is active:
        string activeSetName = Main.ResourceSetsManager.ActiveSet.DisplayedName;
        if (activeSetName.StartsWith("Classic"))
        {
            DrawHitboxOutlineAndText(sb, DragSystem.ClassicLifeBounds(), Element.ClassicLife, x: -90, color: elementColors[Element.ClassicLife]);
            DrawHitboxOutlineAndText(sb, DragSystem.ClassicManaBounds(), Element.ClassicMana, x: -5, color: elementColors[Element.ClassicMana]);
        }
        else if (activeSetName == "Fancy")
        {
            DrawHitboxOutlineAndText(sb, DragSystem.FancyLifeBounds(), Element.FancyLife, x: -80, color: elementColors[Element.FancyLife]);
            DrawHitboxOutlineAndText(sb, DragSystem.FancyManaBounds(), Element.FancyMana, x: -5, color: elementColors[Element.FancyMana]);
        }
        else if (activeSetName == "Fancy 2")
        {
            DrawHitboxOutlineAndText(sb, DragSystem.FancyLifeBounds(), Element.FancyLife, x: -80, color: elementColors[Element.FancyLife]);
            DrawHitboxOutlineAndText(sb, DragSystem.FancyLifeTextBounds(), Element.FancyLifeText, x: -112, color: elementColors[Element.FancyLifeText]);
            DrawHitboxOutlineAndText(sb, DragSystem.FancyManaBounds(), Element.FancyMana, x: -5, color: elementColors[Element.FancyMana]);
        }
        else if (activeSetName == "Bars")
        {
            DrawHitboxOutlineAndText(sb, DragSystem.BarsBounds(), Element.HorizontalBars, x: -120, color: elementColors[Element.HorizontalBars]);
        }
        else if (activeSetName == "Bars 2")
        {
            DrawHitboxOutlineAndText(sb, DragSystem.BarsBounds(), Element.HorizontalBars, x: -120, color: elementColors[Element.HorizontalBars]);
            DrawHitboxOutlineAndText(sb, DragSystem.BarLifeTextBounds(), Element.BarLifeText, x: -95, color: elementColors[Element.BarLifeText]);
        }
        else if (activeSetName == "Bars 3")
        {
            DrawHitboxOutlineAndText(sb, DragSystem.BarsBounds(), Element.HorizontalBars, x: -120, color: elementColors[Element.HorizontalBars]);
            DrawHitboxOutlineAndText(sb, DragSystem.BarLifeTextBounds(), Element.BarLifeText, x: -95, color: elementColors[Element.BarLifeText]);
            DrawHitboxOutlineAndText(sb, DragSystem.BarManaTextBounds(), Element.BarManaText, x: -110, color: elementColors[Element.BarManaText]);
        }

        DrawHitboxOutlineAndText(sb, DragSystem.MapBounds(), Element.Map, x: -40, color: elementColors[Element.Map]);
    }

    public static void DrawHitboxOutlineAndText(SpriteBatch sb, Rectangle rect, Element element, int x = 0, int y = 0, Color color = default)
    {
        if (color == default)
            color = Color.Red;

        var sys = ModContent.GetInstance<EditSystem>();
        if (sys == null || sys.state == null || sys.state.editorPanel == null) return;

        // Draw hitboxes
        if (EditorTabSettings.ShowHitboxes)
        {
            const float fillScaleX = 0.985f;
            const float fillScaleY = 0.97f;

            int padX = (int)((1f - fillScaleX) * rect.Width / 2f); // will be 0
            int padY = (int)((1f - fillScaleY) * rect.Height / 2f);

            var fillRect = new Rectangle(
                rect.X + padX,
                rect.Y + padY,
                (int)(rect.Width * fillScaleX),
                (int)(rect.Height * fillScaleY)
            );

            sb.Draw(TextureAssets.MagicPixel.Value, fillRect, color * EditorTabSettings.Opacity);

            // Draw outline around the full-size rect
            DrawSlices(sb, rect, color, fill: false, fillOpacity: 0f);
        }

        // Draw eye toggle
        // Try to get the interface layer name from the mapping
        if (!ElementHelper.ElementInterfaceLayerMapping.TryGetValue(element, out string interfaceLayerName))
        {
            // Still draw the name if ShowNames is enabled, using the enum's string representation
            if (EditorTabSettings.ShowNames)
            {
                Vector2 pos = rect.Location.ToVector2();
                Utils.DrawBorderString(sb, element.ToString(), pos, Color.White);
            }
            return; // Exit early for this element if no mapping for eye toggle
        }

        // Draw eye toggle (only if mapping was found)
        if (EditorTabSettings.ShowEyeToggle)
        {
            if (LayerSystem.LayerStates == null)
            {
                return;
            }

            bool isCurrentlyVisible = LayerSystem.LayerStates.TryGetValue(interfaceLayerName, out bool currentState) ? currentState : true;
            Rectangle eyeRect = new(rect.X - Ass.EyeOpen.Width(), rect.Y, Ass.EyeOpen.Width(), Ass.EyeOpen.Height());

            if (eyeRect.Contains(Main.mouseX, Main.mouseY))
            {
                // choose hover sprite depending on current visibility
                Texture2D hoverTex = (isCurrentlyVisible ? Ass.EyeOpenHover.Value : Ass.EyeClosedHover.Value);
                sb.Draw(hoverTex, eyeRect, Color.White);

                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    isCurrentlyVisible = !isCurrentlyVisible;
                    LayerSystem.LayerStates[interfaceLayerName] = isCurrentlyVisible;
                    Main.mouseLeftRelease = false;
                }
            }
            else
            {
                sb.Draw(isCurrentlyVisible ? Ass.EyeOpen.Value : Ass.EyeClosed.Value, eyeRect, Color.White);
            }

        }

        // Draw names of the UI elements
        // Use interfaceLayerName if available and ShowNames is true, otherwise use element.ToString()
        if (EditorTabSettings.ShowNames)
        {
            Vector2 pos = rect.Location.ToVector2();
            // Display the mapped interfaceLayerName if available, otherwise the enum name.
            string displayName = !string.IsNullOrEmpty(interfaceLayerName) ? interfaceLayerName : element.ToString();
            if (EditorTabSettings.ShowEyeToggle) displayName = interfaceLayerName;
            else displayName = element.ToString();
            Utils.DrawBorderString(sb, displayName, pos, Color.White);
        }
    }

    /// <summary>
    /// A 30x30 pixel made in photoshop
    /// We grab the 5x5 corner edges to create a rounded edges look.
    /// </summary>
    private static void DrawSlices(SpriteBatch sb, Rectangle t, Color col, bool fill = true, float fillOpacity = 0.3f)
    {
        var tex = Ass.Hitbox.Value;
        int c = EditorTabSettings.Stroke;                         // 5-px corners / edge thickness
        Rectangle sc = new(0, 0, c, c),
                  eh = new(c, 0, 30 - 2 * c, c),
                  ev = new(0, c, c, 30 - 2 * c),
                  ce = new(c, c, 30 - 2 * c, 30 - 2 * c);

        if (fill)
            sb.Draw(tex, new Rectangle(t.X + c, t.Y + c, t.Width - 2 * c, t.Height - 2 * c), ce, col * fillOpacity);

        sb.Draw(tex, new Rectangle(t.X + c, t.Y, t.Width - 2 * c, c), eh, col);                                       // top
        sb.Draw(tex, new Rectangle(t.X + c, t.Bottom - c, t.Width - 2 * c, c), eh, col, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0); // bottom
        sb.Draw(tex, new Rectangle(t.X, t.Y + c, c, t.Height - 2 * c), ev, col);                                       // left
        sb.Draw(tex, new Rectangle(t.Right - c, t.Y + c, c, t.Height - 2 * c), ev, col, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0); // right

        sb.Draw(tex, new Rectangle(t.X, t.Y, c, c), sc, col);                                                          // TL
        sb.Draw(tex, new Rectangle(t.Right - c, t.Y, c, c), sc, col, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0); // TR
        sb.Draw(tex, new Rectangle(t.Right - c, t.Bottom - c, c, c), sc, col, 0, Vector2.Zero, SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally, 0); // BR
        sb.Draw(tex, new Rectangle(t.X, t.Bottom - c, c, c), sc, col, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0); // BL
    }
}
