using System;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace UIEditor.Core.IngameEditor.Hooks;

public class MapToggleHook : ModSystem
{
    public static float OffsetX = 0;
    public static float OffsetY = 0;

    public override void Load()
    {
        IL_Main.DrawInventory += InjectMapToggleOffset;
    }

    public override void Unload()
    {
        IL_Main.DrawInventory -= InjectMapToggleOffset;
    }

    private void InjectMapToggleOffset(ILContext il)
    {
        try
        {
            var c = new ILCursor(il);

            InjectLocalAdjustment(c, nameof(OffsetX),
                i => i.MatchLdcI4(440),
                i => i.MatchSub(),
                i => i.MatchStloc(out _));

            c.Index = 0;
            InjectLocalAdjustment(c, nameof(OffsetY),
                i => i.MatchLdcI4(40),
                i => i.MatchLdloc(out _),
                i => i.MatchAdd(),
                i => i.MatchStloc(out _));

            c.Index = 0;
            InjectLocalAdjustment(c, nameof(OffsetX),
                i => i.MatchLdcI4(40),
                i => i.MatchSub(),
                i => i.MatchStloc(out _));

            c.Index = 0;
            InjectLocalAdjustment(c, nameof(OffsetY),
                i => i.MatchLdcI4(200),
                i => i.MatchSub(),
                i => i.MatchStloc(out _));
        }
        catch (Exception e)
        {
            throw new ILPatchFailureException(Mod, il, e);
        }
    }

    private static void InjectLocalAdjustment(ILCursor c, string offsetFieldName, params Func<Instruction, bool>[] pattern)
    {
        int localIndex = -1;
        if (!c.TryGotoNext(MoveType.After, pattern))
        {
            Log.Error($"Could not find pattern for {nameof(MapToggleHook)}.{offsetFieldName}");
            return;
        }

        Instruction previous = c.Prev;
        if (previous?.OpCode == OpCodes.Stloc_0) localIndex = 0;
        else if (previous?.OpCode == OpCodes.Stloc_1) localIndex = 1;
        else if (previous?.OpCode == OpCodes.Stloc_2) localIndex = 2;
        else if (previous?.OpCode == OpCodes.Stloc_3) localIndex = 3;
        else if (previous?.OpCode == OpCodes.Stloc_S || previous?.OpCode == OpCodes.Stloc)
            localIndex = ((VariableDefinition)previous.Operand).Index;

        if (localIndex < 0)
        {
            Log.Error($"Could not resolve local index for {nameof(MapToggleHook)}.{offsetFieldName}");
            return;
        }

        FieldInfo offsetField = typeof(MapToggleHook).GetField(offsetFieldName);
        c.EmitLdloc(localIndex);
        c.EmitLdsfld(offsetField);
        c.EmitConvI4();
        c.EmitAdd();
        c.EmitStloc(localIndex);
    }
}
