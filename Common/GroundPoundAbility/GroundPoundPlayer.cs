using Terraria.DataStructures;
using Terraria.GameInput;
using TerrariaXMario.Common.CameraEffects;
using TerrariaXMario.Common.JumpEffects;
using TerrariaXMario.Common.StompAbility;
using TerrariaXMario.Content.Cap;
using TerrariaXMario.Utilities.Assets;

namespace TerrariaXMario.Common.GroundPoundAbility;

[CanBeReadBySourceGenerators]
internal partial class GroundPoundPlayer : ModPlayer
{
    internal bool IsGroundPounding => Player.CapPlayer.currentVariation == EquipSet.GroundPound.Name;

    [NetSync] private bool cancelledGroundPound;

    // Allows players to ground pound by holding Down while airborne
    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (!Player.CapPlayer.Enabled || Player.IsOnGroundPrecise || Player.velocity.Y == 0 || Player.mount.Active || IsGroundPounding) return;

        if (PlayerInput.Triggers.JustPressed.Down)
        {
            Player.FlipJumpPlayer.Init(60, new Vector2(Player.Size.X * 0.5f, Player.Size.Y * (Player.gravDir == 1 ? 0.75f : 0.25f)));
            Player.CapPlayer.currentVariation = EquipSet.GroundPound.Name;
            Assets.GroundPoundStart.Play(Player.MountedCenter);
        }
    }

    // Player velocity modifications
    public override void PreUpdateMovement()
    {
        if (cancelledGroundPound) Player.velocity.Y = 0.1f * Player.gravDir;

        if (!IsGroundPounding) return;

        Player.velocity = new Vector2(0, (Player.FlipJumpPlayer.Timer > 0 ? 1f : Player.maxFallSpeed) * Player.gravDir);
    }

    // Ground pound leg animation
    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;

        if (cancelledGroundPound)
        {
            player.fullRotation = 0;
            cancelledGroundPound = false;
        }

        if (!IsGroundPounding || player.FlipJumpPlayer.Timer == 0) return;

        player.JumpEffectPlayer.bodyFrame = 0;
        player.JumpEffectPlayer.legFrame = 3 - (int)Math.Clamp((float)player.FlipJumpPlayer.Timer / player.FlipJumpPlayer.Time * 3, 0, 3);
    }

    // Logic for ending a ground pound
    public override void PreUpdate()
    {
        if (IsGroundPounding && (!Player.CapPlayer.Enabled || Player.mount.Active || Player.IsOnGroundPrecise || (Player.FlipJumpPlayer.Timer == 0 && !Player.controlDown)))
        {
            if (Player.IsOnGroundPrecise) // Player hits the ground during ground pound
            {
                Assets.GroundPound.Play(Player.MountedCenter);
                CameraModifier.DoScreenShake(Player);
                StompImpactDust.Spawn(Player, 4, 1.5f, -1, -0.5f);
            }
            else if (!Player.controlDown) cancelledGroundPound = true;

            CapPlayer.ResetVariation(Player);
            JumpEffectPlayer.Reset(Player, true);
        }
    }

    public override void PostUpdateEquips()
    {
        if (IsGroundPounding) Player.noKnockback = true;
    }
}
