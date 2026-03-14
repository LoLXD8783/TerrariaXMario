namespace TerrariaXMario.Common.PMeter;

[CanBeReadBySourceGenerators]
internal partial class PMeterPlayer : ModPlayer
{
    [NetSync] internal float chargeUpRate = 0.01f;
    [NetSync]
    internal float Charge
    {
        get; set
        {
            field = Math.Clamp(value, 0, 1);
        }
    }

    internal bool FullCharge => Charge == 1;

    public override void PostUpdateRunSpeeds()
    {
        if (!Player.CapPlayer.Enabled || Player.mount.Active || Player.velocity.X == 0)
        {
            Charge = 0;
            return;
        }

        if (Player.IsOnGroundPrecise) Charge += chargeUpRate * ((Math.Abs(Player.velocity.X) != 0 && (Player.controlRight || Player.controlLeft)) ? 1 : -1);
    }
}
