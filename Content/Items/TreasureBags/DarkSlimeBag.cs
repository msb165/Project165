using Project165.Content.Items.Materials;
using Project165.Content.Items.Weapons.Magic;
using Project165.Content.Items.Weapons.Melee;
using Project165.Content.Items.Weapons.Summon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.TreasureBags
{
    public class DarkSlimeBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.BossBag[Type] = true;
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 32;
            Item.expert = true;
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
        }

        public override bool CanRightClick() => true;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadowBlade>(), 3));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadowPike>(), 3));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadowSlimeStaff>(), 3));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadowEssence>(), 1, 8, 30));
        }
    }
}
