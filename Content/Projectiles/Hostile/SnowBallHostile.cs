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
            Projectile.rotation += 0.1f * Projectile.direction;
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
