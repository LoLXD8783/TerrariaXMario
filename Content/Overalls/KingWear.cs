using Terraria;

namespace TerrariaXMario.Content.Overalls;

internal class KingWear : OverallsItem
{
    internal override int Defense => 72;
    internal override int PowAdditive => 94;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.value = Item.buyPrice(2, 50);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);

    }
}
