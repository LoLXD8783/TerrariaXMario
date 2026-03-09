using Terraria.ID;

namespace TerrariaXMario.Content.Cap;

internal readonly struct EquipSet()
{
    internal readonly string Name = "";
    internal readonly EquipType[] EquipTypes = [];

    internal EquipSet(string variation = "", params EquipType[] equipTypes) : this()
    {
        Name = variation;
        EquipTypes = equipTypes;
    }

    internal static readonly EquipSet Default = new("", EquipType.Head, EquipType.Body, EquipType.Legs);
    internal static readonly EquipSet GroundPound = new("GroundPound", EquipType.Legs);
    internal static readonly EquipSet Flying = new("Flying", EquipType.Head, EquipType.Body, EquipType.Legs);

    internal void AddEquipTextures(ModItem item)
    {
        foreach (EquipType equipType in EquipTypes)
        {
            EquipLoader.AddEquipTexture(TerrariaXMario.Instance, $"{item.Texture}{Name}_{equipType}", equipType, item, $"{item.Name}{Name}");
        }
    }

    internal static int GetEquipSlot(string name, string variation, EquipType equipType) => EquipLoader.GetEquipSlot(TerrariaXMario.Instance, name + variation, equipType);

    internal void SetupEquipTextures(ModItem item)
    {
        foreach (EquipType equipType in EquipTypes)
        {
            switch (equipType)
            {
                case EquipType.Head:
                    int equipSlotHead = EquipLoader.GetEquipSlot(TerrariaXMario.Instance, $"{item.Name}{Name}", EquipType.Head);
                    if (equipSlotHead != -1) ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;

                    break;
                case EquipType.Body:
                    int equipSlotBody = EquipLoader.GetEquipSlot(TerrariaXMario.Instance, $"{item.Name}{Name}", EquipType.Body);
                    if (equipSlotBody != -1)
                    {
                        ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
                        ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
                    }

                    break;
                case EquipType.Legs:
                    int equipSlotLegs = EquipLoader.GetEquipSlot(TerrariaXMario.Instance, $"{item.Name}{Name}", EquipType.Legs);
                    if (equipSlotLegs != -1) ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;

                    break;
                default:
                    break;
            }
        }
    }
}
