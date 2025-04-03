using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
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
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption, Scale: 2f, Alpha: 100);
                    dust.noGravity = true;
                    dust.velocity *= 2f;
                }

                SoundEngine.PlaySound(SoundID.Item8, Projectile.position);

                Projectile.ai[0] = 1f;
            }

            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption, Alpha: 100);
                dust.velocity = dust.velocity * 0.3f + Projectile.velocity * 0.2f;
                dust.noGravity = true;
                dust.position = Projectile.Center - Projectile.velocity / 10f * i;

                Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption, Alpha: 200, Scale: 1.5f);
                dust2.scale = 1.5f;
                dust2.velocity *= 0f;
                dust2.noGravity = true;
                dust2.position = Projectile.Center.RotatedByRandom(MathHelper.ToRadians(0.01f)) - Projectile.velocity / 10f * i;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption, Alpha: 100, Scale: 2f);
                dust.noGravity = true;
                dust.velocity *= 4f;
            }
        }
    }
}
