using Terraria;
using Terraria.ModLoader;

namespace Project165.Content.Tiles
{
    public class ShadowSlimeWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            Main.wallLight[Type] = true;
        }
    }
}
