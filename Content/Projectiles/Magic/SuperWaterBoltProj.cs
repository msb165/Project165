using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Project165.Content.Dusts;
using Terraria.ID;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace Project165.Content.Projectiles.Magic
{
    public class SuperWaterBoltProj : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.WaterBolt}";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 60;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new(8);
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 800;
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
                    spinPoint = spinPoint.RotatedBy(Projectile.velocity.ToRotation());
                    Dust testDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Blue, 0, 0, 0, Color.White with { A = 127 }, 0.75f);
                    testDust.noGravity = true;
                    testDust.position = Projectile.Center + spinPoint;
                    testDust.velocity = spinPoint.SafeNormalize(Vector2.UnitY) * 0.5f;
                }
                Projectile.localAI[0] = 1f;
            }

            Projectile.ai[0]++;

            for (int i = 0; i < 3; i++)
            {
                Dust testDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Blue, 0, 0, 0, Color.White with { A = 127 }, 0.75f);
                testDust.noGravity = true;
                testDust.velocity *= 0.2f;
                testDust.velocity += Projectile.velocity * 0.1f;
                testDust.position = Projectile.Center.RotatedByRandom(MathHelper.ToRadians(0.01f)) - Projectile.velocity / 5f * i;

                Dust testDust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Blue, 0, 0, 0, Color.White with { A = 127 }, 1f);
                testDust2.noGravity = true;
                testDust2.velocity = Projectile.velocity;
                testDust2.position = Projectile.Center.RotatedByRandom(MathHelper.ToRadians(0.0125f)) - Projectile.velocity / 5f * i;

                Dust testDust3 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Blue, Scale: 1.25f);
                testDust3.noGravity = true;
                testDust3.velocity *= 0f;
                testDust3.position = Projectile.Center.RotatedByRandom(MathHelper.ToRadians(0.002f)) - Projectile.velocity / 2.5f * i;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            Projectile.penetrate--;

            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Blue, 0, 0, 0, Color.White with { A = 127 }, 0.75f);
                dust.noGravity = true;
                dust.velocity *= 4f;
            }

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

            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Blue, 0, 0, 0, Color.White with { A = 127 }, 0.75f);
                dust.noGravity = true;
                dust.velocity *= 4f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(ProjectileID.StardustTowerMark);
            Texture2D texture = TextureAssets.Projectile[ProjectileID.StardustTowerMark].Value;
            Vector2 origin = texture.Size() / 2;
            Color trailColor = new(0, 60, 180, 0);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 trailPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                trailColor *= 0.99f;
                Main.EntitySpriteDraw(texture, trailPos, null, trailColor * 0.25f, Projectile.rotation, origin, MathHelper.SmoothStep(Projectile.scale * 0.4f, 0f, 0.02f * i), SpriteEffects.None, 1);
                Main.EntitySpriteDraw(texture, trailPos, null, trailColor * 0.075f, Projectile.rotation, origin, MathHelper.SmoothStep(Projectile.scale * 0.8f, 0f, 0.02f * i), SpriteEffects.None, 1);
            }

            return false;
        }
    }
}
