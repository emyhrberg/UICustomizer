using System;
using UICustomizer.Common.Systems;

namespace UICustomizer.UI.Tabs
{
    public sealed class ThemeTab : Tab
    {
        private bool _themeHeaderExpanded = true;

        public ThemeTab(Action<Tab> select, Scrollbar bar) : base("Themes", select, bar)
        {
        }
        protected override void Populate()
        {
            list.Clear();

            AddCollapsibleHeader(
                text: "Themes",
                getState: () => _themeHeaderExpanded,
                setState: (state) => _themeHeaderExpanded = state,
                onToggle: () => Populate() // do nothing
            );

            PopulateThemes();
        }

        private void PopulateThemes()
        {
            if (!_themeHeaderExpanded)
                return;

            for (int i = 1; i <= 3; i++)
            {
                var layoutName = $"Example {i}";
                var hover = $"This is {layoutName}";
                var btn = new Button(
                    text: layoutName,
                    tooltip: () => hover,
                    onClick: () =>
                    {
                        Main.NewText($"You clicked {layoutName} theme button! (This has not been implemented yet.)", 255, 255, 0);
                    },
                    maxWidth: true
                );

                TryAdd(btn); // Add the button to the UI
            }
        }
    }
}
