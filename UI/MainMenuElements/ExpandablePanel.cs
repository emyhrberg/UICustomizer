using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.UI;

namespace UICustomizer.UI.MainMenuElements
{
    internal class ExpandablePanel : UIExpandablePanel
    {
        public ExpandablePanel()
        {
            defaultHeight = 30f;
            Width.Set(320, 0);
            Height.Set(30, 0);
            OverflowHidden = false;
            BackgroundColor = ColorHelper.DarkBluePanel * 0.5f;
            SetPadding(0);
            HAlign = 1f;
            Top.Set(10, 0);

            expandButton.Top.Set(4, 0);
            expandButton.Left.Set(-30, 1);

            //Collapse(); // start collapsed 

            // TODO remove this on release
            expanded = true;
            pendingChanges = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
