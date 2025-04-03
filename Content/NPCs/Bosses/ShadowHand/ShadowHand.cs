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
using Project165.Content.Items.Weapons.Magic;
using Terraria.Map;


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
            NPC.width = 40;
            NPC.height = 64;
            NPC.damage = 75;
            NPC.npcSlots = 5f;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 6f;
            NPC.defense = 28;
            NPC.aiStyle = -1;
            NPC.lifeMax = 32000;
            NPC.lavaImmune = true;
            NPC.noTileCollide = false;
            NPC.GravityIgnoresLiquid = true;
            NPC.GravityIgnoresSpace = true;
            NPC.HitSound = SoundID.Tink with { PitchVariance = 1f };
            NPC.DeathSound = SoundID.NPCDeath43;
            NPC.scale = 1.75f;
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
        public ref float AITimer2 => ref NPC.ai[2];

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

            if (Player.dead || NPC.Distance(Player.Center) > 2500f)
            {
                CurrentAIState = AIState.None;
                NPC.noTileCollide = true;
                NPC.noGravity = true;
                NPC.velocity.Y += 0.2f;
                NPC.EncourageDespawn(12);
                return;
            }


            DisableMountsAndHooks();

            if (NPC.noTileCollide && !Player.dead)
            {
                if (NPC.velocity.Y > 0f && NPC.Bottom.Y > Player.Top.Y)
                {
                    NPC.noTileCollide = false;
                }
                else if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Player.Center, 1, 1) && !Collision.SolidTiles(NPC.position, NPC.width, NPC.height))
                {
                    NPC.noTileCollide = false;
                }
            }

            SecondPhase = NPC.life < NPC.lifeMax * 0.5;
            if (NPC.localAI[0] == 0f)
            {
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
            float waitingTime = SecondPhase ? 8f : 10f;
            float maxTime = 300f;
            float horizontalSpeed = 7f;
            float jumpSpeed = 6f;

            NPC.noGravity = false;
            AITimer++;
            if (NPC.velocity.Y == 0f)
            {
                NPC.noTileCollide = false;
                NPC.velocity.X *= 0.8f;
                AITimer2++;
                if (AITimer2 > waitingTime)
                {
                    NPC.noTileCollide = true;
                    NPC.TargetClosest();
                    AITimer2 = 0f;
                    NPC.velocity.X = horizontalSpeed * NPC.direction;
                    NPC.velocity.Y -= jumpSpeed;

                    if (Player.Center.Y < NPC.Center.Y - 200f)
                    {
                        NPC.velocity.Y -= 4f;
                    }
                    if (!Collision.CanHit(NPC.Center, 1, 1, Player.Center, 1, 1))
                    {
                        NPC.velocity.Y -= 2f;
                    }
                }
            }

            if (AITimer >= maxTime && NPC.velocity.Y == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                SoundEngine.PlaySound(SoundID.Roar, NPC.position);
                CurrentAIState = AIState.JumpingFast;
                AITimer = 0f;
                AITimer2 = 0f;
                NPC.netUpdate = true;
            }
        }

        public void GoAbovePlayer()
        {
            if (NPC.velocity.Y == 0f)
            {
                NPC.TargetClosest();
                //SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    SoundEngine.PlaySound(SoundID.Roar, NPC.position);
                    CurrentAIState = AIState.JumpingFast;
                    AITimer = 0f;
                    AITimer2 = 0f;
                    NPC.netUpdate = true;
                }
            }
        }

        public void Land()
        {

        }

        public void FlyAround()
        {

        }

        public void JumpFaster()
        {
            if (NPC.velocity.Y == 0f)
            {
                NPC.noTileCollide = false;
                NPC.TargetClosest();
                NPC.velocity.X *= 0.8f;
                AITimer++;
                if (AITimer > 5f)
                {
                    NPC.noTileCollide = true;
                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(2))
                    {
                        SoundEngine.PlaySound(SoundID.Item14, NPC.position);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<ShadowStomp>(), NPC.GetAttackDamage_ForProjectiles(5f, 8f), 0f, Main.myPlayer);
                    }

                    AITimer = 0f;
                    NPC.velocity.Y -= 4f;
                    if (Player.Center.Y < NPC.Center.Y)
                    {
                        NPC.velocity.Y -= 2f;
                    }
                    if (Player.Center.Y < NPC.Center.Y - 200f)
                    {
                        NPC.velocity.Y -= 4f;
                    }
                    if (!Collision.CanHit(NPC.Center, 1, 1, Player.Center, 1, 1))
                    {
                        NPC.velocity.Y -= 2f;
                    }
                    NPC.velocity.X = 12 * NPC.direction;
                    AITimer2++;
                    NPC.netUpdate = true;
                }
            }
            else
            {
                NPC.velocity.X *= 0.98f;
                NPC.velocity.X = 8f * NPC.direction;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && AITimer2 >= 3f && NPC.velocity.Y == 0f)
            {
                CurrentAIState = AIState.Jumping;
                AITimer = 0f;
                AITimer2 = 0f;
                NPC.netUpdate = true;
            }
        }

        #endregion

        #region Misc
        public void DisableMountsAndHooks()
        {
            foreach (Player p in Main.player)
            {
                if (!p.active && p.Distance(NPC.Center) > 4000f)
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
        }

        #endregion

        public override bool? CanFallThroughPlatforms() => NPC.target >= 0 && Player.position.Y > NPC.position.Y + NPC.height;

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedShadowHand, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            Conditions.NotExpert notExpert = new();
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ShadowHandBag>()));

            npcLoot.Add(ItemDropRule.ByCondition(notExpert, ModContent.ItemType<ShadowBlade>(), 4));
            npcLoot.Add(ItemDropRule.ByCondition(notExpert, ModContent.ItemType<ShadowPike>(), 4));
            npcLoot.Add(ItemDropRule.ByCondition(notExpert, ModContent.ItemType<ShadowSlimeStaff>(), 4));
            npcLoot.Add(ItemDropRule.ByCondition(notExpert, ModContent.ItemType<BlackHoleStaff>(), 4));
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
