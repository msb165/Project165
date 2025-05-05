using Terraria.Audio;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Hostile
{
    internal class SnowBallHostile : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.SnowBallFriendly}";

        public override void SetDefaults()
        {
            Projectile.Size = new(14);
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.coldDamage = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {            
            Projectile.rotation += 0.2f * Projectile.direction;
            for (int i = 0; i < 3; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Snow, Alpha: 100);
                dust.velocity *= 0.1f;
                dust.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item51, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                Dust snowDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Snow);
                snowDust.noGravity = true;
                Dust dust2 = snowDust;
                dust2.velocity -= Projectile.oldVelocity * 0.25f;
            }
        }
    }
}
