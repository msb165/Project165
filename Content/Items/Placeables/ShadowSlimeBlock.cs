using Project165.Content.Items.Materials;
using Project165.Content.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Placeables
{
    public class ShadowSlimeBlock : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.ShadowSlimeBlock>());
            Item.Size = new(16);
            Item.value = 750;
            Item.ResearchUnlockCount = 100;
        }

        public override void AddRecipes()
        {
            CreateRecipe(50)
                .AddIngredient(ItemID.SlimeBlock, 50)
                .AddIngredient(ModContent.ItemType<ShadowEssence>(), 5)
                .AddTile(TileID.Solidifier)
                .Register();
            CreateRecipe(50)
                .AddIngredient(ItemID.PinkSlimeBlock, 50)
                .AddIngredient(ModContent.ItemType<ShadowEssence>(), 5)
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }
}
