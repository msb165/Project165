using Microsoft.Build.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Ranged
{
    public class IceGuardianBolt : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.FrostBoltStaff}";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.Size = new(10);
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.alpha = 0;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item28 with { Volume = 0.3f }, Projectile.Center);
                Projectile.localAI[0] = 1f;
                for (int i = 0; i < 15; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, 0, 0, Scale: 1f);
                    dust.noGravity = true;
                    dust.velocity *= 2f;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity / 5f * i, DustID.Frost, Vector2.Zero, Scale: 1f);
                dust.scale = 1f;
                dust.noGravity = true;

                Dust dust2 = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity / 5f * i, DustID.SpectreStaff, Vector2.Zero, Scale: 1f);
                dust2.scale = 1f;
                dust2.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, 0, 0, Scale: 1f);
                dust.noGravity = true;
                dust.velocity *= 4f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(ProjectileID.HallowBossLastingRainbow);
            Texture2D texture = TextureAssets.Projectile[ProjectileID.HallowBossLastingRainbow].Value;
            int height = texture.Height / Main.projFrames[Type];
            int frameHeight = height * Projectile.frame;
            Rectangle sourceRectangle = new(0, frameHeight, texture.Width / 2, height);
            sourceRectangle.X += sourceRectangle.Width;
            Vector2 drawOrigin = sourceRectangle.Size() / 2;
            Color drawColor = new Color(100, 255, 255) with { A = 40 } * Projectile.Opacity;
            Color trailColor = drawColor;
            float trailScale = 0.4f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (trailScale < 2f)
                {
                    trailScale *= 1.125f;
                }

                trailColor *= 0.7f;
                Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition - Projectile.velocity / 10f * i, sourceRectangle, trailColor * 0.4f, Projectile.rotation, drawOrigin, trailScale, SpriteEffects.None, 0f);
            }

            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, drawColor * 0.125f, Projectile.rotation, drawOrigin, 1.25f, SpriteEffects.None, 0f);
            return false;
        }
    }
}
