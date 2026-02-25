using Terraria;

namespace TerrariaXMario.Content.Overalls;

internal class DStarWear : OverallsItem
{
    internal override int Defense => 90;
    internal override int PowAdditive => 140;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.value = Item.buyPrice(5);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);

    }
}
