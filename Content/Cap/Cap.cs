using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Utilities;
using TerrariaXMario.Utilities.AssetData;

namespace TerrariaXMario.Content.Cap;

internal abstract class CapItem : ModItem
{
    public override void Load()
    {
        if (Main.netMode == NetmodeID.Server) return;

        EquipSet.Default.AddEquipTextures(this);
        EquipSet.GroundPound.AddEquipTextures(this);
        EquipSet.Flying.AddEquipTextures(this);
    }

    public override void SetStaticDefaults()
    {
        TerrariaXMarioItemSets.GearAccessoryItem[Type] = true;

        if (Main.netMode == NetmodeID.Server) return;

        EquipSet.Default.SetupEquipTextures(this);
        EquipSet.GroundPound.SetupEquipTextures(this);
        EquipSet.Flying.SetupEquipTextures(this);
    }

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 20;
        Item.accessory = true;
    }

    public override bool? PrefixChance(int pre, UnifiedRandom rand) => !(pre == -1 || pre == -3);

    public override bool CanEquipAccessory(Player player, int slot, bool modded) => modded;

    internal void Update(Player player)
    {
        player.CapPlayer.currentCap = Name;

        if (!player.GroundPoundPlayer.IsGroundPounding)
        {
            player.spikedBoots = 1;
            player.accFlipper = true;
        }
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        Update(player);
    }

    public override void UpdateVisibleAccessory(Player player, bool hideVisual)
    {
        Update(player);
    }
}

[CanBeReadBySourceGenerators]
internal partial class CapPlayer : ModPlayer
{
    internal static Dictionary<string, Color> CapColors = new() {
        { "Mario", new Color(217, 22, 22) },
        { "Luigi", new Color(27, 149, 4) }
    };

    [NetSync] internal string oldCap = "";
    [NetSync] internal string currentCap = "";
    [NetSync] internal string currentVariation = "";
    internal Color? CurrentCapColor => currentCap == "" ? null : CapColors[currentCap];

    internal bool Enabled => currentCap != "";

    internal static CapAudioData CapAudio(string cap) => Assets.CapAudio[cap];
    internal CapAudioData CurrentCapAudio => Assets.CapAudio[currentCap];

    internal static void ResetVariation(Player player)
    {
        player.CapPlayer.currentVariation = EquipSet.Default.Name;
    }

    public override void ResetEffects()
    {
        currentCap = "";
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;

        if (!player.CapPlayer.Enabled) return;

        if (player.CapPlayer.currentVariation != EquipSet.Flying.Name && !player.IsOnGroundPrecise && ((player.legFrame.Y / player.legFrame.Height) is 10 or 11 or 12 or 13 or 17 or 18 or 19 or 20)) player.legPosition.Y = player.gravDir == 1 ? -2 : 8;
    }

    public override void FrameEffects()
    {
        if (!Enabled) return;

        foreach (EquipType equipType in Enum.GetValues(typeof(EquipType)))
        {
            int equipSlot = EquipSet.GetEquipSlot(currentCap, currentVariation, equipType);
            if (equipSlot == -1) equipSlot = EquipSet.GetEquipSlot(currentCap, "", equipType);

            switch (equipType)
            {
                case EquipType.Head:
                    Player.head = equipSlot;
                    break;
                case EquipType.Body:
                    Player.body = equipSlot;
                    break;
                case EquipType.Legs:
                    Player.legs = equipSlot;
                    break;
                case EquipType.HandsOn:
                    Player.handon = equipSlot;
                    break;
                case EquipType.HandsOff:
                    Player.handoff = equipSlot;
                    break;
                case EquipType.Back:
                    Player.back = equipSlot;
                    break;
                case EquipType.Front:
                    Player.front = equipSlot;
                    break;
                case EquipType.Shoes:
                    Player.shoe = equipSlot;
                    break;
                case EquipType.Waist:
                    Player.waist = equipSlot;
                    break;
                case EquipType.Wings:
                    Player.wings = equipSlot;
                    break;
                case EquipType.Shield:
                    Player.shield = equipSlot;
                    break;
                case EquipType.Neck:
                    Player.neck = equipSlot;
                    break;
                case EquipType.Face:
                    Player.face = equipSlot;
                    break;
                case EquipType.Balloon:
                    Player.balloon = equipSlot;
                    break;
                case EquipType.Beard:
                    Player.beard = equipSlot;
                    break;
                default:
                    break;
            }
        }
    }

    public override void PostUpdateEquips()
    {
        if (oldCap != currentCap)
        {
            if (currentCap == "") CapAudio(currentCap == "" ? oldCap : currentCap).Unequip.PlayRandom(Player.MountedCenter);
            else CurrentCapAudio.Equip.PlayRandom(Player.MountedCenter);
        }

        oldCap = currentCap;
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        if (!Enabled) return;

        modifiers.DisableSound();
        CurrentCapAudio.Hurt.PlayRandom(Player.MountedCenter);
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (!Enabled) return;

        if (PlayerInput.Triggers.JustPressed.Jump && !Player.mount.Active && (Player.IsOnGroundPrecise || Player.wet))
        {
            if (Player.wet) Assets.Swim.Play(Player.MountedCenter);
            else Assets.Jump.Play(Player.MountedCenter);
        }
    }
}