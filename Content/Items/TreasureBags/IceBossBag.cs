using Project165.Content.Items.Weapons.Magic;
using Project165.Content.Items.Weapons.Melee;
using Project165.Content.Items.Weapons.Ranged;
using Project165.Content.NPCs.Bosses.Frigus;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.TreasureBags
{
    public class IceBossBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.BossBag[Type] = true;
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
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Avalanche>(), 3));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<IceShotgun>(), 3));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<IceChakram>(), 3));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<IceBossFly>()));
        }
    }
}
