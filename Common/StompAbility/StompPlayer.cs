using TerrariaXMario.Content.Cap;

namespace TerrariaXMario.Common.StompAbility;

[CanBeReadBySourceGenerators]
internal partial class StompPlayer : ModPlayer
{
    [NetSync] internal uint stompCount;

    public override void PostUpdate()
    {
        if (Player.IsOnGroundPrecise) stompCount = 0;

        if (!Player.CapPlayer.Enabled || Player.velocity.Y <= 0) return;

        // Adapted from Player.JumpMovement, specifically when using the Golf Cart mount

        Rectangle stompHitboxRect = Player.getRect();
        stompHitboxRect.Offset(0, Player.height - 1);
        stompHitboxRect.Height = 2;
        stompHitboxRect.Inflate(6, 6);

        for (int i = 0; i < 200; i++)
        {
            NPC npc = Main.npc[i];
            if (!npc.active || npc.dontTakeDamage || npc.friendly || npc.immune[Player.whoAmI] != 0 || !Player.CanNPCBeHitByPlayerOrPlayerProjectile(npc)) continue;

            if (stompHitboxRect.Intersects(npc.getRect()) && (npc.noTileCollide || Collision.CanHit(Player.position, Player.width, Player.height, npc.position, npc.width, npc.height)))
            {
                int velocityDirection = Math.Sign(Player.velocity.X);

                if (Player.whoAmI == Main.myPlayer) Player.ApplyDamageToNPC(npc, 1, 0, velocityDirection != 0 ? velocityDirection : Player.direction, true);

                npc.immune[Player.whoAmI] = 10;
                StompImpactDust.Spawn(Player);

                if (!Player.GroundPoundPlayer.IsGroundPounding)
                {
                    Player.velocity.Y = (Player.jumpSpeed + Player.jumpSpeedBoost) * -Player.gravDir * (Player.controlJump ? 2 : 1.5f);
                    CapPlayer.PlaySound(Player, "Stomp", pitch: Math.Clamp(stompCount, 0, 6) * 0.165f);

                    if (stompCount > 6)
                    {
                        Player.Heal(1);
                        CapPlayer.PlaySound(Player, "Heal");
                    }

                    if (Player.LeanJumpPlayer.Enabled()) Player.LeanJumpPlayer.angle *= -1;

                    stompCount++;
                }

                Player.GiveImmuneTimeForCollisionAttack(6);
                break;
            }
        }
    }
}
