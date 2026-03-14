namespace TerrariaXMario.Common.JumpEffects;

[CanBeReadBySourceGenerators]
internal partial class FlipJumpPlayer : JumpEffectPlayer
{
    [NetSync] internal int Time { get; private set; }
    [NetSync] internal int Timer { get; private set; }
    [NetSync] internal Vector2 RotationOrigin { get; private set; }
    [NetSync] internal int FlipCount { get; private set; }
    [NetSync] internal int MaxFlipCount { get; private set; }

    internal void Init(int flipDuration, Vector2 rotationOrigin, int maxFlipCount = 1)
    {
        if (flipDuration <= 0) throw new Exception($"{nameof(flipDuration)} must be positive");
        if (maxFlipCount <= 0) throw new Exception($"{nameof(maxFlipCount)} must be positive");

        Time = Timer = flipDuration;
        MaxFlipCount = maxFlipCount;
        RotationOrigin = rotationOrigin;
    }

    internal override bool CanUpdate()
    {
        bool canDoGroundPoundFlip = Player.GroundPoundPlayer.IsGroundPounding && CommonCondition(Player, true);
        bool result = canDoGroundPoundFlip;

        if (!result)
        {
            FlipCount = 0;
            Timer = 0;
        }

        return result;
    }

    internal override void Update()
    {
        Player.fullRotationOrigin = RotationOrigin;

        if (Timer > 0)
        {
            Player.fullRotation = MathHelper.TwoPi * -Player.direction * ((float)Timer / Time);
            Timer--;
        }
        else
        {
            Player.fullRotation = 0;
            FlipCount++;
            if (FlipCount < MaxFlipCount) Timer = Time;
        }
    }
}
