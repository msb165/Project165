using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Common.Systems;
using Project165.Content.Projectiles.Hostile;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
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

        public float AICounter
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.TrailCacheLength[Type] = 20;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
            Main.npcFrameCount[Type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.lifeMax = 42000;
            NPC.defense = 30;
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

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => true;

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }

        Player Player => Main.player[NPC.target];

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Player.dead || !Player.active)
            {
                NPC.TargetClosest();
            }
            Lighting.AddLight(NPC.Center, 2f, 2f, 2f);

            if (NPC.localAI[0] == 0f)
            {
                NPC.localAI[0] = 1f;
                NPC.alpha = 255;
                CurrentAIState = AIState.Spawning;
            }

            NPC.dontTakeDamage = CurrentAIState is AIState.Spawning;

            switch (CurrentAIState)
            {
                case AIState.Spawning:
                    SpawnAnimation();
                    break;
                case AIState.Flying:
                    FlyAround();
                    break;
                case AIState.Unused:
                    StayInFront();
                    break;
            }
        }

        public void SpawnAnimation()
        {
            if (NPC.alpha > 0)
            {
                NPC.alpha--;
            }

            for (int i = 0; i < 3; i++)
            {
                Vector2 newVelocity = Main.rand.NextVector2Circular(200f, 200f).SafeNormalize(Vector2.UnitY) * 5f;

                Dust dust = Dust.NewDustPerfect(NPC.Center - newVelocity * 20f, DustID.Torch, newVelocity, Scale:3f);
                dust.noGravity = true;
            }

            if (NPC.alpha <= 0)
            {
                NPC.alpha = 0;
                NPC.netUpdate = true;
                CurrentAIState = AIState.Flying;
            }
        }

        public void FlyAround()
        {
            NPC.TargetClosest();
            Vector2 newVel = NPC.DirectionTo(Player.Center - Vector2.UnitY * 250f) * 12f;
            NPC.rotation = NPC.velocity.X * 0.025f;

            AITimer++;
            if (AITimer > 90f)
            {
                AITimer = 0f;
                Vector2 newPos = Main.rand.NextVector2Unit(1000f, 1000f).SafeNormalize(Vector2.UnitY) * 6f;
                for (int j = -1; j <= 1; j++)
                {
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        break;
                    }
                    Vector2 newVelocity = newPos.RotatedBy(MathHelper.ToRadians(j * 15f));
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), Player.Center - newPos * 40, newVelocity, ModContent.ProjectileType<FireBossProj>(), NPC.GetAttackDamage_ForProjectiles(23f, 26f), 4f, Main.myPlayer);
                }
                AICounter++;
            }

            if (AICounter >= 5f)
            {
                AICounter = 0f;
                AITimer = 0f;
                NPC.netUpdate = true;
                CurrentAIState = AIState.Unused;
            }

            NPC.SimpleFlyMovement(newVel, 0.25f);
        }

        public void StayInFront()
        {
            NPC.TargetClosest(true);
            Vector2 offset = new(-150f, -200f);
            Vector2 newVel = NPC.DirectionTo(Player.Center - Vector2.UnitY * 250f) * 10f;
            Vector2 projVelocity = Vector2.Normalize(Player.Center - NPC.Center) * 8f;

            AITimer++;
            if (AITimer > 60f)
            {
                AITimer = 0f;
                for (int j = 0; j < 6; j++)
                {
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        break;
                    }
                    Vector2 newPos = (Vector2.UnitY * 6f).RotatedBy(MathHelper.PiOver2 * Main.rand.NextFloat());
                    newPos.X *= -NPC.direction;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, newPos, ProjectileID.CultistBossFireBall, NPC.GetAttackDamage_ForProjectiles(23f, 26f), 4f, Main.myPlayer);
                }
                AICounter++;
            }

            if (NPC.Distance(Player.Center + offset) > 40f)
            {
                NPC.SimpleFlyMovement(NPC.DirectionTo(Player.Center + offset).SafeNormalize(Vector2.Zero) * 12f, 0.25f);
            }

            if (AICounter >= 2f)
            {
                AICounter = 0f;
                AITimer = 0f;
                NPC.netUpdate = true;
                //CurrentAIState = AIState.Flying;
            }
            NPC.SimpleFlyMovement(newVel, 0.1f);
            NPC.rotation = NPC.velocity.X * 0.025f;
        }

        public void Spam()
        {

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


            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 drawPos = NPC.oldPos[i] + NPC.Size * 0.5f - screenPos;
                npcDrawColorTrail *= 0.75f;
                spriteBatch.Draw(texture, drawPos, NPC.frame, npcDrawColorTrail, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            }

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, npcDrawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
