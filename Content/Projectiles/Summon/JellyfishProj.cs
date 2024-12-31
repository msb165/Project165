using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Buffs;
using Steamworks;
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
    public class JellyfishProj : ModProjectile
    {
        public override string Texture => $"Terraria/Images/NPC_{NPCID.BlueJellyfish}";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            Main.projFrames[Type] = 7;
        }

        public override void SetDefaults() 
        {
            Projectile.netImportant = true;
            Projectile.alpha = 100;
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 12;
        }

        Player Player => Main.player[Projectile.owner];
        float JellyFishColor => Projectile.ai[2];

        public bool CheckActive(Player player)
        {
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<JellyfishBuff>());
                return false;
            }

            if (player.HasBuff(ModContent.BuffType<JellyfishBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        public void FindFrame()
        {
            int projFrames = Projectile.ai[0] == 2f ? Main.projFrames[Type] : Main.projFrames[Type] - 3;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= projFrames)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= projFrames)
            {
                Projectile.frame = 0;
                if (Projectile.ai[0] == 2f)
                {
                    Projectile.frame = 4;
                }
            }
        }

        public override void AI()
        {
            if (!CheckActive(Player))
            {
                return;
            }

            FindFrame();

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;

            float maxDistance = 2000f;
            float maxPlayerDist = 1000f;
            float maxPlrDistWithTarget = 1400f;
            float num558 = 150f;
            float acceleration = 0.1f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (i != Projectile.whoAmI && Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner && Math.Abs(Projectile.position.X - Main.projectile[i].position.X) + Math.Abs(Projectile.position.Y - Main.projectile[i].position.Y) < Projectile.width)
                {
                    Projectile.velocity.X += acceleration * -(Projectile.position.X < Main.projectile[i].position.X).ToDirectionInt();
                    Projectile.velocity.Y += acceleration * -(Projectile.position.Y < Main.projectile[i].position.Y).ToDirectionInt();
                }
            }
            bool isAttacking = false;
            if (Projectile.ai[0] == 2f)
            {
                Projectile.ai[1]++;
                Projectile.extraUpdates = 1;
                if (Projectile.ai[1] > 60f)
                {
                    Projectile.ai[1] = 1f;
                    Projectile.ai[0] = 0f;
                    Projectile.extraUpdates = 0;
                    Projectile.numUpdates = 0;
                    Projectile.netUpdate = true;
                }
                else
                {
                    isAttacking = true;
                }
            }
            if (isAttacking)
            {
                int dustType = DustID.Electric;
                switch (JellyFishColor)
                {
                    case 0:
                        dustType = DustID.Electric;
                        break;
                    case 1:
                        dustType = DustID.GreenTorch;
                        break;
                    case 2:
                        dustType = DustID.WitherLightning;
                        break;
                }
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType);
                dust.velocity *= 0.3f;
                dust.position = Projectile.Center - Projectile.velocity / 10f;
                dust.noGravity = true;
                return;
            }
            Vector2 projPos = Projectile.position;
            Vector2 targetVelocity = Vector2.Zero;
            bool foundTarget = false;
            NPC ownerMinionAttackTargetNPC = Projectile.OwnerMinionAttackTargetNPC;
            if (ownerMinionAttackTargetNPC != null && ownerMinionAttackTargetNPC.CanBeChasedBy(this))
            {
                float targetDist = Vector2.Distance(ownerMinionAttackTargetNPC.Center, Projectile.Center);
                float tripleMaxDist = maxDistance * 3f;
                if (targetDist < tripleMaxDist && !foundTarget && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, ownerMinionAttackTargetNPC.position, ownerMinionAttackTargetNPC.width, ownerMinionAttackTargetNPC.height))
                {
                    maxDistance = targetDist;
                    projPos = ownerMinionAttackTargetNPC.Center;
                    foundTarget = true;
                }
            }
            if (!foundTarget)
            {
                for (int j = 0; j < Main.maxNPCs; j++)
                {
                    NPC target = Main.npc[j];
                    if (target.CanBeChasedBy(this))
                    {
                        float distance = Vector2.Distance(target.Center, Projectile.Center);
                        if (distance <= maxDistance && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, target.position, target.width, target.height))
                        {
                            maxDistance = distance;
                            projPos = target.Center;
                            targetVelocity = target.velocity;
                            foundTarget = true;
                        }
                    }
                }
            }
            float projPlayerDist = maxPlayerDist;
            if (foundTarget)
            {
                projPlayerDist = maxPlrDistWithTarget;
            }
            if (Vector2.Distance(Player.Center, Projectile.Center) > projPlayerDist)
            {
                Projectile.ai[0] = 1f;
                Projectile.tileCollide = false;
                Projectile.netUpdate = true;
            }
            if (foundTarget && Projectile.ai[0] == 0f)
            {
                Vector2 newVelocity = Vector2.Normalize(projPos - Projectile.Center);
                if (Vector2.Distance(projPos, Projectile.Center) > 200f)
                {
                    newVelocity *= 10f;
                    Projectile.velocity = (Projectile.velocity * 40f + newVelocity) / 41f;
                }
                else
                {
                    newVelocity *= -4f;
                    Projectile.velocity = (Projectile.velocity * 40f + newVelocity) / 41f;
                }
            }
            else
            {
                bool isIdling = Projectile.ai[0] == 1f;
                float maxSpeed = 6f;
                float num576 = 40f;
                if (isIdling)
                {
                    maxSpeed = 15f;
                }
                Vector2 playerProjOffset = Player.Center - Projectile.Center - new Vector2(0, 60f);
                float playerProjDist = playerProjOffset.Length();
                if (playerProjDist > 200f && maxSpeed < 8f)
                {
                    maxSpeed = 8f;
                }
                if (maxSpeed < Math.Abs(Player.velocity.X) + Math.Abs(Player.velocity.Y))
                {
                    num576 = 30f;
                    maxSpeed = Math.Abs(Player.velocity.X) + Math.Abs(Player.velocity.Y);
                    if (playerProjDist > 200f)
                    {
                        num576 = 20f;
                        maxSpeed += 4f;
                    }
                    else if (playerProjDist > 100f)
                    {
                        maxSpeed += 3f;
                    }
                }
                if (isIdling && playerProjDist > 300f)
                {
                    maxSpeed += 6f;
                    num576 -= 10f;
                }
                if (playerProjDist < num558 && isIdling && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                if (playerProjDist > 2000f)
                {
                    Projectile.position.X = Player.Center.X - Projectile.width / 2;
                    Projectile.position.Y = Player.Center.Y - Projectile.height / 2;
                    Projectile.netUpdate = true;
                }
                if (playerProjDist > 70f)
                {
                    Vector2 vector46 = Vector2.Normalize(playerProjOffset) * maxSpeed;
                    Projectile.velocity = (Projectile.velocity * num576 + vector46) / (num576 + 1f);
                }
                else if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                {
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
                if (Projectile.velocity.Length() > maxSpeed)
                {
                    Projectile.velocity *= 0.95f;
                }
            }
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += Main.rand.Next(1, 4);
            }
            Projectile.ai[1] = 0f;
            Projectile.netUpdate = true;
            if (Projectile.ai[0] == 0f)
            {
                if (Projectile.ai[1] == 0f && foundTarget && maxDistance < 500f)
                {
                    Projectile.ai[1]++;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.ai[0] = 2f;
                        Vector2 targetVel = projPos - Projectile.Center;
                        targetVel = targetVel.SafeNormalize(Projectile.velocity);
                        float speedMult = 6f;
                        Projectile.velocity = targetVel * speedMult;
                        TryInterceptingTarget(projPos, targetVelocity, speedMult);
                        Projectile.netUpdate = true;
                    }
                }
            }
        }

        public void TryInterceptingTarget(Vector2 targetDir, Vector2 targetVelocity, float speed)
        {
            for (float i = 1f; i <= 1.5f; i += 0.1f)
            {
                Utils.ChaseResults chaseResults = Utils.GetChaseResults(Projectile.Center, speed, targetDir, targetVelocity / 2);
                if (chaseResults.InterceptionHappens && chaseResults.InterceptionTime <= 45f)
                {
                    Projectile.velocity = chaseResults.ChaserVelocity;
                    break;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color projColor = Projectile.GetAlpha(lightColor);
            Color trailColor = projColor;

            Main.instance.LoadNPC(NPCID.BlueJellyfish);
            Main.instance.LoadNPC(NPCID.GreenJellyfish);
            Main.instance.LoadNPC(NPCID.PinkJellyfish);
            int npcID = NPCID.BlueJellyfish;
            switch (JellyFishColor)
            {
                case 0:
                    npcID = NPCID.BlueJellyfish;
                    break;
                case 1:
                    npcID = NPCID.GreenJellyfish;
                    break;
                case 2:
                    npcID = NPCID.PinkJellyfish;
                    break;
            }

            Texture2D texture = TextureAssets.Npc[npcID].Value;
            SpriteBatch spriteBatch = Main.spriteBatch;

            int frameHeight = texture.Height / Main.projFrames[Type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);

            // Show afterimages only when it's attacking
            if (Projectile.ai[0] == 2f)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Vector2 oldDraw = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                    trailColor *= 0.8f;
                    spriteBatch.Draw(texture, oldDraw, sourceRectangle, trailColor, Projectile.rotation, sourceRectangle.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
                }
            }

            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, projColor, Projectile.rotation, sourceRectangle.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
