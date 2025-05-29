using UICustomizer.Common.Configs;
using UICustomizer.Common.Systems.Hooks;

namespace UICustomizer.Common.Systems
{
    // On game launch/entry, set all offsets according to config values.
    // This is a bad solution, plz fix later. 
    // If this is in final code, i will cry
    public class LoadOffsetSystem : ModSystem
    {
        public override void Load()
        {
            if (Conf.C == null) return;

            // Load offsets from config
            ChatHook.OffsetX = Conf.C.ChatOffsetX;
            ChatHook.OffsetY = Conf.C.ChatOffsetY;
            HotbarHook.OffsetX = Conf.C.HotbarOffsetX;
            HotbarHook.OffsetY = Conf.C.HotbarOffsetY;
            MapHook.OffsetX = Conf.C.MapOffsetX;
            MapHook.OffsetY = Conf.C.MapOffsetY;
        }

        public override void OnModLoad()
        {
            if (Conf.C == null) return;

            // Load offsets (again?!) from config
            ChatHook.OffsetX = Conf.C.ChatOffsetX;
            ChatHook.OffsetY = Conf.C.ChatOffsetY;
            HotbarHook.OffsetX = Conf.C.HotbarOffsetX;
            HotbarHook.OffsetY = Conf.C.HotbarOffsetY;
            MapHook.OffsetX = Conf.C.MapOffsetX;
            MapHook.OffsetY = Conf.C.MapOffsetY;
        }
    }

    public class LoadOffsetPlayer : ModPlayer
    {
        public override void OnEnterWorld()
        {
            if (Conf.C == null) return;

            // Load offsets (again?!) from config
            ChatHook.OffsetX = Conf.C.ChatOffsetX;
            ChatHook.OffsetY = Conf.C.ChatOffsetY;
            HotbarHook.OffsetX = Conf.C.HotbarOffsetX;
            HotbarHook.OffsetY = Conf.C.HotbarOffsetY;
            MapHook.OffsetX = Conf.C.MapOffsetX;
            MapHook.OffsetY = Conf.C.MapOffsetY;
        }
    }

}