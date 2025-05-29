using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using UICustomizer.Common.Configs;
using UICustomizer.Common.Systems;
using UICustomizer.Common.Systems.Hooks;

namespace UICustomizer.UI
{
    public class UIEditorPanel : DraggablePanel
    {
        private UIText title;
        private ButtonPanel saveBtn;
        private ButtonPanel resetBtn;
        private UIText positionsText;
        public Checkbox checkboxX;
        public Checkbox checkboxY;

        public UIList list;
        protected UIScrollbar scrollbar;
        public Resize resize;

        public UIEditorPanel()
        {
            BackgroundColor = ColorHelper.DarkBluePanel;
            Width.Set(200, 0);
            Height.Set(400, 0);
            Left.Set(Main.screenWidth - 215, 0);
            Top.Set(Main.screenHeight - 415, 0);

            // ----- scrollable list -----
            list = new UIList
            {
                Width = { Percent = 1f, Pixels = 0 },
                Height = { Percent = 1f, Pixels = 0 },
                Top = { Pixels = 0 },
                Left = { Pixels = 0 },
                ListPadding = 4f
            };
            Append(list);                                   // sibling #1

            // ----- scrollbar (sibling of list) -----
            scrollbar = new UIScrollbar
            {
                Height = { Percent = 1f, Pixels = -24-24-5 },
                Top = { Pixels = 24 },
                Left = { Percent = 1f, Pixels = -15 }
            };
            Append(scrollbar);                              // sibling #2
            list.SetScrollbar(scrollbar);                   // wire them together

            // ----- resize handle -----
            resize = new Resize(Ass.Resize)
            {
                HAlign = 1f,
                VAlign = 1f
            };
            resize.OnDragY += dy =>
            {
                CancelDrag();                               // stop panel drag
                float min = 80f;
                float newH = Math.Clamp(Height.Pixels + dy, min: min, max: 1000f);
                Height.Set(newH, 0);
                list.Height.Set(newH - 24, 0);
                Recalculate();
                list.Recalculate();
                scrollbar.Recalculate();
            };
            //resize.OnDragX += dx =>
            //{
            //    CancelDrag();                               // stop panel drag
            //    float newW = Math.Clamp(Height.Pixels + dx, 180f, 1000f);
            //    Width.Set(newW, 0);
            //    list.Height.Set(newW, 0);
            //    Recalculate();
            //    list.Recalculate();
            //    scrollbar.Recalculate();
            //};
            Append(resize);                                 // sibling #3

            // ----- controls inside the list -----
            int off = 30;
            UIElement spacer = new UIElement();
            spacer.Height.Set(3, 0f);   // 5-pixel gap
            list.Add(spacer);           // first item in the list
            title = new UIText("UI Editor", 0.55f, true) { HAlign = 0.5f, Top = { Pixels = 0 } };
            saveBtn = new ButtonPanel("Save", "Save and exit edit mode", off, SaveAndExitEditMode);
            resetBtn = new ButtonPanel("Reset", "Reset all offsets", off * 2, ResetOffsets);
            checkboxX = new Checkbox("Edit X", "Move only in X") { Top = { Pixels = off * 3 } };
            checkboxY = new Checkbox("Edit Y", "Move only in Y") { Top = { Pixels = off * 4 } };
            positionsText = new UIText("", 0.4f, true) { Top = { Pixels = off * 5 } };

            list.Add(title);
            list.Add(saveBtn);
            list.Add(resetBtn);
            list.Add(checkboxX);
            list.Add(checkboxY);
            list.Add(positionsText);
        }

        private void ResetOffsets()
        {
            ChatHook.OffsetX = ChatHook.OffsetY =
            HotbarHook.OffsetX = HotbarHook.OffsetY =
            MapHook.OffsetX = MapHook.OffsetY =
            InfoAccsHook.OffsetX = InfoAccsHook.OffsetY = 0;

            Conf.C.ChatOffsetX = ChatHook.OffsetX;
            Conf.C.ChatOffsetY = ChatHook.OffsetY;
            Conf.C.HotbarOffsetX = HotbarHook.OffsetX;
            Conf.C.HotbarOffsetY = HotbarHook.OffsetY;
            Conf.C.MapOffsetX = MapHook.OffsetX;
            Conf.C.MapOffsetY = MapHook.OffsetY;
            Conf.Save();
        }

        private void SaveAndExitEditMode()
        {
            Conf.C.ChatOffsetX = ChatHook.OffsetX;
            Conf.C.ChatOffsetY = ChatHook.OffsetY;
            Conf.C.HotbarOffsetX = HotbarHook.OffsetX;
            Conf.C.HotbarOffsetY = HotbarHook.OffsetY;
            Conf.C.MapOffsetX = MapHook.OffsetX;
            Conf.C.MapOffsetY = MapHook.OffsetY;
            Conf.C.InfoAccsOffsetX = InfoAccsHook.OffsetX;
            Conf.C.InfoAccsOffsetY = InfoAccsHook.OffsetY;
            Conf.Save();

            UICustomizerSystem.ExitEditMode();
        }

        public override void Update(GameTime gameTime)
        {
            if (!UICustomizerSystem.EditModeActive) return;

            // --- HOT RELOAD TESTING ---
            //Height.Set(400, 0);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Chat:     ({(int)ChatHook.OffsetX}, {(int)ChatHook.OffsetY})");
            sb.AppendLine($"Hotbar:   ({(int)HotbarHook.OffsetX}, {(int)HotbarHook.OffsetY})");
            sb.AppendLine($"Map:      ({(int)MapHook.OffsetX}, {(int)MapHook.OffsetY})");
            sb.AppendLine($"InfoAccs: ({(int)InfoAccsHook.OffsetX}, {(int)InfoAccsHook.OffsetY})");
            positionsText.SetText(sb.ToString(), 0.35f, true);
            positionsText.Height.Set(100, 0);
            //list.Recalculate();                      // refresh list's total content height
            //scrollbar.Recalculate();                 // scrollbar thumb now updates

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!UICustomizerSystem.EditModeActive) return;

            base.Draw(spriteBatch);
        }
    }
}
