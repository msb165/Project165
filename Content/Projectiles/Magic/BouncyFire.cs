using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Dusts;
using Project165.Utilites;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Magic
{
    public class BouncyFire : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.Size = new(30);
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1000;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3());

            if (Projectile.ai[1] == 0f)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork, Scale: 2f);
                    dust.noGravity = true;
                    dust.position = Projectile.Center;
                    dust.velocity *= 4f;
                }
                Projectile.ai[1] = 1f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 80f)
            {
                Projectile.ai[0] = 0f;
                Projectile.velocity.Y = -30f;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<BigMagicExplosion>(), Projectile.damage, 0f, Projectile.owner);
                }
            }

            Projectile.velocity.Y += 0.8f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = texture.Size() / 2;
            Vector2 projSize = Projectile.Size / 2;
            float offset = MathF.Sin((float)Main.timeForVisualEffects * MathHelper.TwoPi / 30f);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 drawPosTrail = Projectile.oldPos[i] + projSize;
                Color trailColor = Color.Lerp(Color.Orange with { A = 40 }, Color.OrangeRed with { A = 40 }, i * 0.125f) * (1f - 0.0625f * i);
                Main.EntitySpriteDraw(texture, drawPosTrail - Main.screenPosition, null, trailColor * 0.3f, Projectile.oldRot[i], drawOrigin, Projectile.scale * 1.5f, SpriteEffects.None, 0);

                // "Glow" effect
                Main.EntitySpriteDraw(texture, drawPosTrail - Main.screenPosition - Vector2.UnitY.RotatedBy(MathHelper.Pi * i) * (4f + 1f * offset), null, trailColor * 0.15f, Projectile.oldRot[i], drawOrigin, Projectile.scale * 1.5f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture, drawPosTrail - Main.screenPosition + Vector2.UnitY.RotatedBy(MathHelper.Pi * i) * (4f + 1f * offset), null, trailColor * 0.15f, Projectile.oldRot[i], drawOrigin, Projectile.scale * 1.5f, SpriteEffects.None, 0);

                Main.EntitySpriteDraw(texture, drawPosTrail - Main.screenPosition, null, trailColor, Projectile.oldRot[i], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }

    }
}
