using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Common.Globals.Projectiles
{
    public class GlobalProjectileTweaks : GlobalProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[ProjectileID.InfluxWaver] = 20;
            ProjectileID.Sets.TrailingMode[ProjectileID.InfluxWaver] = 2;
        }

        public override void SetDefaults(Projectile entity)
        {
            if (entity.type == ProjectileID.RubyBolt)
            {
                entity.usesLocalNPCImmunity = true;
                entity.localNPCHitCooldown = 25;
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.type == ProjectileID.RainbowFront || projectile.type == ProjectileID.RainbowBack)
            {
                target.AddBuff(BuffID.OnFire, 300);
            }
        }

        public override bool PreAI(Projectile projectile)
        {
            switch (projectile.type)
            {
                case ProjectileID.FrostBoltStaff:
                    FrostBoltAI(projectile);
                    return false;
                case ProjectileID.EmeraldBolt:
                case ProjectileID.RubyBolt:
                case ProjectileID.AmberBolt:
                case ProjectileID.AmethystBolt:
                case ProjectileID.SapphireBolt:
                case ProjectileID.DiamondBolt:
                case ProjectileID.TopazBolt:
                    GemBoltAI(projectile);
                    return false;
            }
            return true;
        }

        #region Projectile AI

        public void GemBoltAI(Projectile projectile)
        {
            short dustType = projectile.type switch
            {
                ProjectileID.SapphireBolt => DustID.GemSapphire,
                ProjectileID.RubyBolt => DustID.GemRuby,
                ProjectileID.TopazBolt => DustID.GemTopaz,
                ProjectileID.EmeraldBolt => DustID.GemEmerald,
                ProjectileID.AmethystBolt => DustID.GemAmethyst,
                ProjectileID.DiamondBolt => DustID.GemDiamond,
                ProjectileID.AmberBolt => DustID.AmberBolt,
                _ => DustID.SpectreStaff,
            };

            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                for (int i = 0; i < 16; i++)
                {
                    Vector2 spinPoint = (Vector2.UnitY).RotatedBy(i * MathHelper.TwoPi / 16f) * new Vector2(1f, 4f);
                    spinPoint = spinPoint.RotatedBy(projectile.velocity.ToRotation());
                    Dust dust = Dust.NewDustPerfect(projectile.position, dustType, Scale: 1.25f);
                    dust.noGravity = true;
                    dust.position = projectile.Center + spinPoint * 4f;
                    dust.velocity = spinPoint.SafeNormalize(Vector2.UnitY);
                }
            }

            for (int i = 0; i < 6; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, dustType, Scale: 1.25f);
                dust.noGravity = true;
                dust.position = projectile.Center - projectile.velocity / 10f * i;
                dust.velocity *= 0.3f;
            }
        }

        public void FrostBoltAI(Projectile projectile)
        {
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                for (int i = 0; i < 16; i++)
                {
                    Vector2 spinPoint = Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / 16f) * new Vector2(2f, 8f);
                    spinPoint = spinPoint.RotatedBy(projectile.velocity.ToRotation());
                    Dust dust = Dust.NewDustPerfect(projectile.position, DustID.Frost, Scale: 1f);
                    dust.noGravity = true;
                    dust.position = projectile.Center + spinPoint - projectile.velocity / 2;
                    dust.velocity = spinPoint.SafeNormalize(Vector2.UnitY) * 2f;
                }
            }

            for (int i = 0; i < 10; i++)
            {
                Vector2 newPos = projectile.Center - projectile.velocity / 10f * i;
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Frost, Alpha: 50, newColor: Color.Cyan, Scale: 1.25f);
                dust.noGravity = true;
                dust.position = newPos;
                dust.velocity *= 0.3f;
                Dust dust2 = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.SpectreStaff, Alpha: 50, newColor: Color.Cyan, Scale: 1.5f);
                dust2.noGravity = true;
                dust2.position = newPos;
                dust2.velocity *= 0.6f;
            }
        }
        #endregion

        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            if (projectile.type == ProjectileID.DiamondBolt)
            {
                projectile.Resize(32, 32);
                for (int i = 0; i < 30; i++)
                {
                    Dust dust2 = Dust.NewDustPerfect(projectile.position, DustID.GemDiamond, Scale: 1.25f);
                    dust2.noGravity = true;
                    dust2.position = projectile.Center + Utils.RandomVector2(Main.rand, -10, 11);
                    dust2.velocity = Vector2.Normalize(Utils.RandomVector2(Main.rand, -20, 21)) * Main.rand.NextFloat(3, 9);
                }
                projectile.Damage();
                SoundEngine.PlaySound(SoundID.Item10, projectile.position);
                return false;
            }
            return true;
        }

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (projectile.type == ProjectileID.InfluxWaver)
            {
                Color drawColor = projectile.GetAlpha(lightColor);
                Color trailColor = drawColor;
                float scale = projectile.scale;
                Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
                Vector2 origin = Vector2.UnitX * texture.Width;
                //float rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;

                for (int i = 0; i < projectile.oldPos.Length; i++)
                {
                    trailColor *= 0.75f;
                    scale *= 1.03f;
                    Main.EntitySpriteDraw(texture, projectile.oldPos[i] + projectile.Size / 2 - Main.screenPosition, null, trailColor, projectile.oldRot[i], origin, scale, SpriteEffects.None);
                }

                Main.EntitySpriteDraw(texture, projectile.Center - Main.screenPosition, null, drawColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None);
                return false;
            }
            return true;
        }
    }
}
