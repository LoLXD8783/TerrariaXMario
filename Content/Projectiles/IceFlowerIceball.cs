using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaXMario.Common.MiscEffects;

namespace TerrariaXMario.Content.Projectiles;

internal class IceFlowerIceball : FireFlowerFireball
{
    internal override Color OutlineColor => new(0, 143, 254);
    internal override Color FillColor => new(180, 254, 254);
    internal override int? PairedMetaballDust => ModContent.DustType<IceFlowerIceballDust>();

    public override void SetDefaults()
    {
        base.SetDefaults();

        gravity = 0.2f;
        bounceSpeed = -4;
        dustType = DustID.BlueTorch;
        buffType = BuffID.Frostburn;
        dustChance = 2;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (tileCollideCount < 2) return base.OnTileCollide(oldVelocity);
        return true;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.FinalDamage.CombineWith(new(0, 0));
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);

        SoundEngine.PlaySound(new($"{GetType().FullName!.Replace(".", "/")}Kill") { Volume = 0.4f }, Projectile.Center);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        SoundEngine.PlaySound(new($"{TerrariaXMario.Sounds}/Misc/Freeze") { Volume = 0.4f }, target.Center);
        target.GetGlobalNPC<IceBlockNPC>().freezePlayer = Projectile.owner;
        target.GetGlobalNPC<IceBlockNPC>().frozen = true;
    }
}
