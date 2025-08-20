using System;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.MainMenu.Hooks;

public sealed class TimeSpeedHook : ModSystem
{
    public static float Speed = 1f; // Default rotationSlider is 1x
    public override void Load()
    {
        var cfg = Conf.C;
        Speed = cfg?.MainMenuTime.Speed ?? 1f;

        IL_Main.UpdateMenu += ModifyMenuTime;
    }
    public override void Unload() => IL_Main.UpdateMenu -= ModifyMenuTime;

    // Edit by zoe, thanksssss!
    private void ModifyMenuTime(ILContext il)
    {
        try
        {
            ILCursor c = new(il);

            ILLabel skipRandomMoonType = c.DefineLabel();

            c.GotoNext(MoveType.Before,
                i => i.MatchCall<Main>($"get_{nameof(Main.rand)}"),
                i => i.MatchLdcI4(9));

            c.EmitBr(skipRandomMoonType);

            c.GotoNext(MoveType.After,
                i => i.MatchStsfld<Main>(nameof(Main.moonType)));

            c.MarkLabel(skipRandomMoonType);

            c.GotoNext(MoveType.AfterLabel,
                i => i.MatchBrfalse(out _),
                i => i.MatchRet());

            c.EmitDelegate(() =>
            {
                if (Main.time >= 0)
                    return;

                if (Main.dayTime)
                    Main.moonPhase = (Main.moonPhase - 1) % 8;

                Main.time = (Main.dayTime ? Main.nightLength : Main.dayLength) - 1;
                Main.dayTime = !Main.dayTime;
            });

            // Change the rotationSlider of scaleSlider.
            c.GotoNext(MoveType.After,
                i => i.MatchLdcR8(33.88235294117647));

            c.EmitDelegate((double time) => time * Speed);

            c.GotoNext(MoveType.After,
                i => i.MatchLdcR8(30.857142857142858));

            c.EmitDelegate((double time) => time * Speed);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}
