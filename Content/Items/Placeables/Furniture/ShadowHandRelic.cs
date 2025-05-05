using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Placeables.Furniture
{
    public class ShadowHandRelic : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.ShadowHandRelic>());
            Item.rare = ItemRarityID.Master;
            Item.master = true;
            Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(gold: 5));
            Item.maxStack = Item.CommonMaxStack;
        }
    }
}
