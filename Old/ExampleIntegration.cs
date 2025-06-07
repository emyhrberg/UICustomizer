// using System;

// [JITWhenModsEnabled("ModReloader")]
// public sealed class ExampleModReloaderButtonIntegration : ModPlayer
// {
//     public override void OnEnterWorld()
//     {
//         if (ModLoader.TryGetMod("ModReloader", out Mod MR))
//         {
//             MR.Call(
//                 "AddButton",
//                 "Example", // name
//                 new Action(() => Main.NewText("Example")), // action
//                 Ass.DragonLensToolIcon, // asset
//                 "Example tooltip" // tooltip
//             );
//         }
//     }
// }