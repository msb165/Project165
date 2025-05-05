using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Magic
{
    public class IceFlaskBolt : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.FrostBeam}";

        public override void SetDefaults()
        {
            Projectile.Size = new(8);
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 40;
        }

        public override void AI()
        {
            for (int i = 0; i < 3; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.FrostStaff);
                dust.position = Projectile.Center - Projectile.velocity / 5f * i;
                dust.velocity *= 0.3f;
                dust.noGravity = true;

                Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SpectreStaff);
                dust2.position = Projectile.Center;
                dust2.scale = 1.5f;
                dust2.velocity *= 0.3f;
                dust2.noGravity = true;
            }

            Projectile.velocity.X *= 0.98f;
            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 8f)
            {
                Projectile.velocity.Y = 8f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.FrostStaff);
                dust.velocity *= 1.5f;
                dust.noGravity = true;
            }
        }
    }
}
