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


namespace Project165.Content.NPCs.Bosses.ShadowSlime
{
    [AutoloadBossHead]
    public class ShadowSlime : ModNPC
    {
        public enum AIState : int
        {            
            Jumping = 0,
            Sliding = 1,
            ShootingAround = 2,
            JumpingFaster = 3,
            Flying = 4
        }

        public AIState CurrentAIState
        {
            get => (AIState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        public bool SecondPhase
        {
            get => NPC.ai[3] == 1f;
            set => NPC.ai[3] = value ? 1f : 0f;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Venom] = true;
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.width = 154;
            NPC.height = 100;
            NPC.damage = 60;
            NPC.npcSlots = 5f;
            NPC.alpha = 128;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 5f;
            NPC.defense = 28;
            NPC.aiStyle = -1;
            NPC.lifeMax = 32000;
            NPC.lavaImmune = true;
            NPC.GravityIgnoresLiquid = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.scale = 1f;
            AnimationType = NPCID.KingSlime;
            Music = MusicID.Boss3;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => true;

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
        private Player Player => Main.player[NPC.target];

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Player.dead || !Player.active || Vector2.Distance(Player.Center, NPC.Center) > 1000f)
            {
                NPC.TargetClosest();
            }

            // Emit Light
            Lighting.AddLight(NPC.Center, 2f, 2f, 2f);

            if (Player.dead || Vector2.Distance(Player.Center, NPC.Center) > 2000f)
            {
                NPC.noTileCollide = true;                
                NPC.velocity.Y += 0.25f;
                NPC.EncourageDespawn(10);
                return;
            }
            Player.AddBuff(BuffID.NoBuilding, 300);
            SecondPhase = NPC.life <= NPC.lifeMax * 0.5;

            if (SecondPhase && NPC.localAI[0] == 0f)
            {
                NPC.localAI[0] = 1f;
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ShadowMinion>());
            }

            switch (CurrentAIState)
            {
                case AIState.Jumping:
                    JumpAround();
                    break;
                case AIState.Sliding:
                    SlideAround();
                    break;
                case AIState.ShootingAround:
                    ShootAround();
                    break;
                case AIState.JumpingFaster:
                    JumpAroundFaster();
                    break;
                case AIState.Flying:
                    FlyAround();
                    break;
            }
        }

