using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Common.Systems;
using Project165.Content.Dusts;
using Project165.Content.Items.Materials;
using Project165.Content.Items.TreasureBags;
using Project165.Content.Items.Weapons.Melee;
using Project165.Content.Items.Weapons.Summon;
using Project165.Content.Projectiles.Hostile;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Project165.Content.NPCs.Bosses.ShadowSlime
{
    [AutoloadBossHead]
    public class ShadowSlime : ModNPC
    {
        public enum AIState : int
        {            
            Jumping = 0,
            ClimbingUp = 1,
            Landing = 2,
            Sliding = 3,
            ShootingAround = 4
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
            NPC.width = 174;
            NPC.height = 120;
            NPC.damage = 60;
            NPC.npcSlots = 5f;
            NPC.alpha = 128;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 5f;
            NPC.defense = 28;
            NPC.aiStyle = -1;
            NPC.lifeMax = 32000;
            NPC.lavaImmune = true;
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
            Lighting.AddLight(NPC.Center, 1f, 1f, 1f);

            if (Player.dead || Vector2.Distance(Player.Center, NPC.Center) > 2000f)
            {
                NPC.noTileCollide = true;                
                NPC.velocity.Y += 0.25f;
                NPC.EncourageDespawn(10);
                return;
            }

            if (NPC.life <= NPC.lifeMax * 0.5)
            {
                SecondPhase = true;
            }

            switch (CurrentAIState)
            {
                case AIState.Jumping:
                    Jump();
                    break;
                case AIState.ClimbingUp:
                    ClimbUp();
                    break;
                case AIState.Landing:
                    Land();
                    break;
                case AIState.Sliding:
                    Slide();
                    break;
                case AIState.ShootingAround:
                    ShootAround();
                    break;
            }
        }

        #region Attacks
        private void Jump()
        {
            float jumpTime = 30f;

            if (SecondPhase)
            {
                jumpTime = 25f;
            }

            NPC.rotation = NPC.velocity.X * 0.025f;
            NPC.noTileCollide = false; 
            
            if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
            {
                AITimer = 0f;
                CurrentAIState = AIState.ClimbingUp;
                NPC.netUpdate = true;
            }

            if (NPC.velocity.Y == 0f)
            {
                NPC.TargetClosest();
                NPC.velocity.X *= 0.9f;
                AITimer++;                

                if (MathF.Abs(NPC.velocity.X) > -0.1f && MathF.Abs(NPC.velocity.X) < 0.1f)
                {
                    NPC.velocity.X = 0f;
                }

                if (AITimer > jumpTime)
                {
                    AITimer = 0f;             
                    
                    // Check if the boss is stuck sideways

                    if (NPC.collideX)
                    {
                        NPC.velocity.Y -= 4f;
                    }

                    if (Player.Center.Y < NPC.Center.Y - 160f)
                    {
                        NPC.velocity.Y -= 2.25f;
                    }

                    NPC.velocity.Y -= 4f;
                    NPC.velocity.X = 7f * NPC.direction;

                    AICounter++;
                    if (AICounter > 4f)
                    {
                        AICounter = 0f;
                        CurrentAIState = AIState.ShootingAround;
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

        private void ClimbUp()
        {
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.TargetClosest();

            Vector2 targetPos = Player.Center + new Vector2(0f, -400f) - NPC.Center;
            NPC.rotation = NPC.velocity.X * 0.025f;
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
                    AICounter = 0f;
                    CurrentAIState = AIState.Landing;
                    NPC.velocity = targetPos;
                    NPC.netUpdate = true;
                }
            }
            else if (MathF.Abs(NPC.Center.X - Player.Center.X) < 40f && NPC.Center.Y < Player.Center.Y - 300f)
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
                targetPos.Normalize();
                targetPos *= 12f;
                NPC.velocity = (NPC.velocity * 5f + targetPos) / 6f;
            }
        }

        private void Land()
        {
            NPC.rotation = NPC.velocity.X * 0.025f;


            if (NPC.Center.Y != Player.position.Y || NPC.velocity.Y <= 0f)
            {
                AITimer++;
                NPC.noTileCollide = true;
                NPC.noGravity = true;

                if (Main.netMode != NetmodeID.MultiplayerClient && AITimer > 16f)
                {
                    AITimer = 0f;
                    AICounter = 0f;
                    CurrentAIState = AIState.ShootingAround;
                    NPC.netUpdate = true;
                }            
            }

            NPC.velocity.X *= 0.85f;
            if (MathF.Abs(NPC.velocity.X) > -0.1f && MathF.Abs(NPC.velocity.X) < 0.1f)
            {
                NPC.velocity.X = 0f;
            }

            NPC.velocity.Y += 0.3f;
            if (NPC.velocity.Y > 16f)
            {
                NPC.velocity.Y = 16f;
            }      
        }

        public void ShootAround()
        {
            float shootTime = 80f;
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            
            if (Main.expertMode)
            {
                shootTime *= 0.8f;
            }

            if (SecondPhase)
            {
                shootTime -= 10f;
            }

            NPC.velocity.X *= 0.85f;
            if (MathF.Abs(NPC.velocity.X) > -0.1f && MathF.Abs(NPC.velocity.X) < 0.1f)
            {
                NPC.velocity.X = 0f;
            }

            AITimer++;
            if (AITimer % shootTime == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int projAmount = 5;
                for (int i = 0; i < projAmount; i++)
                {
                    Vector2 newVelocity = new Vector2(10f, 0f).RotatedBy(-i * MathHelper.TwoPi / projAmount, Vector2.Zero);
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
                CurrentAIState = AIState.Sliding;
                NPC.rotation = 0;
                NPC.netUpdate = true;
            }
        }

        public void Slide()
        {
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.TargetClosest();

            AITimer++;
            if (AITimer > 30f && AITimer < 120f)
            {
                NPC.velocity.X += 0.9f * NPC.direction;
            }
            if (NPC.velocity.X > 10f)
            {
                NPC.velocity.X = 10f;
            }
            else if (NPC.velocity.X < -10f)
            {
                NPC.velocity.X = -10f;
            }
            if (AITimer >= 140f)
            {
                AITimer = 0f;
                NPC.velocity.X = 0f;
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

            if (CurrentAIState is AIState.ClimbingUp or AIState.Landing)
            {
                intensity *= 3f;
            }
            
            Color npcDrawColor = NPC.GetAlpha(drawColor);
            Color drawColorThing = Color.Red with { A = 0, B = 255 };
            Color npcDrawColorTrail = Color.DarkBlue;

            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                npcDrawColorTrail *= 0.75f;
                spriteBatch.Draw(texture, NPC.oldPos[i] + NPC.Size / 2f - screenPos + new Vector2(0f, 2f), NPC.frame, npcDrawColorTrail * intensity, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
            }
            
            spriteBatch.Draw(texture, NPC.Center - screenPos + new Vector2(0f, 2f), NPC.frame, npcDrawColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
            spriteBatch.Draw(extraThingTexture, NPC.Center - screenPos + new Vector2(0f, 2f), null, drawColorThing, NPC.rotation + (float)Main.timeForVisualEffects / 8f, extraThingTexture.Size() / 2, 1.5f, spriteEffects, 0);
            return false;
        }
    }
}
