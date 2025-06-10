using System;
using System.Collections.Generic;
using UICustomizer.Common.States;
using UICustomizer.UI;

public sealed class ElementsTab : Tab
{
    public ElementsTab() : base("Elements") { }

    private readonly Dictionary<string, bool> expandedSections = new(StringComparer.OrdinalIgnoreCase);

    private int knownElements = -1;

    public override void Populate()
    {
        list.Clear();
        list.SetPadding(20);
        list.ListPadding = 0;
        list.Left.Set(-8, 0);
        list.Top.Set(-10, 0);

        PopulateElements();
    }

    private void PopulateElements()
    {
        var elements = UIElementRegistrator.allElements;

        BuildSection("Elements");
    }

    private void BuildSection(string title)
    {
        var elements = UIElementRegistrator.allElements;

        var section = new CollapsibleSection(title, content =>
        {
            foreach (var element in elements)
            {
                var chk = new CheckboxElement(
                    text: element,
                    initialState: true,
                    onStateChanged: (val) => { /* do nothing on state change for now*/ },
                    width: 0,
                    eye: true,
                    maxWidth: true,
                    height: 20
                )
                { Active = true };
                content.Add(chk);
            }
        },
            expandedSections.TryGetValue(title, out var v) ? v : (expandedSections[title] = false),
            onToggle: () => expandedSections[title] = !expandedSections[title],
            contentHeightFunc: () => 100f,
            buildHeader: header =>
            {
                /* do nothing for header for now */
            }
        );
        list.Add(section);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        //if (Main.GameUpdateCount % 60 == 0) Populate();

        // if (knownElements != UIElementRegistrator.allElements.Count)
        // {
        //Populate();
        // }
    }
}
