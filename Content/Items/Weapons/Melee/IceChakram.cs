using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Melee
{
    public class IceChakram : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 46;
            Item.damage = 65;
            Item.knockBack = 8f;
            Item.useTime = 14;
            Item.useAnimation = 18;
            Item.DamageType = DamageClass.Melee;
        }
    }
}
