using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using UICustomizer.Common.Systems;

namespace UICustomizer.UI.Layers
{
    public class LayersPanel : BasePanel
    {
        // Tabs
        public ElementsTab elementsTab;
        public LayersTab layersTab;
        public PacksTab packsTab;

        protected override Action CloseAction => () => LayersSystem.SetActive(false);

        protected override (Tab, Tab, Tab) CreateTabs()
        {
            var elementsTab = new ElementsTab();
            var layersTab = new LayersTab();
            var packsTab = new PacksTab();
            return (elementsTab, layersTab, packsTab);
        }

        public override void Update(GameTime gameTime)
        {
            if (!LayersSystem.IsActive) return;

            base.Update(gameTime);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (!LayersSystem.IsActive) return;

            base.LeftClick(evt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!LayersSystem.IsActive) return; 
            
            base.Draw(spriteBatch);
        }
    }
}
