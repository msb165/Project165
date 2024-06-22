using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Project165.Content.Projectiles.Summon
{
    public class MachineGunProj : ModProjectile
    {
        public enum AIState : int
        {
            Idle = 0,
            Targeting = 1,
            Shooting = 2
        }

        public AIState CurrentAIState
        {
            get => (AIState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }        

        public float AITimer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 54;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 36000;
            Projectile.tileCollide = true;
            Projectile.manualDirectionChange = true;
            Projectile.sentry = true;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
        }

        public readonly float shootRange = 900f;
        public readonly float deadBottomAngle = 0.75f;

        public Player Owner => Main.player[Projectile.owner];

        public override bool? CanDamage() => false;

        public override void AI()
        {
            switch (CurrentAIState)
            {
                case AIState.Idle:
                    IdleStuff();
                    break;
                case AIState.Targeting:
                    TargetStuff();
                    break;
                case AIState.Shooting:
                    ShootStuff();
                    break;
            }

            Projectile.spriteDirection = Projectile.direction;
            Projectile.tileCollide = true;
            Projectile.velocity.Y += 0.2f;
        }

        public void IdleStuff()
        {
            Projectile.direction = Projectile.spriteDirection = Owner.direction;
            CurrentAIState = AIState.Targeting;
            AITimer = 0f;
            Projectile.netUpdate = true;
        }

        public void TargetStuff()
        {
            bool shouldShoot = false;

            Projectile.frame = 0;
            if (AITimer > 0f)
            {
                AITimer--;
            }
            else
            {
                shouldShoot = true;
            }

            int targetIndex = BallistaFindTarget(shootRange, deadBottomAngle, Projectile.Center);
            if (targetIndex != -1)
            {
                Vector2 distance = (Main.npc[targetIndex].Center - Projectile.Center).SafeNormalize(Vector2.UnitY);
                Projectile.rotation = Projectile.AngleTo(Main.npc[targetIndex].Center);
                Projectile.direction = (Projectile.rotation > MathHelper.PiOver2 || Projectile.rotation < -MathHelper.PiOver2).ToDirectionInt() * -1;
                if (shouldShoot && Projectile.owner == Main.myPlayer)
                {
                    Projectile.rotation = distance.ToRotation();
                    CurrentAIState = AIState.Shooting;
                    AITimer = 0f;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Projectile.rotation = (Projectile.direction == 1 ? MathHelper.TwoPi : MathHelper.Pi);
            }
        }

        public void ShootStuff()
        {
            int timeToShoot = 3;
            if (AITimer == timeToShoot)
            {
                SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
                Vector2 targetPos = Vector2.UnitX * Projectile.direction;
                int targetIndex = BallistaFindTarget(shootRange, deadBottomAngle, Projectile.Center);
                if (targetIndex != -1)
                {
                    targetPos = (Main.npc[targetIndex].Center - Projectile.Center).SafeNormalize(Vector2.UnitX * Projectile.direction);
                }

                Projectile.rotation = targetPos.ToRotation();
                Projectile.direction = (Projectile.rotation > MathHelper.PiOver2 || Projectile.rotation < -MathHelper.PiOver2).ToDirectionInt() * -1;

                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, targetPos * 16f, ProjectileID.Bullet, Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
            if (++AITimer >= 12f)
            {
                CurrentAIState = AIState.Targeting;
                AITimer = 0f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public int BallistaFindTarget(float shootRange, float deadBottomAngle, Vector2 shootingSpot)
        {
            int targetIndex = -1;
            NPC ownerMinionAttackTargetNPC = Projectile.OwnerMinionAttackTargetNPC;
            if (ownerMinionAttackTargetNPC != null && ownerMinionAttackTargetNPC.CanBeChasedBy(this))
            {
                for (int i = 0; i < 1; i++)
                {
                    if (!ownerMinionAttackTargetNPC.CanBeChasedBy(this))
                    {
                        continue;
                    }
                    float distance = Vector2.Distance(shootingSpot, ownerMinionAttackTargetNPC.Center);
                    if (distance < shootRange)
                    {
                        Vector2 vector = (ownerMinionAttackTargetNPC.Center - shootingSpot).SafeNormalize(Vector2.UnitY);
                        if ((!(Math.Abs(vector.X) < Math.Abs(vector.Y) * deadBottomAngle) || vector.Y < 0f) && (targetIndex == -1 || distance < Vector2.Distance(shootingSpot, Main.npc[targetIndex].Center)) && Collision.CanHitLine(shootingSpot, 0, 0, ownerMinionAttackTargetNPC.Center, 0, 0))
                        {
                            targetIndex = ownerMinionAttackTargetNPC.whoAmI;
                        }
                    }
                }
                if (targetIndex != -1)
                {
                    return targetIndex;
                }
            }
            for (int j = 0; j < Main.maxNPCs; j++)
            {
                NPC target = Main.npc[j];
                if (!target.CanBeChasedBy(this))
                {
                    continue;
                }
                float distance = Vector2.Distance(shootingSpot, target.Center);
                if (distance < shootRange)
                {
                    Vector2 vector2 = (target.Center - shootingSpot).SafeNormalize(Vector2.UnitY);
                    if ((!(Math.Abs(vector2.X) < Math.Abs(vector2.Y) * deadBottomAngle) || !(vector2.Y > 0f)) && (targetIndex == -1 || distance < Vector2.Distance(shootingSpot, Main.npc[targetIndex].Center)) && Collision.CanHitLine(shootingSpot, 0, 0, target.Center, 0, 0))
                    {
                        targetIndex = j;
                    }
                }
            }
            return targetIndex;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D gunTexture = TextureAssets.Projectile[Type].Value;
            Texture2D baseTexture = (Texture2D)Request<Texture2D>("Project165/Assets/Images/MachineGunBase");
            

            Vector2 baseDrawPos = Projectile.Bottom + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Vector2 baseOrigin = baseTexture.Size() * new Vector2(0.5f, 1f);
            baseOrigin.Y -= 2f;

            //Rectangle gunSourceRectangle = gunTexture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);

            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;


            Vector2 gunOrigin = gunTexture.Size() / 2;
            gunOrigin.X += spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt() * 16;

            Main.EntitySpriteDraw(baseTexture, baseDrawPos, null, lightColor, 0f, baseOrigin, Projectile.scale, spriteEffects & SpriteEffects.FlipHorizontally, 0);

            Main.EntitySpriteDraw(gunTexture, Projectile.Center - Main.screenPosition + new Vector2(0f, 1f), gunTexture.Frame(), lightColor, Projectile.rotation, gunOrigin, Projectile.scale, spriteEffects);

            return false;
        }
    }
}
