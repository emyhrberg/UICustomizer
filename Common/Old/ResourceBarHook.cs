// using System;
// using MonoMod.Cil;
// using Terraria.GameContent.UI.ResourceSets;

// namespace UICustomizer.Common.Systems.Hooks
// {
//    public class ResourceBarHook : ModSystem
//    {
//        public static float OffsetX = 0;
//        public static float OffsetY = 0;

//        public override void Load()
//        {
//         //    IL_Main.DrawHealthBar += InjectOffset;
//         //    IL_Main.DrawBuffIcon += InjectOffset;
//            //IL_Main.GUIBarsDrawInner += SkipCallTest;
//         //    IL_PlayerResourceSetsManager.Draw += SkipCallTest;
//        }

//        public override void Unload()
//        {
//            IL_Main.DrawHealthBar -= InjectOffset;
//            IL_Main.DrawBuffIcon -= InjectOffset;
//            //IL_Main.GUIBarsDrawInner -= SkipCallTest;
//            IL_PlayerResourceSetsManager.Draw -= SkipCallTest;
//        }

//        private void SkipCallTest(ILContext il)
//        {
//            Log.Info("Skip call patching...");

//            try
//            {
//                ILCursor c = new(il);

//                ILLabel target = c.DefineLabel();

//                // Match to place we want to jump from
//                c.EmitBr(target);

//                // Jump to after the line
//                c.GotoNext(MoveType.After,
//                    i => i.MatchCallvirt<PlayerResourceSetsManager>("Draw"));
//                c.MarkLabel(target);

//                // Insert delegate here
//                // (Do nothing, effectively skipping the call)
//                c.EmitDelegate(() =>
//                {
//                    Log.Info("SkipIL");// This is a no-op, effectively skipping the call
//                });
//            }
//            catch (Exception e)
//            {
//                throw new ILPatchFailureException(Mod, il, e);
//            }
//        }

//        private void InjectOffset(ILContext il)
//        {
//            Log.Info("IL resource bar patching...");

//            try
//            {
//                ILCursor c = new(il);
//                int count = 0;
//                while (c.TryGotoNext(MoveType.After,
//                    i => i.MatchNewobj<Vector2>()))
//                {
//                    Log.Info("ILcount: " + count);
//                    count++;
//                    c.EmitDelegate((Vector2 pos) =>
//                    {
//                        return new Vector2(
//                            pos.X + OffsetX,
//                            pos.Y + OffsetY
//                        );
//                    });
//                }
//            }
//            catch (Exception e)
//            {
//                throw new ILPatchFailureException(Mod, il, e);
//            }
//        }
//    }
// }