using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    internal class FrigusMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 22;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 75);
            Item.vanity = true;
        }        
    }
}
