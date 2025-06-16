using Terraria;
using Terraria.ModLoader;
using UICustomizer.Common.Configs;

namespace UICustomizer.Common.Players
{
    internal class OnEnterWorldMessage : ModPlayer
    {
        public override void OnEnterWorld()
        {
            base.OnEnterWorld();

            if (!Conf.C.ShowMessageWhenEnteringWorld) return;

            string msg = "";
            msg += $"{Loc.Get("PlayerMessages.OnEnterWorld.Welcome", this.Mod.DisplayName, this.Mod.Version)}\n";
            msg += $"{Loc.Get("PlayerMessages.OnEnterWorld.IntroPanels")}\n";
            msg += $"{Loc.Get("PlayerMessages.OnEnterWorld.EditToolInfo")}\n";
            msg += $"{Loc.Get("PlayerMessages.OnEnterWorld.LayersToolInfo")}\n";
            msg += $"{Loc.Get("PlayerMessages.OnEnterWorld.ToolIntegration")}\n";
            msg += $"{Loc.Get("PlayerMessages.OnEnterWorld.ConfigInfo")}";

            Main.NewText(msg, Color.LightGray);
        }
    }
}