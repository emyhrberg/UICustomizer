using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using UICustomizer.UI.Layers;

namespace UICustomizer.Common.States
{
    public class LayerState : UIState
    {
        public LayersPanel layersPanel;
        public LayersButton layersButton;

        public LayerState()
        {
            layersPanel = new();
            Append(layersPanel);

            layersButton = new();
            Append(layersButton);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //layersButton = new();
            //Append(layersButton);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
    }
}