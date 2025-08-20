using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;
using UICustomizer.EditMode.Helpers;

namespace UICustomizer.Edit.UI; 
internal sealed class LayoutsPanel : UIPanel
{
    public LayoutsPanel()
    {
        BorderColor = new Color(89, 116, 213) * 0.9f;
        BackgroundColor = new Color(73, 94, 171) * 0.9f;

        Width.Set(360, 0);
        Height.Set(260, 0);
        Left.Set(70, 0);
        Top.Set(0, 0);
        SetPadding(8);

        Build();
    }

    private void Build()
    {
        RemoveAllChildren();

        float y = 4;

        // Active layout
        Append(MakeButton("Use Active", "Switch to Active layout", () =>
        {
            TryCall(() => LayoutHelper.ApplyLayout("Active"));
            TryCall(() => LayoutHelper.CurrentLayoutName = "Active");
            TryCall(() => LayoutHelper.SaveLastLayout());
        }, y));
        y += 28;

        // Presets
        Append(new UIText("Presets", 0.9f) { Top = { Pixels = y }, Left = { Pixels = 6 } });
        y += 20;

        var names = TryGetLayouts() ?? new List<string>();
        foreach (var name in names.Where(n => !string.Equals(n, "Active", StringComparison.OrdinalIgnoreCase)))
        {
            Append(MakeButton(name, "Apply this layout", () =>
            {
                TryCall(() => LayoutHelper.ApplyLayout(name));
                TryCall(() => LayoutHelper.CurrentLayoutName = name);
                TryCall(() => LayoutHelper.SaveLastLayout());
            }, y));

            y += 26;
        }

        y += 6;
        Append(new UIText("Options", 0.9f) { Top = { Pixels = y }, Left = { Pixels = 6 } });
        y += 20;

        Append(MakeButton("Open layout folder", "Open layout folder", () => TryCall(FileHelper.OpenLayoutFolder), y)); y += 26;
        Append(MakeButton("Save as new layout", "Create a new layout file", () => TryCall(() => FileHelper.CreateAndOpenNewLayoutFile("MyCustomLayout")), y)); y += 26;
        Append(MakeButton("Remove all layouts", "Delete all layouts", () => TryCall(FileHelper.DeleteAllLayouts), y)); y += 26;
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
}
