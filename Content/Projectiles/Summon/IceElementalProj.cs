using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Buffs;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Summon
{
    public class IceElementalProj : ModProjectile
    {
        public override string Texture => $"Terraria/Images/NPC_{NPCID.IceElemental}";

        public float AITimer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override void SetStaticDefaults() 
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults() 
        {
            Projectile.width = 26;
            Projectile.height = 54;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
        }


        public bool CheckActive(Player player)
        {
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<IceElementalBuff>());
                return false;
            }

            if (player.HasBuff(ModContent.BuffType<IceElementalBuff>()))
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
            FindFrame();
            IdleAndAttack();
        }

        int timeToShoot = 80;
        public void IdleAndAttack()
        {
            Projectile.Center = Vector2.Lerp(Projectile.Center, Player.Center - (Projectile.ai[0] += 0.02f).ToRotationVector2() * 90f, 0.1f);
            Projectile.rotation = Projectile.AngleTo(Player.Center) - MathHelper.PiOver2;
            Projectile.direction = (Projectile.position.X > Player.position.X).ToDirectionInt();
            Projectile.spriteDirection = Projectile.direction;

            int attackRange = 600;
            int attackTarget = -1;
            Projectile.Minion_FindTargetInRange(attackRange, ref attackTarget, skipIfCannotHitWithOwnBody: false);
            if (attackTarget != -1)
            {                
                NPC target = null;
                int targetIndex = attackTarget;

                if (Main.npc.IndexInRange(targetIndex) && Main.npc[targetIndex].CanBeChasedBy(this))
                {
                    target = Main.npc[targetIndex];
                }

                if (target == null)
                {
                    attackTarget = -1;
                    AITimer = 0f;
                    Projectile.netUpdate = true;
                }

                Projectile.direction = (target.position.X > Player.position.X).ToDirectionInt();

                if (AITimer > 0f)
                {
                    AITimer += Main.rand.Next(1, 4);
                }

                if (AITimer >= timeToShoot)
                {
                    AITimer = 0f;
                    Projectile.netUpdate = true;
                }

                if (Main.myPlayer == Projectile.owner && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height) && AITimer == 0f)
                {
                    AITimer++;
                    Vector2 vel = Vector2.Normalize(target.Center - Projectile.Center) * 8f;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + vel, vel, ProjectileID.FrostBlastFriendly, Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        public void FindFrame()
        {
            if (++Projectile.frameCounter >= Main.projFrames[Type])
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int frameHeight = texture.Height / Main.projFrames[Type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRect = new(0, startY, texture.Width, frameHeight);
            Vector2 drawOrigin = sourceRect.Size() / 2;
            Color drawColor = Projectile.GetAlpha(lightColor);
            Color glowColor = Color.Cyan with { A = 80 } * Projectile.Opacity;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                glowColor *= 0.75f;
                Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, sourceRect, glowColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRect, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
