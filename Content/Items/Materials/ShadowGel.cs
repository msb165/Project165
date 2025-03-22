using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Materials
{
    public class ShadowGel : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.Size = new(16);
            Item.rare = ItemRarityID.Yellow;
            Item.maxStack = Item.CommonMaxStack;
            Item.alpha = 100;
            Item.value = Item.buyPrice(silver:2);
        }
    }
}
