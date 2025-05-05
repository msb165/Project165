using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Dusts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Ranged
{
    public class HallowedJavelinProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new(16);
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Projectile.ai[0] = 1f;
            }

            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.velocity.ToRotation() + MathHelper.PiOver2, 0.4f);
            GenerateDust();
            Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3());
            if (++Projectile.ai[0] < 32f)
            {
                return;
            }

            if ((Projectile.velocity.Y += 0.2f) > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }

        private void GenerateDust()
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 dustPosition = Projectile.Center + Vector2.Normalize(Projectile.velocity) * 8f;

                Dust iceDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SandSpray);
                iceDust.scale = 0.75f;
                iceDust.position = dustPosition - Projectile.velocity / 2.5f * i;
                iceDust.velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2) * 0.2f + Projectile.velocity / 4f;
                iceDust.noGravity = true;

                Dust iceDust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SandSpray);
                iceDust2.scale = 0.75f;
                iceDust2.position = dustPosition - Projectile.velocity / 2.5f * i;
                iceDust2.velocity = Projectile.velocity.RotatedBy(-MathHelper.PiOver2) * 0.2f + Projectile.velocity / 4f;
                iceDust2.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Bleeding, 900);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            Vector2 dustPos = Projectile.position;
            Vector2 projRotation = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
            dustPos += projRotation * 16f;
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(dustPos, Projectile.width, Projectile.height, DustID.SandSpray, 0, 0);
                dust.position = (dust.position + Projectile.Center) / 2f;
                dust.velocity += projRotation;
                dust.velocity *= 0.5f;
                dust.noGravity = true;
                dustPos -= projRotation * 8f;
            }

            if (Main.rand.NextBool(4))
            {
                Projectile.Resize(160, 160);
                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SandSpray, 0, 0);
                    dust.velocity *= 0.6f;
                    dust.noGravity = true;
                }
                Projectile.Damage();
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.instance.LoadProjectile(ProjectileID.DD2BetsyFireball);
            Texture2D glowTexture = TextureAssets.Projectile[ProjectileID.DD2BetsyFireball].Value;
            Vector2 drawOrigin = Vector2.UnitX * texture.Width / 2;
            Vector2 drawOriginGlow = Vector2.UnitX * glowTexture.Width / 2;
            Color drawColor = Projectile.GetAlpha(lightColor);
            Color trailColor = Color.Yellow with { A = 0 } * Projectile.Opacity;
            float scale = Projectile.scale * 0.75f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                trailColor *= 0.75f;
                float rotation = Projectile.oldRot[i];
                if (i > 0)
                {
                    rotation += MathHelper.PiOver2;
                }
                Vector2 trailPos = Projectile.oldPos[i] + Projectile.Size / 2 - Projectile.velocity / 5f * i - Main.screenPosition;
                if ((scale *= 1.25f) > 2f)
                {
                    scale = 2f;
                }

                Main.spriteBatch.Draw(texture, trailPos, null, trailColor, rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(glowTexture, trailPos, null, trailColor * 0.06f, Projectile.rotation, drawOriginGlow, scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
