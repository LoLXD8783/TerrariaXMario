using Terraria;

namespace TerrariaXMario.Content.Overalls;

internal class MasterWear : OverallsItem
{
    internal override int Defense => 66;
    internal override int PowAdditive => 84;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.value = Item.buyPrice(1, 50);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);

    }
}
