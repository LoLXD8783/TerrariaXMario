using MonoMod.Cil;
using TerrariaXMario.Content.Cap;

namespace TerrariaXMario.Common.JumpVariations;

[CanBeReadBySourceGenerators]
internal partial class FlutterJumpPlayer : JumpVariationPlayer
{
    [NetSync] internal byte time = 15;
    [NetSync] private byte timer = 15;
    [NetSync] internal sbyte bodyFrame = -1;
    [NetSync] internal sbyte legFrame = -1;

    internal override bool Enabled()
    {
        bool result = Player.StompPlayer.stompCount % 7 == 5 && CommonCondition(Player);
        if (!result) legFrame = -1;

        //bodyFrame = (sbyte)(result ? 0 : -1);
        Player.CapPlayer.currentVariation = result ? EquipSet.Flying.Name : EquipSet.Default.Name;

        return false;
    }

    internal override void Update()
    {
        //float armAngle = MathHelper.PiOver4;
        //Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, armAngle * Player.direction);
        //Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, (armAngle - MathHelper.PiOver2) * Player.direction);

        if (timer > 0) timer--;
        else
        {
            legFrame = (sbyte)((legFrame == 19) ? 7 : (legFrame + 1));
            timer = time;
        }
    }

    public override void Load()
    {
        // allows Player.bodyFrame.Y and Player.legFrame.Y to be set while airborne
        IL_Player.PlayerFrame += ModifyPlayerFrame;
    }

    private void ModifyPlayerFrame(ILContext il)
    {
        ILCursor c = new(il);
        ILLabel label = c.DefineLabel();
        ILLabel label2 = c.DefineLabel();
        ILLabel label3 = c.DefineLabel();
        ILLabel label4 = c.DefineLabel();

        c.Next(i => i.MatchLdcI4(5), i => i.MatchMul());
        c.Index++;

        c.EmitPop();
        c.EmitLdarg0();
        c.EmitDelegate((Player player) =>
        {
            FlutterJumpPlayer modPlayer = player.FlutterJumpPlayer;
            int legFrame = modPlayer.legFrame;

            return modPlayer.Enabled() && modPlayer.legFrame != -1 ? modPlayer.legFrame : 5;
        });

        c.Next(i => i.MatchStfld<Rectangle>("Y"), i => i.MatchLdarg0(), i => i.MatchLdcR8(0), i => i.MatchStfld<Player>("bodyFrameCounter"));
        c.Index--;

        c.EmitPop();
        c.EmitLdarg0();
        c.EmitDelegate((Player player) =>
        {
            FlutterJumpPlayer modPlayer = player.FlutterJumpPlayer;
            int bodyFrame = modPlayer.bodyFrame;

            return modPlayer.Enabled() && modPlayer.bodyFrame != -1 ? modPlayer.bodyFrame : 5;
        });
    }
}