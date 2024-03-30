using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Project165.Content.Items.SummonItems
{
    internal class ShadowSlimeSummon : ModItem
    {
        public override void SetDefaults()
        {
            Item.Size = new(40);
            Item.rare = ItemRarityID.Pink;
            Item.consumable = false;
            Item.maxStack = 1;
        }
    }
}
