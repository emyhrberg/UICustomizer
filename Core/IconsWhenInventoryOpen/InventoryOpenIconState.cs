using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using UIEditor.Core.IngameEditor.Systems;
using UIEditor.Core.IngameEditor.UI;
using UIEditor.Core.LayersEditor;

namespace UIEditor.Core.IconsWhenInventoryOpen;
public class InventoryOpenIconState : UIState
{
    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);

        if (!Conf.C.ShowIconsUnderInventory)
            return;

        if (!Main.playerInventory)
            return;

        // Textures
        Texture2D editorTex = Ass.EditorIconSmall.Value;
        Texture2D editorTexHover = Ass.EditorIconSmallHover.Value;
        Texture2D layersTex = Ass.LayersIconSmall.Value;
        Texture2D layersTexHover = Ass.LayersIconSmallHover.Value;

        // Position
        Rectangle invRect = DragSystem.InventoryBounds();
        int paddingY = -60;
        int spacingX = 2;
        Vector2 editorPos = new Vector2(invRect.X,invRect.Bottom + paddingY);
        Vector2 layersPos = new Vector2(editorPos.X + editorTex.Width + spacingX,editorPos.Y);
        Rectangle editorRect = new((int)editorPos.X, (int)editorPos.Y, editorTex.Width, editorTex.Height);
        Rectangle layersRect = new((int)layersPos.X+4, (int)layersPos.Y, layersTex.Width, layersTex.Height);
        layersRect.Width += 10; // 33*1.5 = 49,5-33=16,5
        layersRect.Height += 10;

        // Check hover
        bool overEditor = editorRect.Contains(Main.MouseScreen.ToPoint());
        bool overLayers = layersRect.Contains(Main.MouseScreen.ToPoint());

        // Draw edit icon
        sb.Draw(
            overEditor ? editorTexHover : editorTex,
            editorPos,
            null,
            Color.White,
            0f,
            Vector2.Zero,
            1.0f,
            SpriteEffects.None,
            0f
        );

        // Draw layer icon
        sb.Draw(
            overLayers ? layersTexHover : layersTex,
            layersPos,
            null,
            Color.White,
            0f,
            Vector2.Zero,
            1.5f,
            SpriteEffects.None,
            0f
        );

        // Debug
        //sb.Draw(TextureAssets.MagicPixel.Value, editorRect, Color.Red * 0.5f);
        //sb.Draw(TextureAssets.MagicPixel.Value, layersRect, Color.LightSkyBlue * 0.5f);

        // Block item use
        if (overEditor)
        {
            Main.LocalPlayer.mouseInterface = true;
            string openOrClose = EditSystem.IsActive ? "Close" : "Open";
            Main.instance.MouseText($"{openOrClose} UI editor panel");
        }
        if (overLayers)
        {
            Main.LocalPlayer.mouseInterface = true;
            string openOrClose = LayerSystem.IsActive ? "Close" : "Open";
            Main.instance.MouseText($"{openOrClose} layer panel");
        }

        // Handle clicks
        if (Main.mouseLeft && Main.mouseLeftRelease)
        {
            if (overEditor)
            {
                EditSystem.ToggleActive();
                Main.mouseLeftRelease = false;
            }
            else if (overLayers)
            {
                LayerSystem.ToggleActive();
                Main.mouseLeftRelease = false;
            }
        }
    }
}
