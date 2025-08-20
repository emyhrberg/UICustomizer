using System;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks
{
    /// <summary>
    /// Finds all calls to Vector2 constructor in the map drawing code and injects an offset to the position.
    /// </summary>
    public class MapHook : ModSystem
    {
        public static float OffsetX = 0;
        public static float OffsetY = 0;

        public override void Load()
        {
            IL_Main.DrawMap += InjectMapOffset;
            IL_Main.DrawNPCHeadFriendly += InjectNPCOffset;
            IL_Main.DrawNPCHeadBoss += InjectNPCOffset;
        }

        public override void Unload()
        {
            IL_Main.DrawMap -= InjectMapOffset;
            IL_Main.DrawNPCHeadFriendly -= InjectNPCOffset;
            IL_Main.DrawNPCHeadBoss -= InjectNPCOffset;
        }

        private void InjectMapOffset(ILContext il)
        {
            try
            {
                ILCursor c = new(il);
                int it = 0;

                int[] toRun = [2, 3, 15, 16];

                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchNewobj<Vector2>()))
                {
                    it++;

                    if (it == 2 || it == 3 || it == 15 || it == 16)
                    {
                        // Log.Info("Injecting offset for map position #" + it);
                        c.EmitDelegate<Func<Vector2, Vector2>>(pos =>
                        {
                            // Only apply offset when in minimap mode
                            if (Main.mapStyle == 1 && !Main.mapFullscreen)
                            {
                                return new Vector2(pos.X + (OffsetX * Main.MapScale), pos.Y + (OffsetY * Main.MapScale));
                            }
                            return pos;
                        });
                    }
                    else if (it == 8 || it == 9)
                    {
                        // Emit reverse offset
                        // Log.Info("Injecting reverse offset for map position #" + it);
                        c.EmitDelegate<Func<Vector2, Vector2>>(pos =>
                        {
                            // Only apply offset when in minimap mode
                            if (Main.mapStyle == 1 && !Main.mapFullscreen)
                            {
                                return new Vector2(pos.X - (OffsetX * Main.MapScale), pos.Y - (OffsetY * Main.MapScale));
                            }
                            return pos;
                        });
                    }
                    else
                    {
                        // Log.Info("Skipping offset injection for map position #" + it);
                    }
                }
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }

        private void InjectNPCOffset(ILContext il)
        {
            try
            {
                ILCursor c = new(il);

                while (c.TryGotoNext(MoveType.After,
                    i => i.MatchNewobj<Vector2>()))
                {
                    c.EmitDelegate<Func<Vector2, Vector2>>(pos =>
                    {
                        // Only apply offset when in minimap mode
                        if (Main.mapStyle == 1 && !Main.mapFullscreen)
                        {
                            return new Vector2(pos.X + (OffsetX * Main.MapScale), pos.Y + (OffsetY * Main.MapScale));
                        }
                        return pos;
                    });
                }
            }
            catch (Exception e)
            {
                throw new ILPatchFailureException(Mod, il, e);
            }
        }
    }
}