using Terraria;

namespace TerrariaXMario.Content.Overalls;

internal class ThinWear : OverallsItem
{
    internal override int Defense => 2;
    internal override int PowAdditive => 2;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.value = Item.buyPrice(silver: 20);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!DoGearItemEffects(player)) return;
        base.UpdateAccessory(player, hideVisual);
        player.accRunSpeed *= 1.2f;
        player.maxRunSpeed *= 1.2f;
    }
}
