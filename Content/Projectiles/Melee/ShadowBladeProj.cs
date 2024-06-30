using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Items.Weapons.Melee;
using rail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Melee
{
    public class ShadowBladeProj : ModProjectile
    {
        public override string Texture => ModContent.GetInstance<ShadowBlade>().Texture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.Size = new(16);
            Projectile.ownerHitCheck = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.extraUpdates = 1;
            Projectile.scale = 1.25f;
            Projectile.noEnchantmentVisuals = true;
            Projectile.timeLeft = 12;
        }

        public Player Player => Main.player[Projectile.owner];
        public float InitialRotation = -1.75f;

        public bool IsActive => !Player.dead || Player.active || !Player.CCed;

        public float AITimer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float EaseInBack(float value)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1;

            return c3 * value * value * value - c1 * value * value;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + 30f * Projectile.scale * Projectile.rotation.ToRotationVector2(), 16f, ref collisionPoint);
        }

        public override void AI()
        {            
            if (!IsActive)
            {
                Projectile.Kill();
                Player.reuseDelay = 10;
                return;
            }


            AITimer += 0.5f;
            Projectile.rotation = Projectile.velocity.ToRotation() + (InitialRotation * Player.direction) + EaseInBack(AITimer / 10f) * Player.direction;            

            if (AITimer == 11.5f)
            {
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { Pitch = 0.5f }, Projectile.Center);
            }

            if (AITimer % 5f == 0f && Projectile.owner == Main.myPlayer)
            {
                float speedX = MathF.Sign(Projectile.velocity.X) + Main.rand.NextFloat(0.1f, 1f) * MathF.Sign(Projectile.velocity.X);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Player.Center + new Vector2(Projectile.velocity.X * 10f, -250f), new(speedX, 2f), ModContent.ProjectileType<ShadowSlash>(), (int)(Projectile.damage * 1.3f), Projectile.knockBack * 2f, Projectile.owner, 0f, 1f);
            }

            if (AITimer > 14f && Projectile.scale > 0.5f)
            {
                Projectile.scale *= 0.25f;
            }
            if (Projectile.scale <= 0.5f)
            {
                Projectile.scale = 0.5f;
                Projectile.Kill();
            }

            SetPlayerValues();
            SetProjectileValues();
        }

        public void SetProjectileValues()
        {
            Projectile.timeLeft = 2;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.Center = Player.RotatedRelativePoint(Player.Center, true) + Projectile.rotation.ToRotationVector2() * 30f * Projectile.scale - Vector2.UnitX * 4f * Player.direction;
        }

        public void SetPlayerValues()
        {
            Player.heldProj = Projectile.whoAmI;
            Player.itemRotation = MathHelper.WrapAngle(Projectile.rotation);
            float rotation = Projectile.rotation - MathHelper.PiOver4;
            if (Player.direction == -1)
            {
                rotation -= MathHelper.PiOver2;
            }
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
            Player.SetDummyItemTime(2);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
            target.AddBuff(BuffID.Weak, 300);

            ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.NightsEdge, new ParticleOrchestraSettings
            {
                PositionInWorld = target.Center,

            }, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Color projColor = Color.BlueViolet with { A = 0 };
            float rotation = Projectile.rotation + MathHelper.PiOver4;
            if (Player.direction == -1)
            {
                rotation += MathHelper.PiOver2;
            }

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldRot[i] == 0f)
                {
                    continue;
                }

                float oldRotation = Projectile.oldRot[i] + MathHelper.PiOver4;

                if (Player.direction == -1)
                {
                    oldRotation += MathHelper.PiOver2;
                }

                projColor *= 0.7f;
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), projColor, oldRotation, texture.Frame().Size() / 2, Projectile.scale, spriteEffects, 0);
            }

            Main.EntitySpriteDraw(texture, drawPos, texture.Frame(), Color.White, rotation, texture.Frame().Size() / 2, Projectile.scale, spriteEffects);
            return false;
        }
    }
}
