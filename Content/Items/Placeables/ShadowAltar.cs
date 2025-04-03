using Terraria;
using Terraria.ModLoader;

namespace Project165.Content.Items.Placeables
{
    public class ShadowAltar : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.ShadowAltar>());
            Item.width = 48;
            Item.height = 32;
        }
    }
}
