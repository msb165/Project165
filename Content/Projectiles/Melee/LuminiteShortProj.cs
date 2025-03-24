using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Buffs;
using Project165.Content.Dusts;
using Project165.Content.Items.Weapons.Melee;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Melee
{
    public class LuminiteShortProj : ModProjectile
    {
        public override string Texture => ModContent.GetInstance<LuminiteShortsword>().Texture;
        public override void SetDefaults()
        {
            Projectile.Size = new(45);
            Projectile.aiStyle = -1;
            Projectile.scale = 1.25f;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
        }

        Player Player => Main.player[Projectile.owner];
        public override void AI()
        {
            Projectile.scale = Projectile.ai[1];
            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 8f)
            {
                Projectile.ai[0] = 0f;
            }
            Projectile.soundDelay--;
            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item19 with { PitchVariance = 1f }, Projectile.Center);
                Projectile.soundDelay = 8;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                if (Player.channel && !Player.noItems && !Player.CCed)
                {
                    float scaleFactor = Player.HeldItem.shootSpeed;
                    Vector2 vec = Main.MouseWorld - Player.RotatedRelativePoint(Player.MountedCenter);
                    vec = vec.SafeNormalize(Vector2.UnitX * Player.direction) * scaleFactor;
                    if (vec.X != Projectile.velocity.X || vec.Y != Projectile.velocity.Y)
                    {
                        Projectile.netUpdate = true;
                    }
                    Projectile.velocity = vec;
                }
                else
                {
                    Projectile.Kill();
                }
            }

            // Create light on a line
            DelegateMethods.v3_1 = new Vector3(0.5f);
            Utils.PlotTileLine(Projectile.Center - Projectile.velocity, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 80f, 16f, DelegateMethods.CastLightOpen);

            Projectile.position = Player.RotatedRelativePoint(Player.MountedCenter, reverseRotation: false, addGfxOffY: false) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            CreateDust();
            SetPlayerValues();
        }

        public void CreateDust()
        {
            Vector2 spawnPos = Projectile.Center + Projectile.velocity * 6f - Projectile.Size / 2;
            Dust dust = Dust.NewDustDirect(spawnPos, Projectile.width, Projectile.height, DustID.RainbowMk2, newColor:new(34, 221, 151, 0), Scale: 1.25f);
            dust.velocity = Projectile.velocity * 2f;
            dust.noGravity = true;
        }

        public void SetPlayerValues()
        {
            Player.ChangeDir(Projectile.direction);
            Player.heldProj = Projectile.whoAmI;
            Player.SetDummyItemTime(2);
            Player.itemRotation = MathHelper.WrapAngle((Projectile.velocity * Projectile.direction).ToRotation() + Main.rand.NextFloatDirection() * MathHelper.Pi * 0.1f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (float i = 0f; i <= 1f; i += 0.05f)
            {
                float remapVal = Utils.Remap(i, 0f, 1f, 1f, 5f);
                Rectangle projBox = projHitbox;
                Vector2 boxPos = Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.width * remapVal;
                projBox.Offset((int)boxPos.X, (int)boxPos.Y);
                if (projBox.Intersects(targetHitbox))
                {
                    return true;
                }
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<MoonFire>(), 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float projAi = Projectile.ai[1];
            if (projAi == 0f)
            {
                projAi = 1f;
            }
            int drawAmount = (int)Math.Ceiling(3f * projAi);
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.instance.LoadProjectile(ProjectileID.PiercingStarlight);
            Texture2D starTexture = TextureAssets.Projectile[ProjectileID.PiercingStarlight].Value;

            Vector2 projCenter = Projectile.Center - Projectile.rotation.ToRotationVector2() * 2;
            Vector2 origin = texture.Size() / 2f;
            Vector2 originStar = starTexture.Size() / 2f;

            float randFloat = Main.rand.NextFloat();
            float lerpVal = Utils.GetLerpValue(0f, 0.3f, randFloat, clamped: true) * Utils.GetLerpValue(1f, 0.5f, randFloat, clamped: true);
            Color color = Projectile.GetAlpha(Lighting.GetColor(Projectile.Center.ToTileCoordinates())) * lerpVal;
            float newDirection = Main.rand.NextFloatDirection();
            float swordOffset = 11f + MathHelper.Lerp(0f, 20f, randFloat) + Main.rand.NextFloat() * 6f;
            float newRotation = Projectile.rotation + newDirection * MathHelper.TwoPi * 0.04f;
            float swordRotation = newRotation + MathHelper.PiOver4;
            Vector2 drawPos = projCenter + newRotation.ToRotationVector2() * swordOffset + Main.rand.NextVector2Circular(4f, 4f) - Main.screenPosition;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.rotation < -MathHelper.PiOver2 || Projectile.rotation > MathHelper.PiOver2)
            {
                swordRotation += MathHelper.PiOver2;
                spriteEffects |= SpriteEffects.FlipHorizontally;
            }

            Main.spriteBatch.Draw(texture, drawPos, null, color, swordRotation, origin, Projectile.scale, spriteEffects, 0f);
            for (int j = 0; j < drawAmount; j++)
            {
                Color brightStarColor = new(17, 110, 75, 0);
                Color darkStarColor = new(0, 158, 191, 0);
                float floatVal = Main.rand.NextFloat();
                float amount = Utils.GetLerpValue(0f, 0.3f, floatVal, clamped: true) * Utils.GetLerpValue(1f, 0.5f, floatVal, clamped: true);
                float num14 = (projAi - 1f) / 2f;
                float num15 = Main.rand.NextFloat() * 2f * projAi;
                num15 += num14;
                Vector2 starScale = new Vector2(2.8f + num15 * (1f + num14), 1f) * MathHelper.Lerp(0.6f, 1f, amount);
                Vector2 rotVector = Projectile.rotation.ToRotationVector2() * ((j >= 1) ? 56 : 0);
                float offest = 90f + MathHelper.Lerp(0f, 90f * projAi, floatVal) + num15 * 16f;
                float starRotation = Projectile.rotation + Main.rand.NextFloatDirection() * MathHelper.TwoPi * (0.05f - j * 0.01f) / projAi;
                Vector2 drawPosStar = projCenter + starRotation.ToRotationVector2() * offest + Main.rand.NextVector2Circular(20f, 20f) + rotVector - Main.screenPosition;
                SpriteEffects effects = SpriteEffects.None;

                Main.spriteBatch.Draw(texture, drawPosStar, null, brightStarColor, starRotation + MathHelper.PiOver4, origin, Projectile.scale * 1.25f, effects, 0f);
                // Glow effect
                Main.spriteBatch.Draw(texture, drawPos + starRotation.ToRotationVector2() * 48f, null, brightStarColor * 0.25f, swordRotation, origin, Projectile.scale * 1.25f, spriteEffects, 0f);
                Main.spriteBatch.Draw(texture, drawPos + starRotation.ToRotationVector2() * 48f, null, darkStarColor * 0.25f, swordRotation, origin, Projectile.scale * 1.25f, spriteEffects, 0f);

                Main.spriteBatch.Draw(starTexture, drawPosStar, null, darkStarColor * 0.25f, starRotation, originStar, starScale, effects, 0f);
                Main.spriteBatch.Draw(starTexture, drawPosStar, null, brightStarColor * 0.5f, starRotation, originStar, starScale * 0.6f, effects, 0f);
            }

            return false;
        }
    }
}
