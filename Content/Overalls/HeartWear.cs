using Terraria;

namespace TerrariaXMario.Content.Overalls;

internal class HeartWear : OverallsItem
{
    internal override int Defense => 16;
    internal override int PowAdditive => 32;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.value = Item.buyPrice(gold: 4, silver: 50);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);

    }
}