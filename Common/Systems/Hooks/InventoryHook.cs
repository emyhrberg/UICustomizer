using System;
using MonoMod.Cil;

namespace UICustomizer.Common.Systems.Hooks
{
    public class InventoryHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            IL_Main.DrawInventory += InjectInventoryOffset;
        }

        public override void Unload()
        {
            IL_Main.DrawInventory -= InjectInventoryOffset;
        }

        private void InjectInventoryOffset(ILContext il)
        {
            Log.Info("IL inventory patching...");

            try
            {
                // adjust num and num2 to account for the offset
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }
    }
}