//namespace TerrariaXMario.Common.JumpEffects;

//[CanBeReadBySourceGenerators]
//internal partial class FlutterJumpPlayer : JumpEffectPlayer
//{
//    [NetSync] private int time = 15;
//    [NetSync] private int timer = 15;

//    internal override bool CanUpdate()
//    {
//        bool result = Player.StompPlayer.stompCount % 7 == 5 && CommonCondition(Player);
//        if (!result) legFrame = -1;

//        //bodyFrame = result ? 0 : -1;

//        return false;
//    }

//    internal override void Update()
//    {
//        //float armAngle = MathHelper.PiOver4;
//        //Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, armAngle * Player.direction);
//        //Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, (armAngle - MathHelper.PiOver2) * Player.direction);

//        if (timer > 0) timer--;
//        else
//        {
//            legFrame = (legFrame == 19) ? 7 : (legFrame + 1);
//            timer = time;
//        }
//    }
//}