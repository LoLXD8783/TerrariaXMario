using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using TerrariaXMario.Common.BroInfoUI;
using TerrariaXMario.Common.CapEffects;
using TerrariaXMario.Common.SpawnableObject;
using TerrariaXMario.Content.Powerups;
using TerrariaXMario.Utilities.Extensions;

namespace TerrariaXMario.Content.Caps;

internal abstract class CapItem : ModItem, ISpawnableObject
{
    SpawnRarity ISpawnableObject.SpawnRarity { get; set; } = SpawnRarity.Legendary;

    private void LoadEquipTextures(string variation = "", bool head = true, bool body = true, bool legs = true)
    {
        if (head) EquipLoader.AddEquipTexture(Mod, $"{Texture}{variation}_{EquipType.Head}", EquipType.Head, this, $"{Name}{variation}");
        if (body) EquipLoader.AddEquipTexture(Mod, $"{Texture}{variation}_{EquipType.Body}", EquipType.Body, this, $"{Name}{variation}");
        if (legs) EquipLoader.AddEquipTexture(Mod, $"{Texture}{variation}_{EquipType.Legs}", EquipType.Legs, this, $"{Name}{variation}");
    }

    private void SetupEquipTextures(string variation = "", bool head = true, bool body = true, bool legs = true)
    {
        if (head)
        {
            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, $"{Name}{variation}", EquipType.Head);
            if (equipSlotHead != -1) ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
        }
        if (body)
        {
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, $"{Name}{variation}", EquipType.Body);
            if (equipSlotBody != -1)
            {
                ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
                ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            }
        }
        if (legs)
        {
            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, $"{Name}{variation}", EquipType.Legs);
            if (equipSlotLegs != -1) ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }
    }

    public override void Load()
    {
        if (Main.netMode == NetmodeID.Server) return;

        LoadEquipTextures();
        LoadEquipTextures("GroundPound", false, false);
        LoadEquipTextures("Flying");
        LoadEquipTextures("Statue", body: false, legs: false);

        EquipLoader.AddEquipTexture(Mod, $"{Texture}Tail_{EquipType.Waist}", EquipType.Waist, this, $"{Name}Tail");
        EquipLoader.AddEquipTexture(Mod, $"{Texture}TailFlying_{EquipType.Waist}", EquipType.Waist, this, $"{Name}TailFlying");
    }

    public override void SetStaticDefaults()
    {
        if (Main.netMode == NetmodeID.Server) return;

        SetupEquipTextures();
        SetupEquipTextures("GroundPound", false, false);
        SetupEquipTextures("Flying", false);
        SetupEquipTextures("Statue", body: false, legs: false);
    }

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 20;
        Item.accessory = true;

        Item.GetGlobalItemOrNull<GearSlotGlobalItem>()?.gearType = GearType.Cap;
    }

    public override bool? PrefixChance(int pre, UnifiedRandom rand) => !(pre == -1 || pre == -3);

    public override bool CanEquipAccessory(Player player, int slot, bool modded) => modded;

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        CapEffectsPlayer? capEffectsPlayer = player.GetModPlayerOrNull<CapEffectsPlayer>();

        player.GetModPlayerOrNull<CapEffectsPlayer>()?.currentCap = Name;

        if (capEffectsPlayer == null) return;

        if (Name == "Luigi" && !player.IsOnGroundPrecise()) player.gravity = 0.2f + (capEffectsPlayer.currentJump is Jump.Double or Jump.Triple ? 0.05f : 0);

        if (!capEffectsPlayer.GroundPounding && capEffectsPlayer.currentPowerupType != ModContent.GetInstance<FrogSuit>().Type) player.spikedBoots = 1;

        capEffectsPlayer.CurrentPowerup?.UpdateConsumed(player);

        if (!player.GetModPlayerOrNull<BroInfoPlayer>()?.ShowBroInfo ?? true) return;

        if (capEffectsPlayer.slippyTimer > 0 && player.IsOnGroundPrecise()) player.slippy = true;

        player.lifeRegen = 0;
        player.lifeRegenTime = 0;
        if (!capEffectsPlayer?.GroundPounding ?? true) player.accFlipper = true;
        player.noKnockback = true;
        player.statDefense += capEffectsPlayer?.StatDefense ?? 0;
        player.statLifeMax2 += capEffectsPlayer?.StatHP ?? 0;
    }

    public override void UpdateVisibleAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayerOrNull<CapEffectsPlayer>()?.currentCapToDraw = Name;
    }
}

internal class Mario : CapItem { }
internal class Luigi : CapItem { }