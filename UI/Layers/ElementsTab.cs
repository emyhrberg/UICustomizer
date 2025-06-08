using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.UI;
using Terraria.ModLoader;
using UICustomizer.UI;
using UICustomizer.Common.Systems;
using UICustomizer.Helpers;
using Terraria.ModLoader.Core; // for GetModInstance

public sealed class ElementsTab : Tab
{
    // track expanded/collapsed state
    private bool _filtersExpanded = true;
    private bool _elementsExpanded = true;
    private static object actions;

    // filter state per‐mod (and "All")
    private readonly Dictionary<string, bool> _filterStates =
        new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

    public ElementsTab() : base("Elements") { }

    public static Mod GetModInstance(Type type)
    {
        Log.Info("Cotlim1 Trying");
        var modType = AssemblyManager.GetLoadableTypes(type.Assembly)
            .FirstOrDefault(t => t.IsSubclassOf(typeof(Mod)) && !t.IsAbstract, null);
        if (modType == null) 
                return null;

        // Imagine this as ModContent.GetInstance<modType>()
        var method = typeof(ModContent)
            .GetMethod(nameof(ModContent.GetInstance))
            ?.GetGenericMethodDefinition()
            ?.MakeGenericMethod(modType);
        var result = method?.Invoke(null, null);

        if (result is not Mod instance)
            throw new InvalidCastException($"{modType.FullName} is not a Mod or could not be instantiated via ModContent.GetInstance<T>()");
        Log.Info("Cotlim2 succcesss" + instance.displayNameClean);

        return instance;
    }

    public override void Populate()
    {
        list.Clear();

        // grab the current active‐name list
        var sys = ModContent.GetInstance<UIElementSystem>();
        var allNames = sys.debugState
                          .activeUIElementsNameList
                          .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                          .ToList();

        //
        // 2) build a mapping from element‐name → mod name ("Vanilla" if none)
        //
        var elementModMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in allNames)
        {
            string modName = "Vanilla";
            try
            {
                var t = Type.GetType(name);
                if (t != null)
                {
                    var modInst = GetModInstance(t);
                    if (modInst != null)
                        modName = modInst.Name;
                }
            }
            catch (Exception ex)
            {
                Main.NewText($"[UICustomizer] Couldn't resolve mod for '{name}': {ex.Message}");
            }
            elementModMap[name] = modName;
        }

        // distinct list of mods + ensure filter state entries exist
        var modNames = elementModMap.Values
                          .Distinct(StringComparer.OrdinalIgnoreCase)
                          .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                          .ToList();

        // add "All" at the front
        if (!modNames.Contains("All"))
            modNames.Insert(0, "All");

        foreach (var m in modNames)
            if (!_filterStates.ContainsKey(m))
                _filterStates[m] = true;

