using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using UICustomizer.Common.Systems;

namespace UICustomizer.UI.Layers
{
    public class LayerPanel : BasePanel
    {
        // Tabs
        public ElementsTab elementsTab;
        public LayersTab layersTab;
        public PacksTab packsTab;

        public LayerPanel()
        {
            Left.Set(1300, 0f);
        }

        protected override Action CloseAction => () => LayerSystem.SetActive(false);

        protected override (Tab, Tab, Tab) CreateTabs()
        {
            var elementsTab = new ElementsTab();
            var layersTab = new LayersTab();
            var packsTab = new PacksTab();
            return (layersTab, elementsTab, packsTab);
        }

        public override void Update(GameTime gameTime)
        {
            //Main.NewText(Top.Pixels);
            //Main.NewText(Left.Pixels);
            //Left.Set(0, 0);


            if (!LayerSystem.IsActive) return;

            base.Update(gameTime);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (!LayerSystem.IsActive) return;

            base.LeftClick(evt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!LayerSystem.IsActive) return; 
            
            base.Draw(spriteBatch);
        }
    }
}
