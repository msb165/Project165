using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.TreasureBags
{
    internal class IceBossBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.BossBag[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Pink;
        }
    }
}
