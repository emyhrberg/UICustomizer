using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.Config.UI;

namespace UIEditor.Common.Configs;

public class ColorTagConfigElement : ConfigElement<string>
{
    private readonly UIPanel _tagPanel;
    private readonly UIText _tagText;

    public ColorTagConfigElement()
    {
        _tagPanel = new UIPanel();
        _tagPanel.Height.Set(26f, 0f);
        _tagPanel.Width.Set(0f, 1f);
        _tagPanel.Left.Set(30f, 0f);              // leave room on the left
        Append(_tagPanel);

        _tagText = new UIText("");
        _tagText.VAlign = 0.5f;
        _tagText.Left.Set(4f, 0f);
        _tagPanel.Append(_tagText);
    }

    public override void OnBind()
    {
        base.OnBind();
        // if (string.IsNullOrWhiteSpace(Value))
        // Value = "#FFFFFF";                    // default white
        UpdateText();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        // UpdateText();                             // keep text fresh
    }

    private void UpdateText()
    {
        string hex = Value ?? "#FFFFFF";
        _tagText.SetText(hex.ToUpper());
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);

        _tagPanel.Width.Set(105f, 0f);
        _tagPanel.VAlign = 0.5f;
        _tagPanel.HAlign = 1f;
        _tagPanel.Left.Set(0f, 0f);              // leave room on the left

        if (!TryParseHexColor(Value, out Color col))
            col = Color.White;

        Rectangle tagRect = _tagPanel.GetDimensions().ToRectangle();
        Rectangle previewRect = new Rectangle(tagRect.Left - 28, tagRect.Top + 2, 22, 22);

        Texture2D px = TextureAssets.MagicPixel.Value;
        sb.Draw(px, previewRect, col);            // filled box
        DrawBorder(sb, previewRect, Color.Black); // thin black border
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

    private static void DrawBorder(SpriteBatch sb, Rectangle rect, Color color)
    {
        Texture2D px = TextureAssets.MagicPixel.Value;
        sb.Draw(px, new Rectangle(rect.X - 1, rect.Y - 1, rect.Width + 2, 1), color); // Top
        sb.Draw(px, new Rectangle(rect.X - 1, rect.Bottom, rect.Width + 2, 1), color); // Bottom
        sb.Draw(px, new Rectangle(rect.X - 1, rect.Y, 1, rect.Height), color);        // Left
        sb.Draw(px, new Rectangle(rect.Right, rect.Y, 1, rect.Height), color);        // Right
    }
}
