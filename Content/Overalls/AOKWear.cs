using Terraria;

namespace TerrariaXMario.Content.Overalls;

internal class AOKWear : OverallsItem
{
    internal override int Defense => 100;
    internal override int PowAdditive => 175;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.value = Item.buyPrice(10);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);

    }
}
