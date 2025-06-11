using DragonLens.Core.Systems.ThemeSystem;
using DragonLens.Core.Systems.ToolSystem;
using DragonLens.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using UICustomizer.Helpers;

namespace UICustomizer.Common.Systems.Integrations.DragonLens
{
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DragonLensLayersTool : Tool
    {
        public override string IconKey => "Layers";

        public override string Name => "Layers And Packs";

        public override string Description => "Toggle UIElements, Drawn Interface Layers from all mods, and Toggle Resource Packs directly in-game.";

        public override bool HasRightClick => true;

        public override void OnActivate()
        {
            LayerSystem.ToggleActive();
        }

        public override void OnRightClick()
        {
            //base.OnRightClick();

            // TODO Toggle all UI element hitboxes
        }

        public override void DrawIcon(SpriteBatch spriteBatch, Rectangle position)
        {
            // base.DrawIcon(spriteBatch, position);

            // Draw the icon for the UI Editor tool
            Asset<Texture2D> asset = Ass.LayersIcon;
            if (asset == null || !asset.IsLoaded)
            {
                Log.Error("LayersIcon is not loaded");
                return;
            }
            Texture2D tex = asset.Value;


            //if (tex.Width > position.Width || tex.Height > position.Height)
            //scale = tex.Width > tex.Height ? position.Width / tex.Width : position.Height / tex.Height;

            float scale = 1.85f;
            Vector2 pos = new(position.Center.X, position.Center.Y);

            spriteBatch.Draw(tex, pos, null, Color.White, 0, tex.Size() / 2f, scale, 0, 0);

            if (LayerSystem.IsActive)
            {
                // Draw the icon with a glow effect when edit mode is active
                GUIHelper.DrawOutline(spriteBatch, new Rectangle(position.X - 4, position.Y - 4, 46, 46), ThemeHandler.ButtonColor.InvertColor());

                // Texture2D tex = DragonLensAssets.Misc.GlowAlpha.Value;
                Asset<Texture2D> glowTex = ModContent.Request<Texture2D>("DragonLens/Assets/Misc/GlowAlpha");
                if (glowTex == null || !glowTex.IsLoaded)
                {
                    return;
                }

                Color color = new(255, 215, 150);
                color.A = 0;
                var glowPosition = new Rectangle(position.X, position.Y, 38, 38);
                spriteBatch.Draw(glowTex.Value, glowPosition, color);
            }
        }
    }
}
