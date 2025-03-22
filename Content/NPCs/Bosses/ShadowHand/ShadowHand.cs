using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Project165.Common.Systems;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Items.Materials;
using Terraria.GameContent.ItemDropRules;
using Project165.Content.Items.TreasureBags;
using Project165.Content.Items.Weapons.Melee;
using Project165.Content.Items.Weapons.Summon;
using Project165.Content.Projectiles.Hostile;
using System.IO;
using Terraria.Audio;
using System;


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
            NPCID.Sets.NPCBestiaryDrawModifiers npcBestiaryDrawModifiers = new()
            {
                PortraitScale = 1.25f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, npcBestiaryDrawModifiers);
            NPCID.Sets.BossBestiaryPriority.Add(Type);
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
            NPC.noTileCollide = false;
            NPC.GravityIgnoresLiquid = true;
            NPC.HitSound = SoundID.Tink with { PitchVariance = 1f };
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



            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Player.dead || !Player.active)
            {
                NPC.TargetClosest();
            }

            if (Player.dead || NPC.Distance(Player.Center) > 1000f)
            {
                CurrentAIState = AIState.None;
                NPC.noTileCollide = true;
                NPC.noGravity = true;
                NPC.velocity.Y += 0.2f;
                NPC.EncourageDespawn(12);
                return;
            }

            foreach (Player p in Main.player)
            {
                if (!p.active && p.Distance(NPC.Center) > 2000f)
                {
                    break;
                }

                if (p.grapCount > 0)
                {
                    p.RemoveAllGrapplingHooks();
                }
                if (p.mount.Active)
                {
                    p.mount.Dismount(p);
                }
            }

            SecondPhase = NPC.life < NPC.lifeMax * 0.5;
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
                NPC.localAI[0] = 1f;
                NPC.netUpdate = true;
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
                case AIState.None:
                    break;
            }
        }


        #region Attacks

        public void Jump()
        {
            float maxTime = 20f;
            float horizontalSpeed = 7f;
            float jumpSpeed = 6f;

            NPC.noGravity = false;
            NPC.noTileCollide = false;

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
            if (AICounter > 210f && NPC.velocity.Y == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                SoundEngine.PlaySound(SoundID.Roar with { PitchVariance = 1f }, NPC.position);
                CurrentAIState = AIState.AbovePlayer;
                AITimer = 0f;
                AICounter = 0f;
                NPC.netUpdate = true;
            }
        }

        public void GoAbovePlayer()
        {
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.TargetClosest();
            Vector2 playerCenter = Player.Center;
            Vector2 targetPos = playerCenter - Vector2.UnitY * 250f - NPC.Center;
            if (AICounter == 1f)
            {
                AITimer++;
                targetPos = Vector2.Normalize(Player.Center - NPC.Center) * 6f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, targetPos, 0.1f);
                if (Main.netMode != NetmodeID.MultiplayerClient && AITimer > 6f)
                {
                    AITimer = 0f;
                    CurrentAIState = AIState.Landing;
                    AICounter = 0f;
                    NPC.velocity = targetPos;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.position.Y < Player.position.Y - 200f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    AITimer = 0f;
                    AICounter = 1f;
                    NPC.netUpdate = true;
                }
            }
            else
            {
                targetPos = Vector2.Normalize(targetPos) * 20f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, targetPos, 0.2f);
            }
        }

        public void Land()
        {
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            if (NPC.position.Y + NPC.height >= Player.Center.Y || NPC.velocity.Y == 0f)
            {
                AITimer++;
                if (AITimer >= 5f)
                {
                    SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 1; i < 3; i++)
                        {
                            int j = i > 1 ? 1 : -1;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX * 6f * j, ModContent.ProjectileType<ShadowWave>(), NPC.GetAttackDamage_ForProjectiles(23f, 25f), 0f, Main.myPlayer);
                        }
                        if (!Collision.CanHitLine(NPC.Center, 0, 0, Player.Center, 0, 0))
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<ShadowStomp>(), NPC.GetAttackDamage_ForProjectiles(18f, 20f), 0f, Main.myPlayer);
                        }
                    }
                    CurrentAIState = SecondPhase ? AIState.JumpingFast : AIState.Jumping;
                    AITimer = 0f;
                    AICounter = 0f;
                    NPC.netUpdate = true;
                }

                if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                {
                    CurrentAIState = AIState.Flying;
                }
            }

            if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
            {
                NPC.noTileCollide = true;
            }

            NPC.velocity.Y += 1f;
            if (NPC.velocity.Y > 8f)
            {
                NPC.velocity.Y = 8f;
            }
        }

        public void FlyAround()
        {
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.direction = (NPC.velocity.X > 0f).ToDirectionInt();
            NPC.spriteDirection = NPC.direction;
            Vector2 targetPos = Player.Center + Vector2.UnitX * 32f - NPC.Center;
            targetPos.Y -= 24f;
            if (Main.netMode != NetmodeID.MultiplayerClient && targetPos.Length() < 200f && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
            {
                CurrentAIState = SecondPhase ? AIState.AbovePlayer : AIState.Jumping;
                AITimer = 0f;
                AICounter = 0f;
                NPC.netUpdate = true;
            }
            if (targetPos.Length() > 20f)
            {
                targetPos.Normalize();
                targetPos *= 20f;
            }
            NPC.SimpleFlyMovement(Vector2.Normalize(targetPos) * 20f, 0.2f);
        }

        public void JumpFaster()
        {
            if (NPC.velocity.Y == 0f)
            {
                NPC.TargetClosest();
                NPC.velocity.X *= 0.8f;
                AITimer++;
                if (AITimer > 5f)
                {
                    AITimer = 0f;
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
                    AICounter++;
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
            if (Main.netMode != NetmodeID.MultiplayerClient && AICounter >= 3f && NPC.velocity.Y == 0f)
            {
                if (SecondPhase)
                {
                    SoundEngine.PlaySound(SoundID.Roar, NPC.position);
                }

                CurrentAIState = SecondPhase ? AIState.AbovePlayer : AIState.Jumping;
                AITimer = 0f;
                AICounter = 0f;
                NPC.netUpdate = true;
            }
        }

        #endregion

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedShadowHand, -1);
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
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / Main.npcFrameCount[Type] / 2);
            float intensity = 0.4f;

            Color npcDrawColor = NPC.GetAlpha(drawColor);
            Color npcDrawColorTrail = Color.DarkBlue;

            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                npcDrawColorTrail *= 0.75f;
                spriteBatch.Draw(texture, NPC.oldPos[i] + NPC.Size / 2f - screenPos, NPC.frame, npcDrawColorTrail * intensity, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
            }

            float offset = MathF.Cos((float)Main.timeForVisualEffects * MathHelper.Pi / 15f);
            Color glowColor = Color.Purple * 0.2f * NPC.Opacity;
            glowColor *= 0.75f + 0.25f * offset;

            for (int i = 0; i < 8; i++)
            {
                Vector2 glowDrawPos = NPC.Center - screenPos + Vector2.One.RotatedBy(MathHelper.PiOver4 * i) * (4f + 1f * offset);
                Main.EntitySpriteDraw(texture, glowDrawPos, NPC.frame, glowColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects);
            }

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, npcDrawColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
            return false;
        }
    }
}
