namespace TerrariaXMario.Common.Stomp;

internal class StompImpactDust : ModDust
{
    private int timeLeft = 0;
    public override void OnSpawn(Dust dust)
    {
        timeLeft = 45;
        dust.rotation = Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
        dust.noGravity = true;
        dust.frame = new Rectangle(0, Main.rand.Next(3) * 16, 16, 16);
    }

    public override bool Update(Dust dust)
    {
        if (timeLeft > 0) timeLeft--;

        dust.scale -= 0.05f;
        dust.position += dust.velocity * (timeLeft * 0.075f);
        if (dust.scale < 0) dust.active = false;

        return false;
    }

    internal static void Spawn(Player player, int amount = 2, float speedXMultiplier = 1, float speedYRandMin = -0.5f, float speedYRandMax = 0.5f)
    {
        int num = amount / 2;
        for (int i = -num; i < num + 1; i++)
        {
            if (i == 0) continue;
            Dust.NewDustPerfect(player.gravDir == 1 ? player.Bottom : player.Top, ModContent.DustType<StompImpactDust>(), new Vector2(speedXMultiplier * Math.Sign(i), player.gravDir * Main.rand.NextFloat(speedYRandMin, speedYRandMax)));
        }
    }
}
