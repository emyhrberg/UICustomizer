using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;
using UIEditor.Core.IngameEditor.Helpers;

namespace UIEditor.Core.IngameEditor.UI.Subpanels;
internal sealed class LayoutsPanel : UIPanel
{
    public LayoutsPanel()
    {
        BorderColor = new Color(89, 116, 213) * 0.9f;
        BackgroundColor = new Color(73, 94, 171) * 0.9f;

        Width.Set(360, 0);
        Height.Set(260, 0);
        Left.Set(90, 0);
        Top.Set(0, 0);
        SetPadding(8);

        Build();
    }

    private void Build()
    {
        RemoveAllChildren();

        float y = 4f;

        Append(new UIText("Layouts", 0.9f) { Top = { Pixels = y }, Left = { Pixels = 6 } });
        y += 18f;
        var line = new Underline(thickness: 2f, horizontalInset: 6f);
        line.Top.Pixels = y;
        Append(line);
        y += 8f;

        var names = TryGetLayouts() ?? new List<string>();
        foreach (var name in names)
        {
            Append(MakeButton(name, "Apply this layout", () =>
            {
                TryCall(() => LayoutHelper.ApplyLayout(name));
            }, y));
            y += 26f;
        }

        y += 6f;
        Append(new UIText("Options", 0.9f) { Top = { Pixels = y }, Left = { Pixels = 6 } });
        y += 18f;
        line.Top.Pixels = y;
        Append(line);
        y += 8f;

        Append(MakeButton("Open layout folder", "Open layout folder", () => TryCall(FileHelper.OpenLayoutFolder), y)); y += 26f;

        Append(MakeButton(
            "Save layout",
            "Save current positions to a new layout and set it in config",
            () => TryCall(() => LayoutHelper.SaveCurrentAs("MyCustomLayout", setAsActiveInConfig: true)),
            y
        ));
        y += 26f;
    }

    private UIElement MakeButton(string text, string tip, Action onClick, float y)
    {
        var btn = new UIPanel
        {
            Width = { Percent = 1f, Pixels = -12 },
            Height = { Pixels = 22 },
            Top = { Pixels = y },
            Left = { Pixels = 6 },
            BackgroundColor = new Color(60, 80, 150) * 0.9f,
            BorderColor = new Color(40, 55, 110)
        };
        btn.SetPadding(0);
        btn.Append(new UIText(text, 0.85f) { HAlign = 0.5f, VAlign = 0.5f });
        btn.OnLeftClick += (_, _) => onClick();
        btn.OnMouseOver += (_, _) =>
        {
            UICommon.TooltipMouseText(tip);
            btn.BackgroundColor = new Color(80, 100, 170) * 0.95f;
        };
        btn.OnMouseOut += (_, _) => btn.BackgroundColor = new Color(60, 80, 150) * 0.9f;
        return btn;
    }

    private static void TryCall(Action a)
    {
        try { a?.Invoke(); } catch { /* no-op if helpers not present */ }
    }
    private static List<string> TryGetLayouts()
    {
        try { return FileHelper.GetLayouts().ToList(); } catch { return null; }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        BackgroundColor = ColorHelper.DarkBluePanel * 0.75f;
        BorderColor = Color.Black * 0.45f;

        base.Draw(spriteBatch);
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        base.LeftClick(evt);
    }
}
