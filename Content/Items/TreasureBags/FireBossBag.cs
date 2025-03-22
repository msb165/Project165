using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Project165.Content.Items.Weapons.Melee;
using Project165.Content.Items.Weapons.Ranged;
using Project165.Content.Items.Weapons.Magic;

namespace Project165.Content.Items.TreasureBags
{
    public class FireBossBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.BossBag[Type] = true;
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 32;
            Item.expert = true;
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
        }

        public override bool CanRightClick() => true;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<UltraFireStaff>(), 3));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<InfernalBow>(), 3));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SuperFireSword>(), 3));
        }
    }
}
