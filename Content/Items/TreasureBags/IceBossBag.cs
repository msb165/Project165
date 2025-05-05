using Project165.Content.Items.Accessories;
using Project165.Content.Items.Weapons.Magic;
using Project165.Content.Items.Weapons.Melee;
using Project165.Content.Items.Weapons.Ranged;
using Project165.Content.Items.Weapons.Summon;
using Project165.Content.NPCs.Bosses.Frigus;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.TreasureBags;

public class IceBossBag : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.BossBag[Type] = true;
        Item.ResearchUnlockCount = 3;
    }
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.Size = new(24);
        Item.rare = ItemRarityID.Pink;
        Item.expert = true;
    }

    public override bool CanRightClick() => true;

    public override void ModifyItemLoot(ItemLoot itemLoot)
    {
        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Avalanche>(), 6));
        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<IceShotgun>(), 6));
        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<IceChakram>(), 6));
        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<IceBat>(), 6));
        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<IceGuardianStaff>(), 6));
        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<IceCloak>(), 6));

        itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<IceBossFly>()));
    }
}
