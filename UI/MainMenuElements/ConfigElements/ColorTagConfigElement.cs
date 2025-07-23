using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ModLoader.Config.UI;
using UICustomizer.UI.MainMenuElements.TagElements;

public class ColorTagConfigElement : ConfigElement<string>
{
    private PanelTag panelTag;

    public ColorTagConfigElement()
    {
        panelTag = new PanelTag();
        panelTag.Left.Set(30f, 0); // space for color box
        Append(panelTag);
    }

    public override void OnBind()
    {
        base.OnBind();

        // Safe place to use Value
        if (string.IsNullOrWhiteSpace(Value))
            Value = "#FFFFFF";

        UpdateText();
    }

    public override void Update(GameTime gameTime)
    {
        panelTag.Left.Set(0f, 0); // space for color box

        base.Update(gameTime);
        UpdateText();
    }

    private void UpdateText()
    {
        string hex = Value ?? "#FFFFFF";
        panelTag.tagText.SetText(hex.ToUpper());
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);

        Rectangle tagRect = panelTag.GetDimensions().ToRectangle();
        Rectangle previewRect = new(tagRect.Left - 28, tagRect.Top + 2, 22, 22);

        if (TryParseHexColor(Value, out Color color))
        {
            Texture2D t = TextureAssets.MagicPixel.Value;
            sb.Draw(t, previewRect, color);
            DrawBorder(sb, previewRect, Color.Black);
        }
    }

    private static bool TryParseHexColor(string hex, out Color color)
    {
        try
        {
            if (string.IsNullOrEmpty(hex)) throw new Exception();
            if (hex.StartsWith("#")) hex = hex[1..];
            if (hex.Length != 6) throw new Exception();

            byte r = Convert.ToByte(hex.Substring(0, 2), 16);
            byte g = Convert.ToByte(hex.Substring(2, 2), 16);
            byte b = Convert.ToByte(hex.Substring(4, 2), 16);
            color = new Color(r, g, b);
            return true;
        }
        catch
        {
            color = Color.White;
            return false;
        }
    }

    private void DrawBorder(SpriteBatch sb, Rectangle rect, Color color)
    {
        Texture2D t = TextureAssets.MagicPixel.Value;
        sb.Draw(t, new Rectangle(rect.X - 1, rect.Y - 1, rect.Width + 2, 1), color); // Top
        sb.Draw(t, new Rectangle(rect.X - 1, rect.Bottom, rect.Width + 2, 1), color); // Bottom
        sb.Draw(t, new Rectangle(rect.X - 1, rect.Y, 1, rect.Height), color); // Left
        sb.Draw(t, new Rectangle(rect.Right, rect.Y, 1, rect.Height), color); // Right
    }
}
