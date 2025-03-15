using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Project165.Content.Dusts;
using Project165.Common.Systems;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Items.Materials;
using Terraria.GameContent.ItemDropRules;
using Project165.Content.Items.TreasureBags;
using Project165.Content.Items.Weapons.Melee;
using Project165.Content.Items.Weapons.Summon;
using Project165.Content.Projectiles.Hostile;
using System.IO;
using Project165.Utilites;
using Terraria.Audio;


namespace Project165.Content.NPCs.Bosses.ShadowHand
{
    [AutoloadBossHead]
    public class ShadowHand : ModNPC
    {
        public enum AIState : int
        {
            None = -1,
            Jumping = 0,
            AbovePlayer = 1,
            Landing = 2,
            Flying = 3,
            JumpingFast = 4
        }

        public AIState CurrentAIState
        {
            get => (AIState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Venom] = true;
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.width = 64;
            NPC.height = 64;
            NPC.damage = 60;
            NPC.npcSlots = 5f;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 6f;
            NPC.defense = 28;
            NPC.aiStyle = -1;
            NPC.lifeMax = 32000;
            NPC.lavaImmune = true;
            NPC.GravityIgnoresLiquid = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.scale = 1.25f;
            Music = MusicID.Boss3;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;
            return true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }

        public ref float AITimer => ref NPC.ai[1];
        public ref float AICounter => ref NPC.ai[2];

        public bool SecondPhase
        {
            get => NPC.ai[3] == 1f;
            set => NPC.ai[3] = value ? 1f : 0f;
        }

        private Player Player => Main.player[NPC.target];


