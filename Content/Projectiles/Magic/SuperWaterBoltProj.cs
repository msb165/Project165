using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Project165.Content.Dusts;
using Terraria.ID;
using Terraria.Audio;

namespace Project165.Content.Projectiles.Magic
{
    public class SuperWaterBoltProj : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.WaterBolt}"; 
        public override void SetDefaults()
        {
            Projectile.Size = new(12);
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 10;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                int dustAmount = 24;
                for (int i = 0; i < dustAmount; i++)
                {
                    Vector2 spinPoint = Vector2.Zero;
                    spinPoint -= Vector2.UnitY.RotatedBy(-i * MathHelper.TwoPi / dustAmount, Vector2.Zero) * new Vector2(4f, 16f);
                    Dust testDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Blue, 0, 0, 0, Color.White with { A = 127 }, 0.75f);
                    testDust.noGravity = true;
                    testDust.position = Projectile.Center + spinPoint;
                    testDust.velocity = spinPoint.SafeNormalize(Vector2.UnitY) * 0.5f;
                }
                Projectile.localAI[0] = 1f;
            }

            Projectile.ai[0]++;
            Projectile.tileCollide = Projectile.ai[0] < 20f;

            for (int i = 0; i < 10; i++)
            {
                Dust testDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Blue, 0, 0, 0, Color.White with { A = 127 }, 0.75f);
                testDust.noGravity = true;
                testDust.velocity *= 0.2f;
                testDust.velocity += Projectile.velocity * 0.1f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            Projectile.penetrate--;
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
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Blue, 0, 0, 0, Color.White with { A = 127 }, 0.75f);
            }
        }
    }
}
