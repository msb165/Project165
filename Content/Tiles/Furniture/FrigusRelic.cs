using Microsoft.Xna.Framework.Graphics;
using Project165.Utilites;
using Terraria;
using Terraria.ModLoader;

namespace Project165.Content.Tiles.Furniture
{
    public class FrigusRelic : BaseRelic
    {
        public override string RelicTextureName => Project165Utils.ContentPath + "Tiles/Furniture/FrigusRelic";

        public override void Load()
        {
            if (!Main.dedServ)
            {
                RelicTexture = ModContent.Request<Texture2D>(RelicTextureName);
            }
        }
    }
}
