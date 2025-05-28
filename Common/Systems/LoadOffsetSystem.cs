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
        }

        public override void OnModLoad()
        {
            if (Conf.C == null) return;

            // Load offsets (again?!) from config
            ChatHook.OffsetX = 0f;
            ChatHook.OffsetY = 0f;
            HotbarHook.OffsetX = 0f;
            HotbarHook.OffsetY = 0f;
        }
    }

    public class LoadOffsetPlayer : ModPlayer
    {
        public override void OnEnterWorld()
        {
            if (Conf.C == null) return;

            // Load offsets from config
            ChatHook.OffsetX = Conf.C.ChatOffsetX;
            ChatHook.OffsetY = Conf.C.ChatOffsetY;
            HotbarHook.OffsetX = Conf.C.HotbarOffsetX;
            HotbarHook.OffsetY = Conf.C.HotbarOffsetY;
        }
    }

}