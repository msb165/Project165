using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Ranged
{
    public class IceShard : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.CrystalShard}";

        public override void SetDefaults()
        {
            Projectile.Size = new(8);
            Projectile.scale = 1.7f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile.rotation += 0.3f * Projectile.direction;
            Projectile.scale -= 0.1f;
            if (Projectile.scale <= 0.1f)
            {
                Projectile.Kill();
            }
        }
    }
}