        #region Attacks
        private void JumpAround()
        {            
            float jumpTime = 30f;

            if (SecondPhase)
            {
                jumpTime = 25f;
            }

            NPC.TargetClosest();
            NPC.rotation = NPC.velocity.X * 0.025f;
            NPC.noTileCollide = false; 
            
            /*if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
            {
                AITimer = 0f;
                CurrentAIState = AIState.Flying;
                NPC.netUpdate = true;
            }*/

            if (NPC.velocity.Y == 0f)
            {
                NPC.TargetClosest();
                NPC.velocity.X *= 0.85f;
                AITimer++;                

                if (MathF.Abs(NPC.velocity.X) > -0.1f && MathF.Abs(NPC.velocity.X) < 0.1f)
                {
                    NPC.velocity.X = 0f;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient && AITimer > jumpTime)
                {
                    AITimer = 0f;            

                    if (Player.Center.Y < NPC.Center.Y - 100f)
                    {
                        NPC.velocity.Y -= 2.25f;
                    }

                    if (Player.Center.Y < NPC.Center.Y - 200f)
                    {
                        NPC.velocity.Y -= 6f;
                    }

                    NPC.velocity.Y -= 4f;
                    NPC.velocity.X = 7f * NPC.direction;

                    AICounter++;
                    if (AICounter > 4f)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            Vector2 newVelocity = Vector2.Normalize(Player.Center - NPC.Center).RotatedByRandom(MathHelper.ToRadians(15f)) * 8f;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, newVelocity, ModContent.ProjectileType<SlimeBossProj>(), NPC.GetAttackDamage_ForProjectiles(18f, 20f), 0f, Player.whoAmI);
                        }

                        AICounter = 0f;
                        CurrentAIState = AIState.JumpingFaster;
                        NPC.netUpdate = true;
                    }
                }
            }
            else
            {
                NPC.velocity.X *= 0.99f;
                if (NPC.direction < 0 && NPC.velocity.X > -1f)
                {
                    NPC.velocity.X = -5f;
                }
                if (NPC.direction > 0 && NPC.velocity.X < 1f)
                {
                    NPC.velocity.X = 5f;
                }
            }
        }

        public void ShootAround()
        {
            NPC.TargetClosest(true);
            float shootTime = 80f;
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            
            if (Main.expertMode)
            {
                shootTime *= 0.75f;
            }

            if (SecondPhase)
            {
                shootTime -= 15f;
            }

            NPC.velocity.X *= 0.85f;
            if (MathF.Abs(NPC.velocity.X) > -0.1f && MathF.Abs(NPC.velocity.X) < 0.1f)
            {
                NPC.velocity.X = 0f;
            }

            AITimer++;
            if (AITimer % shootTime == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int projAmount = 3;
                for (int i = 0; i < projAmount; i++)
                {
                    //Vector2 newVelocity = new Vector2(8f, 0f).RotatedBy(-i * MathHelper.TwoPi / projAmount, Vector2.Zero);
                    Vector2 newVelocity = new(Main.rand.Next(6, 9) * NPC.direction, Main.rand.Next(1, 9));
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, newVelocity, ModContent.ProjectileType<SlimeBossProj>(), NPC.GetAttackDamage_ForProjectiles(20f, 23f), 0f, Player.whoAmI);
                }
            }

            for (int i = 0; i < 2; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(300f, 300f).SafeNormalize(Vector2.UnitY) * 5f;
                Dust.NewDustPerfect(NPC.Center - speed * 5f, ModContent.DustType<GlowDust>(), speed / 4, 100, Color.DarkSlateBlue, 0.75f);
            }

            if (AITimer == 240f)
            {
                AITimer = 0f;
                AICounter = 0f;
                NPC.rotation = 0f;
                CurrentAIState = AIState.Sliding;
                NPC.netUpdate = true;
            }
        }

        public void SlideAround()
        {
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.TargetClosest(faceTarget: true);

            AITimer++;
            if (AITimer > 60f && AITimer < 200f)
            {
                NPC.velocity.X += 0.65f * NPC.direction;
            }
            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -8f, 8f);

            if (AITimer >= 220f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 newVelocity = Vector2.Normalize(Player.Center - NPC.Center).RotatedByRandom(MathHelper.ToRadians(15f)) * 8f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, newVelocity, ModContent.ProjectileType<SlimeBossProj>(), NPC.GetAttackDamage_ForProjectiles(18f, 20f), 0f, Player.whoAmI);
                }

                AITimer = 0f;
                NPC.velocity.X = 0f;
                CurrentAIState = AIState.Jumping;
                NPC.netUpdate = true;
            }
        }

        public void JumpAroundFaster()
        {
            NPC.rotation = NPC.velocity.X * 0.025f;
            if (NPC.velocity.Y == 0f)
            {
                NPC.TargetClosest();
                NPC.velocity.X *= 0.8f;

                if (MathF.Abs(NPC.velocity.X) > -0.1f && MathF.Abs(NPC.velocity.X) < 0.1f)
                {
                    NPC.velocity.X = 0f;
                }

                AITimer++;
                if (AITimer > 5f)
                {
                    AITimer = 0f;
                    NPC.velocity.Y -= 4f;

                    if (Player.Center.Y < NPC.position.Y - 100f)
                    {
                        NPC.velocity.Y -= 2f;
                    }
                    if (Player.Center.Y < NPC.position.Y - 200f)
                    {
                        NPC.velocity.Y -= 4f;
                    }

                    AICounter++;
                    NPC.velocity.X = 10 * NPC.direction;
                    NPC.netUpdate = true;
                }
            }
            else
            {
                NPC.velocity.X *= 0.98f;
                if (NPC.direction < 0 && NPC.velocity.X > -6f)
                {
                    NPC.velocity.X = -6f;
                }
                if (NPC.direction > 0 && NPC.velocity.X < 6f)
                {
                    NPC.velocity.X = 6f;
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && AICounter >= 3f && NPC.velocity.Y == 0f)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 newVelocity = Vector2.Normalize(Player.Center - NPC.Center).RotatedByRandom(MathHelper.ToRadians(15f)) * 8f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, newVelocity, ModContent.ProjectileType<SlimeBossProj>(), NPC.GetAttackDamage_ForProjectiles(18f, 20f), 0f, Player.whoAmI);
                }

                AICounter = 0f;
                AITimer = 0f;
                NPC.rotation = 0f;
                NPC.velocity.X = 0f;
                CurrentAIState = AIState.Sliding;
            }
        }


        public void FlyAround()
        {
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.TargetClosest();

            AITimer++;

            Vector2 newVelocity = Vector2.Normalize(Player.Center - NPC.Center) * 10f;
            NPC.SimpleFlyMovement(newVelocity, 0.05f);

            if (AITimer >= 30f)
            {
                AITimer = 0f;
                NPC.velocity = Vector2.Zero;
                NPC.noTileCollide = false;
                NPC.noGravity = false;
                CurrentAIState = AIState.Jumping;
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
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<DarkSlimeBag>()));

            npcLoot.Add(ItemDropRule.ByCondition(notExpert, ModContent.ItemType<ShadowBlade>(), 3));
            npcLoot.Add(ItemDropRule.ByCondition(notExpert, ModContent.ItemType<ShadowPike>(), 3));
            npcLoot.Add(ItemDropRule.ByCondition(notExpert, ModContent.ItemType<ShadowSlimeStaff>(), 3));
            npcLoot.Add(ItemDropRule.ByCondition(notExpert, ModContent.ItemType<ShadowEssence>(), 1, 8, 30));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Texture2D extraThingTexture = (Texture2D)ModContent.Request<Texture2D>("Project165/Assets/Images/RoquefortiExtra");
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / Main.npcFrameCount[Type] / 2);
            float intensity = 0.2f;

            
            Color npcDrawColor = NPC.GetAlpha(drawColor);
            Color drawColorThing = Color.Red with { A = 0, B = 255 };
            Color npcDrawColorTrail = Color.DarkBlue;

            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                npcDrawColorTrail *= 0.75f;
                spriteBatch.Draw(texture, NPC.oldPos[i] + NPC.Size / 2f - screenPos - new Vector2(0f, 9f), NPC.frame, npcDrawColorTrail * intensity, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
            }
            
            spriteBatch.Draw(texture, NPC.Center - screenPos - new Vector2(0f, 9f), NPC.frame, npcDrawColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
            spriteBatch.Draw(extraThingTexture, NPC.Center - screenPos - new Vector2(0f, 9f), null, drawColorThing, NPC.rotation + (float)Main.timeForVisualEffects / 8f, extraThingTexture.Size() / 2, 1.5f, spriteEffects, 0);
            return false;
        }
    }
}
