using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace UIEditor.Core.LayersEditor;
public class LayerState : UIState
{
    public LayerPanel layersPanel;

    public LayerState()
    {
        layersPanel = new();
        Append(layersPanel);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);
    }
}