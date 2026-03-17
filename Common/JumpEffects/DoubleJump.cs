namespace TerrariaXMario.Common.JumpEffects;

[CanBeReadBySourceGenerators]
internal class DoubleJumpPlayer : JumpEffectPlayer
{
    internal override bool CanUpdate()
    {
        bool result = Player.StompPlayer.stompCount % 7 is 4 or 6 && CommonCondition(Player);

        bodyFrame = result ? 7 : -1;

        return result;
    }
}
