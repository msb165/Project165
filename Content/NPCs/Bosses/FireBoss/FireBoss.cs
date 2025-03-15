using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Common.Systems;
using Project165.Content.Items.TreasureBags;
using Project165.Content.Items.Weapons.Melee;
using Project165.Content.Items.Weapons.Ranged;
using Project165.Content.Projectiles.Hostile;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.NPCs.Bosses.FireBoss
{
    [AutoloadBossHead]
    public class FireBoss : ModNPC
    {
        public enum AIState : int
        {
            Spawning = 0,
            Flying = 1,
            Unused = 2,
            Unused2 = 3,
            Death = 4
        }

        public AIState CurrentAIState
        {
            get => (AIState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        public float AITimer
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        public float Direction
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }

        public float ShootTimer
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.TrailCacheLength[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
            Main.npcFrameCount[Type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.lifeMax = 42000;
            NPC.defense = 100;
            NPC.Size = new(60);
            NPC.scale = 2.5f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.Item105 with { Pitch = 1f };
            NPC.DeathSound = SoundID.Item105;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(
            [
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld
            ]);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }

        Player Player => Main.player[NPC.target];

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Player.dead || !Player.active)
            {
                NPC.TargetClosest();
            }

            if (Player.dead)
            {
                NPC.velocity.Y += 0.2f;
                NPC.EncourageDespawn(despawnTime: 10);
            }

            if (NPC.localAI[1] == 0f)
            {
                NPC.localAI[1] = 1f;
                NPC.alpha = 255;
                CurrentAIState = AIState.Spawning;
            }

            Lighting.AddLight(NPC.Center, 2f, 2f, 2f);
            NPC.dontTakeDamage = CurrentAIState is AIState.Spawning or AIState.Death;

            switch (CurrentAIState)
            {
                case AIState.Spawning:
                    SpawnAnimation();
                    break;
                case AIState.Flying:
                    FlyAround();
                    break;
                case AIState.Unused:
                    StayAbove();
                    break;
                case AIState.Unused2:
                    SpinState();
                    break;
            }
        }

        public void SpawnAnimation()
        {
            NPC.TargetClosest();
            NPC.localAI[0]++;
            if (NPC.alpha > 0)
            {
                NPC.alpha -= 4;
            }

            Player.AddBuff(BuffID.Slow, 300);

            for (int i = 0; i < 3; i++)
            {
                Vector2 newVelocity = Main.rand.NextVector2Circular(200f, 200f).SafeNormalize(Vector2.UnitY) * 5f;
                Dust dust = Dust.NewDustPerfect(NPC.Center - newVelocity * 20f, DustID.Torch, newVelocity, Scale: 3f);
                dust.noGravity = true;
            }

            if (NPC.alpha <= 0)
            {
                Player.ClearBuff(BuffID.Slow);
                NPC.alpha = 0;
            }

            if (NPC.localAI[0] > 180f)
            {
                NPC.localAI[0] = 180f;
            }

            if (NPC.localAI[0] >= 180f)
            {
                CurrentAIState = AIState.Flying;
                NPC.netUpdate = true;
            }
        }

        public void FlyAround()
        {
            if (Direction == 0f)
            {
                NPC.TargetClosest();
                Direction = (NPC.Center.X < Player.Center.X).ToDirectionInt();
            }
            NPC.TargetClosest();
            float moveAcceleration = 0.5f;
            float maxSpeed = 11f;
            float maxDistance = 600f;
            float distance = MathF.Abs(NPC.Center.X - Player.Center.X);

            if ((NPC.Center.X < Player.Center.X && Direction < 0f || NPC.Center.X > Player.Center.X && Direction > 0f) && distance > maxDistance)
            {
                Direction = 0f;
            }
            if (NPC.life < NPC.lifeMax * 0.5)
            {
                moveAcceleration = 0.85f;
                maxSpeed = 12f;
            }
            if (NPC.life < NPC.lifeMax * 0.25)
            {
                moveAcceleration = 1.15f;
                maxSpeed = 15f;
            }
            NPC.velocity.X += Direction * moveAcceleration;
            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -maxSpeed, maxSpeed);
            float verticalDistance = Player.Center.Y - 200f - NPC.Center.Y;
            if (NPC.velocity.Y < verticalDistance)
            {
                NPC.velocity.Y += 0.25f;
                if (NPC.velocity.Y < 0f && verticalDistance > 0f)
                {
                    NPC.velocity.Y += 0.25f;
                }
            }
            else if (NPC.velocity.Y > verticalDistance)
            {
                NPC.velocity.Y -= 0.25f;
                if (NPC.velocity.Y > 0f && verticalDistance < 0f)
                {
                    NPC.velocity.Y -= 0.25f;
                }
            }
            NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -8f, 8f);
            NPC.rotation = NPC.velocity.X * 0.025f;
            if (distance < 800f && NPC.position.Y < Player.position.Y && AITimer > 35f)
            {
                ShootTimer++;
                int timeToShoot = 60;
                if (NPC.life < NPC.lifeMax * 0.5)
                {
                    timeToShoot -= 4;
                }
                if (NPC.life < NPC.lifeMax * 0.25)
                {
                    timeToShoot -= 8;
                }
                if (ShootTimer >= timeToShoot && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    ShootTimer = 0f;
                    Vector2 newVelocity = Vector2.Normalize(Player.Center - NPC.Center) * 12f;
                    for (int i = 0; i < 3; i++)
                    {
                        newVelocity *= Main.rand.NextFloat(0.9f, 1.1f);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, newVelocity.RotatedByRandom(MathHelper.ToRadians(10f)), ModContent.ProjectileType<FireBossProj>(), NPC.GetAttackDamage_ForProjectiles(38f, 42f), 0f, Main.myPlayer);
                    }
                }
            }
            AITimer++;
            if (AITimer > 600f && distance < 600f)
            {
                CurrentAIState = AIState.Unused;
                AITimer = 0f;
                Direction = 0f;
                ShootTimer = 0f;
                NPC.rotation = 0f;
                NPC.netUpdate = true;
            }
        }

        public void StayAbove()
        {
            NPC.TargetClosest();
            Vector2 newPosition = Vector2.Normalize(Player.Center + Vector2.UnitX * 250f - NPC.Center) * 20f;
            NPC.rotation = (Player.Center - NPC.Center).ToRotation() - MathHelper.PiOver2;
            NPC.SimpleFlyMovement(newPosition, 0.5f);

            if (Main.netMode != NetmodeID.MultiplayerClient && AITimer > 30f)
            {
                ShootTimer++;
                int timeToShoot = 35;
                if (NPC.life < NPC.lifeMax * 0.75)
                {
                    timeToShoot = 30;
                }
                if (NPC.life < NPC.lifeMax * 0.5)
                {
                    timeToShoot = 28;
                }
                if (ShootTimer >= timeToShoot)
                {
                    ShootTimer = 0f;
                    Vector2 newVelocity = Vector2.Normalize(Player.Center - NPC.Center) * 8f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + newVelocity * 8f, newVelocity, ModContent.ProjectileType<FireBossProj>(), NPC.GetAttackDamage_ForProjectiles(28f, 37f), 0f, Main.myPlayer);
                    newVelocity.X *= -1f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), Player.Center + Vector2.UnitX * -400f * -NPC.direction, newVelocity, ModContent.ProjectileType<FireBossProj>(), NPC.GetAttackDamage_ForProjectiles(28f, 37f), 0f, Main.myPlayer);
                }
            }

            AITimer++;
            if (AITimer > 600f)
            {
                CurrentAIState = AIState.Unused2;
                ShootTimer = 0f;
                AITimer = 0f;
                NPC.netUpdate = true;
            }
        }

        public void SpinState()
        {
            int timeToShoot = 80;
            double difficultyValue = Main.expertMode ? 0.3 : 0.5;

            NPC.TargetClosest();
            Vector2 npcVel = Vector2.Normalize(Player.Center + Vector2.One * 200f - NPC.Center) * 12f;
            NPC.SimpleFlyMovement(npcVel, 0.25f);
            NPC.rotation = NPC.velocity.X * 0.05f;
            NPC.rotation = MathHelper.Clamp(NPC.rotation, -0.5f, 0.5f);
            
            if (NPC.life < NPC.lifeMax * difficultyValue)
            {
                timeToShoot -= 8;
            }
            if (NPC.life < NPC.lifeMax * 0.1)
            {
                timeToShoot -= 10;
            }
            ShootTimer++;
            if (ShootTimer > timeToShoot)
            {
                ShootTimer = 0f;
                for (int i = 0; i < 6; i++)
                {
                    //Vector2 newVelocity = (Vector2.UnitX * 16f).RotatedBy(i * MathHelper.TwoPi / 6f);
                    //Vector2 newPosition = Main.rand.NextVector2Circular(500f, 500f);
                    Vector2 newPosition = new(Main.screenPosition.X, Main.screenPosition.Y + Main.rand.Next(Main.screenHeight));
                    if (Main.rand.NextBool(2))
                    {
                        newPosition.X += Main.screenWidth;
                    }
                    Vector2 newVelocity = Vector2.Normalize(Player.Center - newPosition) * 8f;

                    if (Vector2.Distance(newPosition, Player.Center) < 100f)
                    {
                        //newPosition *= 1.5f;
                    }

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), newPosition, newVelocity, ModContent.ProjectileType<FireBossProjHoming>(), NPC.GetAttackDamage_ForProjectiles(33f, 38f), 0f, Main.myPlayer);
                }
            }
            AITimer++;
            if (AITimer > 500f)
            {
                CurrentAIState = AIState.Flying;
                ShootTimer = 0f;
                AITimer = 0f;
                NPC.netUpdate = true;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<FireBossBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<InfernalBow>(), 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SuperFireSword>(), 3));
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedFireBoss, -1);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.25;
            if (NPC.frameCounter >= Main.npcFrameCount[Type])
            {
                NPC.frameCounter = 0;
            }
            NPC.frame.Y = (int)NPC.frameCounter * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / Main.npcFrameCount[Type] / 2 + 16);
            Color npcDrawColor = Color.White * NPC.Opacity;
            Color npcDrawColorTrail = npcDrawColor with { A = 0 };

            float offset = (float)Main.timeForVisualEffects / 240f + Main.GlobalTimeWrappedHourly * 0.5f;

            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 drawPos = NPC.oldPos[i] + NPC.Size * 0.5f - screenPos;
                npcDrawColorTrail *= 0.5f;
                spriteBatch.Draw(texture, drawPos, NPC.frame, npcDrawColorTrail, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.25f)
            {
                spriteBatch.Draw(texture, NPC.Center - screenPos + (Vector2.UnitY * 8f).RotatedBy((i + offset) * MathHelper.TwoPi) * 3f, NPC.frame, Color.Yellow with { A = 0 } * NPC.Opacity * 0.2f, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, npcDrawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
