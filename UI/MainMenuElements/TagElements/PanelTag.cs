using Terraria.GameContent.UI.Elements;

namespace UICustomizer.UI.MainMenuElements.TagElements
{
    internal class PanelTag : UIPanel
    {
        public UIText tagText;
        public PanelTag()
        {
            Height.Set(26, 0);
            HAlign = 1f;
            VAlign = 1f;
            Left.Set(-4, 0);
            Width.Set(95, 0);

            tagText = new UIText("#FFFFFF") { HAlign = .5f, VAlign = .5f };
            Append(tagText);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
