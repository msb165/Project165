using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Magic
{
    public class UltraStar : ModProjectile
    {
        public override void SetStaticDefaults()
        {            

        }

        public override void SetDefaults()
        {
            Projectile.Size = new(16);
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
        }
    }
}
