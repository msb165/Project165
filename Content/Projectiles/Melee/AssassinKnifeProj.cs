using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Items.Placeables;
using Project165.Content.Items.Weapons.Melee;
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
    public class AssassinKnifeProj : ModProjectile
    {
        public override string Texture => ModContent.GetInstance<AssassinKnife>().Texture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new(16);
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.hide = true;
            Projectile.scale = 1.25f;
            Projectile.extraUpdates = 1;
        }

        Player Player => Main.player[Projectile.owner];
        public float AITimer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {
            AITimer++;

            Projectile.scale = 0.25f + Utils.GetLerpValue(0f, 7f, AITimer, clamped: true) * Utils.GetLerpValue(16f, 12f, AITimer, clamped: true);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - MathHelper.PiOver4;

            Projectile.spriteDirection = Player.direction;

            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation -= MathHelper.PiOver2;
            }

            Lighting.AddLight(Projectile.Center + Projectile.velocity, Color.Red.ToVector3());
            Projectile.Center = Player.RotatedRelativePoint(Player.MountedCenter) + Projectile.velocity * (AITimer - 1f);
            Projectile.velocity = Projectile.velocity.RotatedBy(MathF.Cos(AITimer * MathHelper.PiOver4 / 60f) * -0.02f * Player.direction);

            if (AITimer >= 16f)
            {
                Projectile.Kill();
            }
            else
            {
                Player.heldProj = Projectile.whoAmI;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Player.HeldItem.ModItem is AssassinKnife knife)
            {
                if (knife.projAmount < 3 && hit.Crit)
                {
                    knife.projAmount++;
                }
            }

            target.AddBuff(BuffID.Bleeding, 300);
            target.bloodButchered = true;
            for (int i = 0; i < 6; i++)
            {
                Vector2 spawnPos = Main.rand.NextVector2CircularEdge(50f, 50f);
                Vector2 newVel = spawnPos.SafeNormalize(Vector2.UnitY) * 8f;
                Dust dust = Dust.NewDustPerfect(target.Center - spawnPos, DustID.LifeDrain, newVel);
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.instance.LoadProjectile(ProjectileID.HallowBossLastingRainbow);
            Texture2D glowTexture = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Vector2 drawOrigin = texture.Size() / 2;
            Vector2 drawOriginGlow = glowTexture.Size() / 2;
            Color drawColor = Projectile.GetAlpha(lightColor);
            Color glowColor = Color.Red with { A = 0 } * Projectile.Opacity;
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            float rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + Projectile.velocity * 4f, null, glowColor, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0f);
            Main.spriteBatch.Draw(glowTexture, Projectile.Center - Main.screenPosition + Projectile.velocity * 4f, null, glowColor * 0.5f, rotation, drawOriginGlow, Projectile.scale * 1.25f, spriteEffects, 0f);

            return false;
        }
    }
}
