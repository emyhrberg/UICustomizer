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

            string msg = $"[Welcome to {this.Mod.DisplayName} v{this.Mod.Version}!]\n";
            msg += "There are two panels, type [c/FFFF00:/edit] or [c/FFFF00:/layers] in chat to use them. There is also a button above Settings.\n";
            msg += "[c/FFFF00:/edit] allows you to move elements and save your layouts.\n";
            msg += "[c/FFFF00:/layers] allows you to toggle elements, interface layers, and resource packs\n";
            msg += "Use the configuration to toggle this message and set other stuff, too.";

            Main.NewText(msg, Color.LightGray);
        }
    }
}