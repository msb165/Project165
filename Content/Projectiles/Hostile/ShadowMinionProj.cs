using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Hostile
{
    public class ShadowMinionProj : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.GemHookRuby}";
        public override void SetDefaults()
        {
            Projectile.Size = new(10);
            Projectile.alpha = 255;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                for (int i = 0; i < 25; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptSpray);
                    dust.noGravity = true;
                    dust.velocity *= 2f;
                }
                Projectile.ai[0] = 1f;
            }

            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptSpray);
                dust.noGravity = true;
                dust.position = Projectile.Center - Projectile.velocity / 10f * i;
                dust.velocity *= 0.3f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptSpray);
                dust.noGravity = true;
                dust.velocity *= 4f;
            }
        }
    }
}
