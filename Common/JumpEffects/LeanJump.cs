//namespace TerrariaXMario.Common.JumpEffects;

//[CanBeReadBySourceGenerators]
//internal partial class LeanJumpPlayer : JumpEffectPlayer
//{
//    [NetSync] internal float angle = MathHelper.PiOver4 * 0.5f;

//    internal override bool CanUpdate() => false/*Player.StompPlayer.stompCount % 7 is 2 or 3 && CommonCondition(Player)*/;

//    internal override void Update()
//    {
//        Player.fullRotationOrigin = Player.Size * 0.5f;
//        Player.fullRotation = Player.fullRotation.AngleLerp(Player.gravDir * angle, 0.1f);
//    }
//}
