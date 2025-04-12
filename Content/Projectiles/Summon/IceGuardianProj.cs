using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Buffs;
using Project165.Content.Projectiles.Ranged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Summon
{
    public class IceGuardianProj : ModProjectile
    {
        public enum AIState : int
        {
            Idling = 0,
            Attacking = 1
        }

        public AIState CurrentState 
        { 
            get => (AIState)Projectile.ai[0]; 
            set => Projectile.ai[0] = (float)value; 
        }

        public float AITimer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public float AttackTarget 
        { 
            get => Projectile.ai[2]; 
            set => Projectile.ai[2] = value; 
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Projectile.Size = new(34);
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage() => false;

        public bool CheckActive(Player player)
        {
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<IceGuardianBuff>());
                return false;
            }

            if (player.HasBuff(ModContent.BuffType<IceGuardianBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        Player Player => Main.player[Projectile.owner];

        public override void AI()
        {
            if (!CheckActive(Player))
            {
                return;
            }

            if (CurrentState == AIState.Idling)
            {
                Idle();
            }
            else if (CurrentState == AIState.Attacking)
            {
                Attack();
            }

            float acceleration = 3f;
            float projWidth = Projectile.width * 1.5f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (i != Projectile.whoAmI && Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].type == Projectile.type && MathF.Abs(Projectile.position.X - Main.projectile[i].position.X) + MathF.Abs(Projectile.position.Y - Main.projectile[i].position.Y) < projWidth)
                {
                    if (Projectile.position.X < Main.projectile[i].position.X)
                    {
                        Projectile.velocity.X -= acceleration;
                    }
                    else
                    {
                        Projectile.velocity.X += acceleration;
                    }
                    if (Projectile.position.Y < Main.projectile[i].position.Y)
                    {
                        Projectile.velocity.Y -= acceleration;
                    }
                    else
                    {
                        Projectile.velocity.Y += acceleration;
                    }
                }
            }

            Projectile.spriteDirection = Projectile.direction;
        }

        public void Idle()
        {
            Vector2 newSpeed = Player.Center - Projectile.Center - Vector2.UnitY * 80f;
            float distance = newSpeed.Length();
            float multiplier = 12f;
            newSpeed.Normalize();

            if (distance > 50f)
            {
                newSpeed *= multiplier;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, newSpeed, 0.015f);
            }
            if (distance > 2000f)
            {
                Projectile.position = Player.Center - Vector2.UnitY * 60f;
                Projectile.netUpdate = true;
            }

            if (Projectile.velocity.Length() > multiplier)
            {
                Projectile.velocity *= 0.95f;
            }

            Projectile.rotation = Projectile.AngleTo(Player.Center) - MathHelper.PiOver2;

            int attackRange = 800;
            int attackTarget = -1;
            Projectile.Minion_FindTargetInRange(attackRange, ref attackTarget, skipIfCannotHitWithOwnBody: false);
            if (attackTarget != -1)
            {
                Projectile.netUpdate = true;
                CurrentState = AIState.Attacking;
                AttackTarget = attackTarget;
            }
        }

        int timeToShoot = 40;
        public void Attack()
        {
            NPC target = null;
            int targetIndex = (int)AttackTarget;
            if (Main.npc.IndexInRange(targetIndex) && Main.npc[targetIndex].CanBeChasedBy(this))
            {
                target = Main.npc[targetIndex];
            }

            AITimer++;
            if (target == null || Player.Distance(target.Center) >= 1000f)
            {
                CurrentState = AIState.Idling;
                AITimer = 0f;
                AttackTarget = -1f;
                Projectile.netUpdate = true;
            }
            else
            {
                Vector2 targetSpeed = Vector2.Normalize(target.Center + new Vector2(200 * target.direction, -200f) - Projectile.Center) * 12f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetSpeed, 0.025f);
                Projectile.rotation = Projectile.AngleTo(target.Center) - MathHelper.PiOver2;

                if (AITimer >= timeToShoot && Main.myPlayer == Projectile.owner && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    AITimer = 0f;
                    Vector2 projVel = Vector2.Normalize(target.Center - Projectile.Center) * 10f;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + projVel * 4f, projVel, ModContent.ProjectileType<IceGuardianBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);
            Color glowColor = Color.Cyan with { A = 40 } * Projectile.Opacity;
            float timer = MathF.Sin((float)Main.timeForVisualEffects * 0.25f * MathHelper.TwoPi / 30f);

            for (int i = 0; i < 8; i++)
            {
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY.RotatedBy(MathHelper.PiOver4 * i) * (1f + 4f * timer), null, glowColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
