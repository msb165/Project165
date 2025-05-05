using Microsoft.Xna.Framework;
using System;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;

namespace Project165.Content.Projectiles.Melee
{
    public class BeamSuperProj : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Excalibur}";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new(8);
            Projectile.aiStyle = ProjAIStyleID.TrueNightsEdge;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 90;
            Projectile.ownerHitCheck = true;
            Projectile.ownerHitCheckDistance = 300f;
        }

        Player Player => Main.player[Projectile.owner];
        public override void AI()
        {
            float num3 = Projectile.ai[1];
            float maxTime = Projectile.ai[1] + 25f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[1] == 1f)
            {
                Projectile.localAI[0] += 2f;
            }
            Projectile.Opacity = Utils.Remap(Projectile.localAI[0], 0f, Projectile.ai[1], 0f, 1f) * Utils.Remap(Projectile.localAI[0], num3, maxTime, 1f, 0f);
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.localAI[1] = 1f;
                Projectile.Kill();
                return;
            }
            float fromValue = Projectile.localAI[0] / Projectile.ai[1];
            float num6 = Utils.Remap(Projectile.localAI[0], Projectile.ai[1] * 0.4f, maxTime, 0f, 1f);
            Projectile.direction = (Projectile.spriteDirection = (int)Projectile.ai[0]);

            Projectile.ownerHitCheck = Projectile.localAI[0] <= 6f;
            if (Projectile.localAI[0] >= MathHelper.Lerp(num3, maxTime, 0.65f))
            {
                Projectile.damage = 0;
            }
            float fromValue2 = 1f - (1f - num6) * (1f - num6);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.scale = Utils.Remap(fromValue2, 0f, 1f, 1.5f, 1f) * Projectile.ai[2];
            Projectile.Opacity = Utils.Remap(Projectile.localAI[0], 0f, Projectile.ai[1] * 0.5f, 0f, 1f) * Utils.Remap(Projectile.localAI[0], maxTime - 12f, maxTime, 1f, 0f);
            if (Projectile.velocity.Length() > 8f)
            {
                Projectile.velocity *= 0.94f;
                float num12 = Utils.Remap(fromValue, 0.7f, 1f, 110f, 110f);
                if (Projectile.localAI[1] == 0f)
                {
                    bool canHit = false;
                    for (float i = -1f; i <= 1f; i += 0.5f)
                    {
                        Vector2 startPos = Projectile.Center + (Projectile.rotation + i * MathHelper.PiOver4 * 0.25f).ToRotationVector2() * num12 * 0.5f * Projectile.scale;
                        Vector2 endPos = Projectile.Center + (Projectile.rotation + i * (MathHelper.PiOver4) * 0.25f).ToRotationVector2() * num12 * Projectile.scale;
                        if (Collision.CanHit(startPos, 0, 0, endPos, 0, 0))
                        {
                            canHit = true;
                            break;
                        }
                    }
                    if (!canHit)
                    {
                        Projectile.localAI[1] = 1f;
                    }
                }
                if (Projectile.localAI[1] == 1f && Projectile.velocity.Length() > 8f)
                {
                    Projectile.velocity *= 0.8f;
                }
                if (Projectile.localAI[1] == 1f)
                {
                    Projectile.velocity *= 0.88f;
                }
            }
            float num14 = Projectile.rotation + Main.rand.NextFloatDirection() * MathHelper.PiOver2 * 0.9f;
            Vector2 vector3 = Projectile.Center + num14.ToRotationVector2() * 85f * Projectile.scale;
            Color value = new(200, 220, 96);
            Color value2 = new(105, 124, 12);
            Lighting.AddLight(Projectile.Center + Projectile.rotation.ToRotationVector2() * 85f * Projectile.scale, value.ToVector3());
            for (int j = 0; j < 3; j++)
            {
                if (Main.rand.NextFloat() < Projectile.Opacity + 0.1f)
                {
                    Color.Lerp(Color.Lerp(Color.Lerp(value2, value, Utils.Remap(fromValue, 0f, 0.6f, 0f, 1f)), Color.White, Utils.Remap(fromValue, 0.6f, 0.8f, 0f, 0.5f)), Color.White, Main.rand.NextFloat() * 0.3f);
                    Dust dust3 = Dust.NewDustPerfect(vector3, DustID.YellowStarDust, Projectile.velocity * 0.4f, 100, default(Color) * Projectile.Opacity, 1f * Projectile.Opacity);
                    dust3.scale *= 0.7f;                    
                    dust3.velocity += Player.velocity * 0.1f;
                    dust3.position -= dust3.velocity * 6f;
                }
            }
            if (Projectile.damage == 0)
            {
                Projectile.localAI[0] += 3f;
                Projectile.velocity *= 0.76f;
            }
            if (Projectile.localAI[0] < 10f && (Projectile.localAI[1] == 1f || Projectile.damage == 0))
            {
                Projectile.localAI[0] += 1f;
                Projectile.velocity *= 0.85f;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float coneLength = 90f * Projectile.scale;
            float maximumAngle = MathHelper.PiOver4;
            float coneRotation = Projectile.rotation;
            if (targetHitbox.IntersectsConeFastInaccurate(Projectile.Center, coneLength, coneRotation, maximumAngle) && Collision.CanHit(Projectile.Center, 0, 0, targetHitbox.Center.ToVector2(), 0, 0))
            {
                return true;
            }
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.position += Projectile.velocity;
            Projectile.velocity = oldVelocity;
            Projectile.velocity *= 0.01f;
            Projectile.localAI[1] = 1f;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.immune[Projectile.owner] == 0)
            {
                Vector2 positionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox);
                ParticleOrchestraSettings orchestraSettings = default;
                orchestraSettings.PositionInWorld = positionInWorld;
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur, orchestraSettings, Projectile.owner);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Texture2D val = TextureAssets.Projectile[Type].Value;
            Rectangle rectangle = val.Frame(1, 4);
            Vector2 origin = rectangle.Size() / 2f;
            float scale = Projectile.scale;
            SpriteEffects spriteEffects = Projectile.ai[0] >= 0f ? SpriteEffects.None : SpriteEffects.FlipVertically;
            SpriteEffects effects = spriteEffects ^ SpriteEffects.FlipVertically;
            float num = Utils.Remap(Projectile.localAI[0], 0f, Projectile.ai[1] + 30f, 0f, 1f);
            float opacity = Projectile.Opacity;
            float drawLightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / MathF.Sqrt(3);
            drawLightColor = 0.5f + drawLightColor * 0.5f;
            drawLightColor = Utils.Remap(drawLightColor, 0.2f, 1f, 0f, 1f);

            Color backColor = new(255, 190, 0);
            Color middleColor = new(255, 210, 90);
            Color frontColor = new(255, 255, 202);

            Color edgeGlowColor = Color.White * opacity * 0.5f;
            edgeGlowColor.A = (byte)(edgeGlowColor.A * (1f - drawLightColor));
            Color color5 = edgeGlowColor * drawLightColor * 0.5f;
            color5.G = (byte)(color5.G * drawLightColor);
            color5.B = (byte)(color5.R * (0.25f + drawLightColor * 0.75f));

            float num4 = 1f - num;
            float num5 = 0.25f;

            Main.spriteBatch.Draw(val, drawPos, rectangle, backColor * drawLightColor * opacity, Projectile.rotation + Projectile.ai[0] * (MathHelper.PiOver4) * 0.5f * -1f * (1f - num), origin, scale * 0.95f, spriteEffects, 0f);
            Main.spriteBatch.Draw(val, drawPos, rectangle, backColor * drawLightColor * opacity, Projectile.rotation + Projectile.ai[0] * (MathHelper.PiOver4) * 0.5f * 1f * (1f - num), origin, scale * 0.95f, effects, 0f);
            Main.spriteBatch.Draw(val, drawPos, rectangle, color5 * 0.15f, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, scale, spriteEffects, 0f);
            Main.spriteBatch.Draw(val, drawPos, rectangle, color5 * 0.15f, Projectile.rotation + Projectile.ai[0] * -0.01f, origin, scale, effects, 0f);
            Main.spriteBatch.Draw(val, drawPos, rectangle, frontColor * drawLightColor * opacity * 0.3f, Projectile.rotation + Projectile.ai[0] * num5 * num4, origin, scale, spriteEffects, 0f);
            Main.spriteBatch.Draw(val, drawPos, rectangle, frontColor * drawLightColor * opacity * 0.3f, Projectile.rotation + (0f - Projectile.ai[0]) * num5 * num4, origin, scale, effects, 0f);
            Main.spriteBatch.Draw(val, drawPos, rectangle, middleColor * drawLightColor * opacity * 0.5f, Projectile.rotation + Projectile.ai[0] * 0.15f * num4, origin, scale * 0.975f, spriteEffects, 0f);
            Main.spriteBatch.Draw(val, drawPos, val.Frame(1, 4, 0, 3), Color.White * 0.6f * opacity, Projectile.rotation + Projectile.ai[0] * 0.05f * num4, origin, scale, spriteEffects, 0f);
            Main.spriteBatch.Draw(val, drawPos, val.Frame(1, 4, 0, 3), Color.White * 0.5f * opacity, Projectile.rotation + Projectile.ai[0] * -0.05f, origin, scale * 0.8f, spriteEffects, 0f);
            Main.spriteBatch.Draw(val, drawPos, val.Frame(1, 4, 0, 3), Color.White * 0.4f * opacity, Projectile.rotation + Projectile.ai[0] * -0.1f, origin, scale * 0.6f, spriteEffects, 0f);

            for (float j = -3f; j < 3f; j += 1f)
            {
                float num9 = Projectile.rotation + Projectile.ai[0] * j * MathHelper.TwoPi * 0.025f;
                Vector2 starPos = drawPos + num9.ToRotationVector2() * (val.Width * 0.5f - 6f) * scale;
                Color drawColor = middleColor * opacity;
                drawColor *= MathF.Abs(j) / 9f;
                DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, starPos, drawColor, frontColor, num, 0f, 0.5f, 0.5f, 1f, num9, new Vector2(0f, Utils.Remap(num, 0f, 1f, 3f, 0f)) * scale, Vector2.One * scale);
            }

            Vector2 starPos2 = drawPos + Projectile.rotation.ToRotationVector2() * (val.Width * 0.5f - 4f) * scale;
            DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, starPos2, new Color(255, 255, 255, 0) * opacity * 0.5f, frontColor, num, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(2f, Utils.Remap(num, 0f, 1f, 4f, 1f)) * scale, Vector2.One * scale * 1.5f);
            return false;
        }

        private void DrawPrettyStarSparkle(float Opacity, SpriteEffects dir, Vector2 drawpos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness)
        {
            Texture2D value = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Color color = shineColor with { A = 0 } * Opacity * 0.5f;
            Vector2 origin = value.Size() / 2f;
            Color color2 = drawColor * 0.5f;
            float num = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
            Vector2 vector = new Vector2(fatness.X * 0.5f, scale.X) * num;
            Vector2 vector2 = new Vector2(fatness.Y * 0.5f, scale.Y) * num;
            color *= num;
            color2 *= num;
            Main.EntitySpriteDraw(value, drawpos, null, color, MathHelper.PiOver2 + rotation, origin, vector, dir);
            Main.EntitySpriteDraw(value, drawpos, null, color, 0f + rotation, origin, vector2, dir);
            Main.EntitySpriteDraw(value, drawpos, null, color2, MathHelper.PiOver2 + rotation, origin, vector * 0.6f, dir);
            Main.EntitySpriteDraw(value, drawpos, null, color2, 0f + rotation, origin, vector2 * 0.6f, dir);
        }
    }
}