        //
        // 3) Filters section
        //
        var filtersSection = new CollapsibleSection(
            title: "Filters",
            initialState: _filtersExpanded,

            buildContent: content =>
            {
                float y = 10f;
                foreach (var m in modNames)
                {
                    var chk = new Checkbox(
                        text: m,
                        hover: $"Show elements from {m}",
                        width: 300
                    )
                    {
                        Top = { Pixels = y },
                        state = _filterStates[m]
                                    ? CheckboxState.Checked
                                    : CheckboxState.Unchecked
                    };
                    chk.box.SetImage(_filterStates[m] ? Ass.Check : Ass.Uncheck);

                    chk.OnLeftClick += (_, _) =>
                    {
                        // toggle your stored flag
                        _filterStates[m] = !_filterStates[m];

                        // now do your “All”‐sync logic
                        if (m == "All")
                        {
                            foreach (var other in modNames.Where(x => x != "All"))
                                _filterStates[other] = _filterStates["All"];
                        }
                        else if (!_filterStates[m])
                        {
                            // if any single mod is turned off, uncheck "All"
                            _filterStates["All"] = false;
                        }

                        // rebuild the UI—your checkboxes will now be constructed
                        // with the new values in _filterStates
                        Populate();
                    };


                    content.Append(chk);
                    y += 22f;
                }
            },

            onToggle: () => _filtersExpanded = !_filtersExpanded,
            contentHeight: () => Math.Max(80, modNames.Count * 22 + 20),

            buildHeader: header =>
            {
                // tiny “toggle all filters” box in the header
                bool allOn = modNames.All(m => _filterStates[m]);
                var toggleAll = new Checkbox(text: "", hover: "Toggle all filters", width: 20)
                {
                    HAlign = 1f,
                    VAlign = 0.5f,
                    Left = { Pixels = -24 },
                    Top = { Pixels = -4 },
                    state = allOn ? CheckboxState.Checked : CheckboxState.Unchecked
                };
                toggleAll.box.SetImage(allOn ? Ass.Check : Ass.Uncheck);

                toggleAll.OnLeftClick += (evt, _) =>
                {
                    if (evt.Target is Checkbox) return;
                    bool newState = toggleAll.state == CheckboxState.Unchecked;
                    foreach (var m in modNames)
                        _filterStates[m] = newState;
                    Populate();
                };

                header.Append(toggleAll);
            }
        );
        list.Add(filtersSection);
        Gap(8);

        //
        // 4) UI Elements section, showing only those whose mod filter is on (or "All")
        //
        var filtered = allNames
            .Where(n =>
            {
                var mod = elementModMap[n];
                return _filterStates["All"] || (_filterStates.ContainsKey(mod) && _filterStates[mod]);
            })
            .ToList();

        if (filtered.Count > 0)
        {
            var elemsSection = new CollapsibleSection(
                title: "UI Elements",
                initialState: _elementsExpanded,

                buildContent: content =>
                {
                    float y = 10f;
                    foreach (var name in filtered)
                    {
                        var chk = new Checkbox(
                            text: name,
                            hover: $"Show all {name} UIElements",
                            width: 300
                        )
                        {
                            Top = { Pixels = y },
                            state = sys.debugState.GetElement(name, true)
                                        ? CheckboxState.Checked
                                        : CheckboxState.Unchecked
                        };
                        chk.box.SetImage(chk.state == CheckboxState.Checked
                                            ? Ass.Check
                                            : Ass.Uncheck);
                        chk.OnLeftClick += (_, _) =>
                            sys.debugState.SetElement(name, chk.state == CheckboxState.Checked);

                        content.Append(chk);
                        y += 22f;
                    }
                },

                onToggle: () => _elementsExpanded = !_elementsExpanded,
                contentHeight: () => Math.Max(80, filtered.Count * 22 + 20),

                buildHeader: header =>
                {
                    bool allOn = filtered.All(n => sys.debugState.GetElement(n, true));
                    var toggleAll = new Checkbox(text: "", hover: "Toggle all UI Elements", width: 20)
                    {
                        HAlign = 1f,
                        VAlign = 0.5f,
                        Left = { Pixels = -24 },
                        Top = { Pixels = -4 },
                        state = allOn ? CheckboxState.Checked : CheckboxState.Unchecked
                    };
                    toggleAll.box.SetImage(allOn ? Ass.Check : Ass.Uncheck);

                    toggleAll.OnLeftClick += (evt, _) =>
                    {
                        if (evt.Target is Checkbox) return;
                        bool newState = toggleAll.state == CheckboxState.Unchecked;
                        foreach (var n in filtered)
                            sys.debugState.SetElement(n, newState);
                        Populate();
                    };

                    header.Append(toggleAll);
                }
            );
            list.Add(elemsSection);
        }

        // finally clear the source list for the next tick
        sys.debugState.activeUIElementsNameList.Clear();
        list.Recalculate();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (Main.GameUpdateCount % 60 == 0)
            Populate();
    }
}
