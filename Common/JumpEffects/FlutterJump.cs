namespace TerrariaXMario.Common.JumpEffects;

[CanBeReadBySourceGenerators]
internal partial class FlutterJumpPlayer : JumpEffectPlayer
{
    private readonly int time = 15;
    [NetSync] private int timer;

    internal override bool CanUpdate()
    {
        bool result = Player.StompPlayer.stompCount % 7 == 5 && CommonCondition(Player);
        if (!result) legFrame = -1;

        bodyFrame = result ? 0 : -1;

        return result;
    }

    internal override void Update()
    {
        if (timer > 0) timer--;
        else
        {
            legFrame++;

            if (legFrame < 7) legFrame = 19;
            else if (legFrame > 19) legFrame = 7;

            Main.NewText(legFrame);
            timer = time;
        }
    }
}