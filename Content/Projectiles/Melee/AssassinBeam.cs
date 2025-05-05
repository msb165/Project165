using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Dusts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Melee
{
    public class AssassinBeam : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.PurificationPowder}";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new(12);
            Projectile.ignoreWater = true;
            Projectile.penetrate = 2;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 17;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 50;
        }

        public float VerticalSpeed => Projectile.ai[0];

        public override void AI()
        {
            Projectile.velocity.Y += VerticalSpeed * 0.025f;
            Projectile.velocity.Y = MathHelper.Clamp(Projectile.velocity.Y, -8, 8f);
            for (int i = 0; i < 3; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.position, ModContent.DustType<GlowDust>(), Vector2.Zero, 200, new Color(250, 250, 250), Scale: 1f);
                dust.scale = 1f;
                dust.position = Projectile.Center - Projectile.velocity / 5f * i;
                dust.noGravity = true;

                Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Red, 0, 0, 0, Scale: 0.6f);
                dust2.scale = 0.6f;
                dust2.velocity *= 0.75f;
                dust2.position = Projectile.Center - Projectile.velocity / 5f * i;
                dust2.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Red, 0, 0, 0, Scale: 0.6f);
                dust2.scale = 0.6f;
                dust2.velocity *= 2f;
                dust2.noGravity = true;
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
            Color drawColor = new Color(255, 40, 40) with { A = 40 };
            Color trailColor = drawColor;
            float trailScale = 0.5f;
            float rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (trailScale < 2f)
                {
                    trailScale *= 1.125f;
                }

                trailColor *= 0.75f;
                Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, sourceRectangle, trailColor, rotation, drawOrigin, trailScale, SpriteEffects.None, 0f);
            }

            return false;
        }
    }
}