        public override void AI()
        {
            NPC.dontTakeDamage = false;
            NPC.noGravity = false;

            if (Player.dead)
            {
                CurrentAIState = AIState.None;
                NPC.EncourageDespawn(12);
                return;
            }

            if (SecondPhase && NPC.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Roar, NPC.position);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int minionNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ShadowMinion>());
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, minionNPC);
                    }
                }
            }

            switch (CurrentAIState)
            {
                case AIState.Jumping:
                    Jump();
                    break;
                case AIState.AbovePlayer:
                    GoAbovePlayer();
                    break;
                case AIState.Landing:
                    Land();
                    break;
                case AIState.Flying:
                    FlyAround();
                    break;
                case AIState.JumpingFast:
                    JumpFaster();
                    break;
                default:
                    NPC.damage = 0;
                    NPC.life = NPC.lifeMax;
                    NPC.dontTakeDamage = true;
                    NPC.noTileCollide = true;
                    NPC.velocity.X *= 0f;
                    NPC.velocity.Y += 0.1f;
                    break;
            }
        }


        #region Attacks

        public void Jump()
        {
            float maxTime = 45f;
            float horizontalSpeed = 7f;
            float jumpSpeed = 4f;

            if (Main.netMode != NetmodeID.MultiplayerClient && Vector2.Distance(Player.Center, NPC.Center) > 600f)
            {
                CurrentAIState = AIState.Flying;
                AITimer = 0f;
                AICounter = 0f;
                NPC.netUpdate = true;
            }

            if (NPC.velocity.Y == 0f)
            {
                NPC.TargetClosest();
                NPC.velocity.X *= 0.85f;
                AITimer++;

                if (!Collision.CanHit(NPC.Center, 1, 1, Player.Center, 1, 1))
                {
                    jumpSpeed += 2f;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient && AITimer > maxTime)
                {
                    AITimer = 0f;
                    NPC.velocity.Y -= jumpSpeed;
                    NPC.velocity.X = horizontalSpeed * NPC.direction;
                    NPC.netUpdate = true;
                }
            }
            else
            {
                NPC.velocity.X *= 0.99f;
                if (NPC.direction < 0 && NPC.velocity.X > -1f)
                {
                    NPC.velocity.X = -1f;
                }
                if (NPC.direction > 0 && NPC.velocity.X < 1f)
                {
                    NPC.velocity.X = 1f;
                }
            }
            AICounter++;
            if (AICounter > 210.0 && NPC.velocity.Y == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                switch (Main.rand.Next(3))
                {
                    case 0:
                        CurrentAIState = AIState.Flying;
                        break;
                    case 1:
                        CurrentAIState = AIState.AbovePlayer;
                        NPC.noTileCollide = true;
                        NPC.velocity.Y = -8f;
                        break;
                    case 2:
                        CurrentAIState = AIState.JumpingFast;
                        NPC.ai[0] = 6f;
                        break;
                    default:
                        CurrentAIState = AIState.Jumping;
                        break;
                }
                AITimer = 0f;
                AICounter = 0f;
                NPC.netUpdate = true;
            }
        }

        public void GoAbovePlayer()
        {
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.direction = -(NPC.velocity.X < 0f).ToDirectionInt();
            NPC.spriteDirection = NPC.direction;
            NPC.TargetClosest();
            Vector2 playerCenter = Player.Center;
            Vector2 targetPos = playerCenter - Vector2.UnitY * 350f - NPC.Center;
            if (AICounter == 1f)
            {
                AITimer++;
                targetPos = Player.Center - NPC.Center;
                targetPos.Normalize();
                targetPos *= 8f;
                NPC.velocity = (NPC.velocity * 4f + targetPos) / 5f;
                if (Main.netMode != NetmodeID.MultiplayerClient && AITimer > 6f)
                {
                    AITimer = 0f;
                    CurrentAIState = AIState.Landing;
                    AICounter = 0f;
                    NPC.velocity = targetPos;
                    NPC.netUpdate = true;
                }
            }
            else if (Math.Abs(NPC.Center.X - Player.Center.X) < 40f && NPC.Center.Y < Player.Center.Y - 300f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    CurrentAIState = AIState.Jumping;
                    AICounter = 1f;
                    NPC.netUpdate = true;
                }
            }
            else
            {
                targetPos.Normalize();
                targetPos *= 12f;
                NPC.velocity = (NPC.velocity * 5f + targetPos) / 6f;
            }
        }

        public void Land()
        {
            if (AICounter == 0f && Collision.CanHit(NPC.Center, 1, 1, Player.Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
            {
                AICounter = 1f;
            }
            if (NPC.position.Y + NPC.height >= Player.position.Y || NPC.velocity.Y <= 0f)
            {
                AITimer++;
                SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                if (Main.netMode != NetmodeID.MultiplayerClient && AITimer > 10f)
                {
                    for (int i = 1; i < 3; i++)
                    {
                        int j = i > 1 ? 1 : -1;  
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX * 6f * j, ModContent.ProjectileType<ShadowWave>(), NPC.GetAttackDamage_ForProjectiles(23f, 25f), 0f, Main.myPlayer);
                    }

                    CurrentAIState = AIState.Jumping;
                    AITimer = 0f;
                    AICounter = 0f;
                    NPC.netUpdate = true;
                    if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    {
                        CurrentAIState = AIState.Flying;
                    }
                }
            }
            else if (AICounter == 0f)
            {
                NPC.noTileCollide = true;
                NPC.noGravity = true;
            }
            NPC.velocity.Y += 0.2f;
            if (NPC.velocity.Y > 16f)
            {
                NPC.velocity.Y = 16f;
            }
        }

        public void FlyAround()
        {
            NPC.direction = (NPC.velocity.X > 0f).ToDirectionInt();
            NPC.spriteDirection = NPC.direction;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            Vector2 targetPos = Player.Center - NPC.Center;
            targetPos.Y -= 4f;
            if (Main.netMode != NetmodeID.MultiplayerClient && targetPos.Length() < 200f && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
            {
                CurrentAIState = AIState.Jumping;
                AITimer = 0f;
                AICounter = 0f;
                NPC.netUpdate = true;
            }
            if (targetPos.Length() > 10f)
            {
                targetPos.Normalize();
                targetPos *= 10f;
            }
            NPC.velocity = (NPC.velocity * 4f + targetPos) / 5f;
        }

        public void JumpFaster()
        {
            if (NPC.velocity.Y == 0f)
            {
                NPC.TargetClosest();
                NPC.velocity.X *= 0.8f;
                NPC.ai[1] += 1f;
                if (NPC.ai[1] > 5f)
                {
                    NPC.ai[1] = 0f;
                    NPC.velocity.Y -= 4f;
                    if (Player.position.Y + Player.height < NPC.Center.Y)
                    {
                        NPC.velocity.Y -= 1.25f;
                    }
                    if (Player.position.Y + Player.height < NPC.Center.Y - 40f)
                    {
                        NPC.velocity.Y -= 1.5f;
                    }
                    if (Player.position.Y + Player.height < NPC.Center.Y - 80f)
                    {
                        NPC.velocity.Y -= 1.75f;
                    }
                    if (Player.position.Y + Player.height < NPC.Center.Y - 120f)
                    {
                        NPC.velocity.Y -= 2f;
                    }
                    if (Player.position.Y + Player.height < NPC.Center.Y - 160f)
                    {
                        NPC.velocity.Y -= 2.25f;
                    }
                    if (Player.position.Y + Player.height < NPC.Center.Y - 200f)
                    {
                        NPC.velocity.Y -= 2.5f;
                    }
                    if (!Collision.CanHit(NPC.Center, 1, 1, Player.Center, 1, 1))
                    {
                        NPC.velocity.Y -= 2f;
                    }
                    NPC.velocity.X = 12 * NPC.direction;
                    NPC.ai[2] += 1f;
                    NPC.netUpdate = true;
                }
            }
            else
            {
                NPC.velocity.X *= 0.98f;
                if (NPC.direction < 0 && NPC.velocity.X > -8f)
                {
                    NPC.velocity.X = -8f;
                }
                if (NPC.direction > 0 && NPC.velocity.X < 8f)
                {
                    NPC.velocity.X = 8f;
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[2] >= 3f && NPC.velocity.Y == 0f)
            {
                CurrentAIState = AIState.Jumping;
                AITimer = 0f;
                AICounter = 0f;
                NPC.netUpdate = true;
            }
        }

        #endregion

        public override void OnKill()
        {            
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedShadowSlime, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            Conditions.NotExpert notExpert = new();
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ShadowHandBag>()));

            npcLoot.Add(ItemDropRule.ByCondition(notExpert, ModContent.ItemType<ShadowBlade>(), 3));
            npcLoot.Add(ItemDropRule.ByCondition(notExpert, ModContent.ItemType<ShadowPike>(), 3));
            npcLoot.Add(ItemDropRule.ByCondition(notExpert, ModContent.ItemType<ShadowSlimeStaff>(), 3));
            npcLoot.Add(ItemDropRule.ByCondition(notExpert, ModContent.ItemType<ShadowEssence>(), 1, 8, 30));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Texture2D extraThingTexture = (Texture2D)ModContent.Request<Texture2D>(Project165Utils.ImagesPath + "RoquefortiExtra");
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / Main.npcFrameCount[Type] / 2);
            float intensity = 0.2f;
            
            Color npcDrawColor = NPC.GetAlpha(drawColor);
            Color drawColorThing = Color.Red with { A = 0, B = 255 };
            Color npcDrawColorTrail = Color.DarkBlue;

            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                npcDrawColorTrail *= 0.75f;
                spriteBatch.Draw(texture, NPC.oldPos[i] + NPC.Size / 2f - screenPos, NPC.frame, npcDrawColorTrail * intensity, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
            }
            
            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, npcDrawColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
            return false;
        }
    }
}
