using Terraria.DataStructures;

namespace TerrariaXMario.Common.JumpVariations;

[CanBeReadBySourceGenerators]
internal partial class JumpVariationPlayer : ModPlayer
{
    internal static bool CommonCondition(Player player) => player.CapPlayer.Enabled && !player.IsOnGroundPrecise && !player.GroundPoundPlayer.IsGroundPounding;

    internal virtual bool Enabled() => false;
    internal virtual void Update() { }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;

        if (!player.CapPlayer.Enabled || player.IsOnGroundPrecise) return;

        bool reset = true;

        foreach (ModPlayer modPlayer in player.ModPlayers)
        {
            if (modPlayer is not JumpVariationPlayer jumpVariationPlayer || !jumpVariationPlayer.Enabled()) continue;
            jumpVariationPlayer.Update();
            reset = false;
        }

        if (reset)
        {
            player.fullRotationOrigin = player.Size * 0.5f;
            player.fullRotation = player.fullRotation.AngleLerp(0, 0.1f);
        }
    }
}