using System;
using Terraria.GameContent.UI.Elements;
using UICustomizer.Common.Configs;
using UICustomizer.Common.Systems;
using UICustomizer.Common.Systems.Hooks;

namespace UICustomizer.UI.Tabs
{
    public sealed class EditorTab : Tab
    {
        private UIText positions;
        public Checkbox CheckboxX { get; private set; }
        public Checkbox CheckboxY { get; private set; }
        public Checkbox CheckboxHitbox { get; private set; }
        public Checkbox CheckboxNames { get; private set; }

        public EditorTab(Action<Tab> select, UIScrollbar bar = null)
            : base("Editor", select, bar)
        {
        }

        protected override void Populate()
        {
            var saveBtn = new Button("Save", "Save and exit edit mode", 0, UICustomizerSystem.ExitEditMode);
            var resetBtn = new Button("Reset", "Reset all offsets", 0, ResetAllOffsets);
            var testBtn = new Button("Test", "Do something", 0, ResetAllOffsets);

            CheckboxX = new Checkbox("X", "Move only in X", 50);
            CheckboxY = new Checkbox("Y", "Move only in Y", 50);
            CheckboxHitbox = new Checkbox("Hitbox", "Show hitboxes", 80);
            CheckboxNames = new Checkbox("Names", "Show names", 80);

            const int btnH = 24;
            const int vGap = 4;              // 4-px blank row between button rows
            const int rowTot = btnH + vGap;    // 28 px per “row”
            const int leftX = 0;
            const int chkX = 100;            // where check-boxes begin
            const int chkYOffset = 2;          // center check-box vertically in 24-px row

            // helper to make one row
            static UIElement Row(UIElement btn, UIElement chkLeft = null, UIElement chkRight = null, int rowIndex = 0)
            {
                var row = new UIElement
                {
                    Width = { Percent = 1f },
                    Height = { Pixels = btnH }
                };
                int top = rowIndex * rowTot;
                row.Top.Pixels = top;

                btn.Left.Pixels = leftX;
                btn.Top.Pixels = 0;
                row.Append(btn);

                if (chkLeft != null)
                {
                    chkLeft.Left.Pixels = chkX;
                    chkLeft.Top.Pixels = chkYOffset;
                    row.Append(chkLeft);
                }
                if (chkRight != null)
                {
                    chkRight.Left.Pixels = chkX + 80;
                    chkRight.Top.Pixels = chkYOffset;
                    row.Append(chkRight);
                }
                return row;
            }

            // headers
            UIText changeUIHeader = new("Change UI", 0.5f, true);
            UIText positionsHeader = new("Positions", 0.5f, true);

            // assemble rows
            Gap();
            TryAdd(changeUIHeader);
            Gap();
            TryAdd(Row(saveBtn, CheckboxX, CheckboxY, 0)); // row 0
            TryAdd(Row(resetBtn, CheckboxHitbox, CheckboxNames, 1)); // row 1
            TryAdd(Row(testBtn, null, null, 2)); // row 2
            Gap(12);
            TryAdd(positionsHeader);
            Gap();
            positions = new("", .35f, true);
            TryAdd(positions);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Chat:   ({(int)ChatHook.OffsetX}, {(int)ChatHook.OffsetY})");
            sb.AppendLine($"Hotbar: ({(int)HotbarHook.OffsetX}, {(int)HotbarHook.OffsetY})");
            sb.AppendLine($"Map:    ({(int)MapHook.OffsetX},  {(int)MapHook.OffsetY})");
            sb.AppendLine($"InfoAccs: ({(int)InfoAccsHook.OffsetX}, {(int)InfoAccsHook.OffsetY})");
            sb.AppendLine($"FancyLife: ({(int)FancyLifeHook.OffsetX}, {(int)FancyLifeHook.OffsetY})");
            sb.AppendLine($"ClassicLife: ({(int)ClassicLifeHook.OffsetX}, {(int)ClassicLifeHook.OffsetY})");
            sb.AppendLine($"ClassicMana: ({(int)ClassicManaHook.OffsetX}, {(int)ClassicManaHook.OffsetY})");
            sb.AppendLine($"FancyMana: ({(int)FancyManaHook.OffsetX}, {(int)FancyManaHook.OffsetY})");
            sb.AppendLine($"HorizontalLifeBar: ({(int)HorizontalLifeBarHook.OffsetX}, {(int)HorizontalLifeBarHook.OffsetY})");
            positions.SetText(sb.ToString(), 0.35f, true);
            positions.Height.Set(240, 0); // TODO update to real value
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
            InfoAccsHook.OffsetX = InfoAccsHook.OffsetY = 0f;

            // Reset config
            Conf.C.ChatOffsetX = Conf.C.ChatOffsetY = 0f;
            Conf.C.HotbarOffsetX = Conf.C.HotbarOffsetY = 0f;
            Conf.C.MapOffsetX = Conf.C.MapOffsetY = 0f;
            Conf.C.InfoAccsOffsetX = Conf.C.InfoAccsOffsetY = 0f;
            Conf.C.FancyLifeOffsetX = Conf.C.FancyLifeOffsetY = 0f;
            Conf.C.ClassicLifeOffsetX = Conf.C.ClassicLifeOffsetY = 0f;
            Conf.C.ClassicManaOffsetX = Conf.C.ClassicManaOffsetY = 0f;
            Conf.C.FancyManaOffsetX = Conf.C.FancyManaOffsetY = 0f;
            Conf.C.HorizontalLifeBarOffsetX = Conf.C.HorizontalLifeBarOffsetY = 0f;
        }
    }
}
