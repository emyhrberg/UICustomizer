using Terraria.GameContent.UI.Elements;
using UICustomizer.Common.Systems;
using UICustomizer.Common.Systems.Hooks;

namespace UICustomizer.UI.Tabs
{
    public sealed class EditorTab : Tab
    {
        private UIText positions;
        public Checkbox CheckboxX { get; private set; }
        public Checkbox CheckboxY { get; private set; }
        public Checkbox CheckboxDarken { get; private set; }
        public Checkbox CheckboxHitbox { get; private set; }
        public Checkbox CheckboxNames { get; private set; }

        public EditorTab() : base("Editor") { }

        protected override void Populate()
        {
            Log.Info("EditorTab.Populate() called");

            // Create elements
            Button saveButton = new("Save", "Save and exit edit mode", 0, UICustomizerSystem.ExitEditMode);
            Button resetButton = new("Reset", "Reset all offsets", 0, ResetAllOffsets);
            positions = new("", .35f, true);
            CheckboxX = new("X", "Enable X offset editing");
            CheckboxY = new("Y", "Enable Y offset editing");
            CheckboxDarken = new("Darken", "Darken the background when editing");
            CheckboxHitbox = new("Show Hitbox", "Show hitboxes of all UI");
            CheckboxNames = new("Show Names", "Show names of all UI elements");

            Log.Info("Created all UI elements");

            // Build the tab with content
            Gap();
            list.Add(saveButton);
            list.Add(resetButton);
            Gap();
            list.Add(CheckboxX);
            list.Add(CheckboxY);
            list.Add(CheckboxDarken);
            list.Add(CheckboxHitbox);
            list.Add(CheckboxNames);
            Gap();
            list.Add(positions);
            positions.SetText("", 0.35f, true);

            Log.Info($"Added {list.Count} elements to list");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Chat:   ({(int)ChatHook.OffsetX}, {(int)ChatHook.OffsetY})");
            sb.AppendLine($"Hotbar: ({(int)HotbarHook.OffsetX}, {(int)HotbarHook.OffsetY})");
            sb.AppendLine($"Map:    ({(int)MapHook.OffsetX},  {(int)MapHook.OffsetY})");
            sb.AppendLine($"InfoAccs: ({(int)InfoAccsHook.OffsetX}, {(int)InfoAccsHook.OffsetY})");
            sb.AppendLine(""); // empty line for readability
            sb.AppendLine($"FancyLife: ({(int)FancyLifeHook.OffsetX}, {(int)FancyLifeHook.OffsetY})");
            sb.AppendLine($"ClassicLife: ({(int)ClassicLifeHook.OffsetX}, {(int)ClassicLifeHook.OffsetY})");
            sb.AppendLine($"ClassicMana: ({(int)ClassicManaHook.OffsetX}, {(int)ClassicManaHook.OffsetY})");
            sb.AppendLine($"FancyMana: ({(int)FancyManaHook.OffsetX}, {(int)FancyManaHook.OffsetY})");
            sb.AppendLine($"HorizontalLifeBar: ({(int)HorizontalLifeBarHook.OffsetX}, {(int)HorizontalLifeBarHook.OffsetY})");
            positions.SetText(sb.ToString(), 0.35f, true);
        }

        private void ResetAllOffsets()
        {
            ChatHook.OffsetX = ChatHook.OffsetY = 0f;
            HotbarHook.OffsetX = HotbarHook.OffsetY = 0f;
            MapHook.OffsetX = MapHook.OffsetY = 0f;
            FancyLifeHook.OffsetX = FancyLifeHook.OffsetY = 0f;
            ClassicLifeHook.OffsetX = ClassicLifeHook.OffsetY = 0f;
            ClassicManaHook.OffsetX = ClassicManaHook.OffsetY = 0f;
            FancyManaHook.OffsetX = FancyManaHook.OffsetY = 0f;
            HorizontalLifeBarHook.OffsetX = HorizontalLifeBarHook.OffsetY = 0f;
        }
    }
}
