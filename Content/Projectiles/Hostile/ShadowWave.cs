using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Hostile
{
    public class ShadowWave : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.DD2SquireSonicBoom}";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 2;
            ProjectileID.Sets.TrailingMode[Type] = 20;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.height = 80;
            Projectile.width = 22;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage() => Projectile.alpha < 127;

        public override void AI()
        {
            if (Projectile.timeLeft < 300)
            {
                Projectile.velocity *= 0.98f;
            }
            if (Projectile.alpha < 255)
            {
                Projectile.alpha += 2;
            }
            if (Projectile.alpha > 255)
            {
                Projectile.alpha = 255;
                Projectile.Kill();
            }
        }
    }
}
