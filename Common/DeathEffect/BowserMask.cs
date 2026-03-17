using TerrariaXMario.Core.Effects;

namespace TerrariaXMario.Common.DeathEffect;

internal class BowserMask : MaskEffect
{
    internal override bool Enabled => Main.LocalPlayer.dead;
    internal override Vector2 Size => new(494, 594);

    internal override void Init()
    {
        Scale = 5;
        ScreenPosition = Main.ScreenSize.ToVector2() * 0.5f - Size * 0.5f * Scale;
    }

    internal override void Update()
    {
        Scale = Math.Max(0, (Main.LocalPlayer.respawnTimer - 5) / 60f * 5);
        ScreenPosition = Main.ScreenSize.ToVector2() * 0.5f - Size * 0.5f * Scale;
    }
}
