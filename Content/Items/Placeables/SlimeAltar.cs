using Project165.Content.Tiles;
using Terraria.ModLoader;

namespace Project165.Content.Items.Placeables
{
    public class SlimeAltar : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.SlimeAltar>());
            Item.width = 48;
            Item.height = 32;
            Item.value = 150;
        }
    }
}
