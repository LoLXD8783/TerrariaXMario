namespace TerrariaXMario.Common.JumpVariations;

[CanBeReadBySourceGenerators]
internal partial class LeanJumpPlayer : JumpVariationPlayer
{
    [NetSync] internal float angle = MathHelper.PiOver4 * 0.5f;

    internal override bool Enabled() => Player.StompPlayer.stompCount % 7 is 2 or 3 && CommonCondition(Player);

    internal override void Update()
    {
        Player.fullRotationOrigin = Player.Size * 0.5f;
        Player.fullRotation = Player.fullRotation.AngleLerp(Player.gravDir * angle, 0.1f);
    }
}
