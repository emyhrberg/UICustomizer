using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using UICustomizer.Common.Systems;
using UICustomizer.Common.Systems.Hooks;

namespace UICustomizer.UI
{
    public class LayoutsPanel : DraggablePanel
    {
        public bool Active;
        private readonly UIList list;
        private readonly UIScrollbar bar;

        public LayoutsPanel()
        {
            BackgroundColor = ColorHelper.DarkBluePanel;
            Width.Set(300, 0); 
            Height.Set(400, 0);
            VAlign = 0.5f;
            HAlign = 0.1f;

            list = new UIList { 
                Width = { Percent = 1f }, 
                Height = { Percent = 1f }, 
                ListPadding = 4f,
                ManualSortMethod = (e) => { }
            };
            
            bar = new UIScrollbar { Height = { Percent = 1f }, HAlign = 1f };
            list.SetScrollbar(bar);

            Append(list);
            Append(bar);

            UIText header = new("Layouts", 0.6f, true) { HAlign = 0.5f };
            list.Add(header);

            // add one checkbox per layer (now or later in Update if dictionary still empty)
            Populate();
        }

        private void Populate()
        {
            list.Clear();
            UIElement spacing = new UIElement();
            spacing.Height.Set(3, 0f);   // 5-pixel gap
            list.Add(spacing);           // first item in the list
            list.Add(new UIText("Layouts", 0.4f, true) { HAlign = 0.5f });

            // Add buttonpanel for each layout
            ButtonPanel defaultLayout = new("Default", "No offsets applied", 0, () => ApplyLayout("Default"));
            ButtonPanel hotbarCenterLayout = new("HBCenter", "Hotbar centered", 0, () => ApplyLayout("HBCenter"));
            ButtonPanel hotbarBottomLayout = new("HBBottom", "Hotbar bottom", 0, () => ApplyLayout("HBBottom"));
            list.Add(defaultLayout);
            list.Add(hotbarCenterLayout);
            list.Add(hotbarBottomLayout);
        }

        private static void ApplyLayout(string layoutName)
        {
            Main.NewText("Applied layout: " + layoutName);

            if (layoutName == "Default")
            {
                HotbarHook.OffsetX = 0;
                HotbarHook.OffsetY = 0;
            }

            if (layoutName == "HBCenter")
            {
                HotbarHook.OffsetX = 50;
            }
            if (layoutName == "HBBottom")
            {
                HotbarHook.OffsetY = 500;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!Active) return;
            if (!UICustomizerSystem.EditModeActive) return;

            base.Update(gameTime);

            // Dragging code below._.
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (dragging)
            {
                Left.Set(Main.mouseX - offset.X, 0f);
                Top.Set(Main.mouseY - offset.Y, 0f);
                Recalculate();
            }

            var parentSpace = Parent.GetDimensions().ToRectangle();
            if (!GetDimensions().ToRectangle().Intersects(parentSpace))
            {
                Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
                Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
                Recalculate();
            }
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active) return;
            if (!UICustomizerSystem.EditModeActive) return;


            base.Draw(spriteBatch);
        }
    }
}
