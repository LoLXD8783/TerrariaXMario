using Terraria;

namespace TerrariaXMario.Content.Overalls;

internal class BrawnyWear : OverallsItem
{
    internal override int Defense => 25;
    internal override int PowAdditive => 40;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.value = Item.buyPrice(gold: 10);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);

    }
}
