using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Project165.Content.Items.Materials;

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

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ShadowGel>(), 20)
                .AddIngredient(ItemID.LihzahrdPowerCell, 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
