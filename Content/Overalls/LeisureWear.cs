using Terraria;

namespace TerrariaXMario.Content.Overalls;

internal class LeisureWear : OverallsItem
{
    internal override int Defense => 10;
    internal override int PowAdditive => 12;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.value = Item.buyPrice(gold: 1, silver: 15);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);

    }
}
