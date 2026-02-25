using Terraria;

namespace TerrariaXMario.Content.Overalls;

internal class StarWear : OverallsItem
{
    internal override int Defense => 75;
    internal override int PowAdditive => 110;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.value = Item.buyPrice(3);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);

    }
}
