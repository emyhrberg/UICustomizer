// using System;
// using System.Linq;
// using Microsoft.Xna.Framework.Graphics;
// using Terraria.GameContent.UI.Elements;
// using UICustomizer.Common.Systems;

// namespace UICustomizer.UI
// {
//     public class LayerTogglePanel : MainPanel
//     {
//         public bool Active = false;
//         private readonly UIList list;
//         private readonly UIScrollbar bar;

//         public LayerTogglePanel()
//         {
//             BackgroundColor = ColorHelper.DarkBluePanel;
//             Width.Set(300, 0);
//             Height.Set(400, 0);
//             VAlign = 0.5f;
//             HAlign = 0.1f;

//             list = new UIList
//             {
//                 Width = { Percent = 1f },
//                 Height = { Percent = 1f },
//                 ListPadding = 4f,
//                 ManualSortMethod = (e) => { }
//             };

//             bar = new UIScrollbar { Height = { Percent = 1f }, HAlign = 1f };
//             list.SetScrollbar(bar);

//             Append(list);
//             Append(bar);

//             UIText header = new("Interface layers", 0.6f, true) { HAlign = 0.5f };
//             list.Add(header);

//             // add one checkbox per layer (now or later in Update if dictionary still empty)
//             Populate();
//         }
//         private void AddCheckbox(string name, bool visible)
//         {
//             Checkbox cb = null; // Declare the variable before using it  
//             cb = new Checkbox(name, "", () =>
//             {
//                 LayerToggleSystem.LayerStates[name] = cb.state == CheckboxState.Checked;
//             });

//             cb.state = visible ? CheckboxState.Checked : CheckboxState.Unchecked;
//             cb.box.SetImage(visible ? Ass.Check : Ass.Uncheck);

//             list.Add(cb);
//         }

//         // store last dictionary size so we only rebuild when it really changes
//         private int knownLayerCount;

//         private void Populate()
//         {
//             list.Clear();
//             UIElement spacing = new UIElement();
//             spacing.Height.Set(3, 0f);   // 5-pixel gap
//             list.Add(spacing);           // first item in the list
//             list.Add(new UIText("Interface layers", 0.4f, true) { HAlign = 0.5f });

//             var dict = LayerToggleSystem.LayerStates;

//             // layers that start with V
//             foreach (var (name, visible) in dict
//                      .Where(p => p.Key.StartsWith("V", StringComparison.OrdinalIgnoreCase))
//                      .OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase))
//                 AddCheckbox(name, visible);

//             // everything else
//             foreach (var (name, visible) in dict
//                      .Where(p => !p.Key.StartsWith("V", StringComparison.OrdinalIgnoreCase))
//                      .OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase))
//                 AddCheckbox(name, visible);

//             list.Recalculate();
//             bar.Recalculate();
//             knownLayerCount = dict.Count;
//         }

//         public override void Update(GameTime gameTime)
//         {
//             if (!Active) return;
//             if (!UICustomizerSystem.EditModeActive) return;


//             //list.Clear();
//             //Populate();

//             // rebuild only when the engine adds / removes layers
//             if (LayerToggleSystem.LayerStates.Count != knownLayerCount)
//                 Populate();

//             base.Update(gameTime);

//             if (ContainsPoint(Main.MouseScreen))
//             {
//                 Main.LocalPlayer.mouseInterface = true;
//             }

//             if (dragging)
//             {
//                 Left.Set(Main.mouseX - offset.X, 0f);
//                 Top.Set(Main.mouseY - offset.Y, 0f);
//                 Recalculate();
//             }

//             var parentSpace = Parent.GetDimensions().ToRectangle();
//             if (!GetDimensions().ToRectangle().Intersects(parentSpace))
//             {
//                 Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
//                 Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
//                 Recalculate();
//             }

//         }

//         public override void Draw(SpriteBatch spriteBatch)
//         {
//             if (!Active) return;
//             if (!UICustomizerSystem.EditModeActive) return;

//             base.Draw(spriteBatch);
//         }
//     }
// }
