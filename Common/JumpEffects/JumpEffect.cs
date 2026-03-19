using Terraria.DataStructures;

namespace TerrariaXMario.Common.JumpEffects;

[CanBeReadBySourceGenerators]
internal partial class JumpEffectPlayer : ModPlayer
{
    [NetSync] internal bool shouldReset = true;
    [NetSync] internal int bodyFrame = -1;
    [NetSync] internal int legFrame = -1;

    internal static bool CommonCondition(Player player, bool canUpdateDuringGroundPound = false) => player.CapPlayer.Enabled && !player.IsOnGroundPrecise && (canUpdateDuringGroundPound || !player.GroundPoundPlayer.IsGroundPounding);

    internal bool Enabled => Player.CapPlayer.Enabled && !Player.IsOnGroundPrecise;

    internal virtual bool CanUpdate() => false;
    internal virtual void Update() { }

    internal static void Reset(Player player, bool force = false)
    {
        JumpEffectPlayer modPlayer = player.JumpEffectPlayer;

        if (!force && !modPlayer.shouldReset) return;
        player.fullRotationOrigin = player.Size * 0.5f;
        player.fullRotation = player.fullRotation.AngleLerp(0, 0.1f);
        modPlayer.bodyFrame = -1;
        modPlayer.legFrame = -1;
        modPlayer.shouldReset = true;
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;

        if (!player.JumpEffectPlayer.Enabled)
        {
            Reset(player, true);
            return;
        }

        shouldReset = true;

        foreach (ModPlayer modPlayer in player.ModPlayers)
        {
            if (modPlayer is not JumpEffectPlayer jumpVariationPlayer || !jumpVariationPlayer.CanUpdate()) continue;
            jumpVariationPlayer.Update();
            shouldReset = false;
        }

        Reset(player);
    }

    public override void PostUpdate()
    {
        if (!Enabled) return;

        if (bodyFrame != -1) Player.bodyFrame.Y = Player.bodyFrame.Height * bodyFrame;
        if (legFrame != -1) Player.legFrame.Y = Player.legFrame.Height * legFrame;
    }
}