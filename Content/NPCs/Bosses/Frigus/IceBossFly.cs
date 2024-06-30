using Terraria;
using System.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Project165.Content.Dusts;
using Project165.Common.Systems;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.ItemDropRules;
using Project165.Content.Items.TreasureBags;
using Project165.Content.Projectiles.Hostile;
using Project165.Content.Items.Weapons.Melee;
using Project165.Content.Items.Weapons.Magic;
using Project165.Content.Items.Weapons.Ranged;

namespace Project165.Content.NPCs.Bosses.Frigus
{
    [AutoloadBossHead]
    public class IceBossFly : ModNPC
    {
        public enum AIState : int
        {
            Spawning = -1,
            PhaseOne_Sideways = 0,
            PhaseOne_Static = 1,
            PhaseOne_Bottom = 2,
            DeathAnimation = 3,
        }
        public AIState CurrentAIState
        {
            get => (AIState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        public ref float Timer => ref NPC.ai[1];
        public ref float ShootCounter => ref NPC.localAI[0];
        public bool PhaseTwo
        {
            get => NPC.ai[2] == 1f;
            set => NPC.ai[2] = value ? 1f : 0f;
        }

        public float acceleration = 0.2f;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.TrailCacheLength[Type] = 15;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers npcBestiaryDrawModifiers = new()
            { 
                PortraitScale = 1.25f,                
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, npcBestiaryDrawModifiers);
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            Main.npcFrameCount[Type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.width = 110;
            NPC.height = 98;            
            NPC.defense = 10;
            NPC.damage = 1;
            NPC.lifeMax = 25000;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.value = 120000f;
            NPC.npcSlots = 5f;
            NPC.aiStyle = -1;
            Music = MusicID.FrostMoon;
        }

        private Player Player => Main.player[NPC.target];

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(
            [
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow
            ]);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => true;

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            Conditions.NotExpert notExpertCondition = new();
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<IceBossBag>()));

            npcLoot.Add(ItemDropRule.ByCondition(notExpertCondition, ModContent.ItemType<Avalanche>(), 3));
            npcLoot.Add(ItemDropRule.ByCondition(notExpertCondition, ModContent.ItemType<IceShotgun>(), 3));
            npcLoot.Add(ItemDropRule.ByCondition(notExpertCondition, ModContent.ItemType<IceChakram>(), 3));
        }

        #region AI

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.dontTakeDamage = reader.ReadBoolean();
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Player.dead || !Player.active)
            {
                NPC.TargetClosest();
            }

            if (Player.dead)
            {
                NPC.velocity.Y += 0.15f;
                NPC.EncourageDespawn(10);
                return;
            }

            HandleRotation();

            if (NPC.localAI[0] == 0f)
            {
                NPC.localAI[0] = 1f;
                NPC.alpha = 255;
                NPC.velocity.Y = -5f;
                CurrentAIState = AIState.Spawning;
            }
            

            if (NPC.life < NPC.lifeMax * 0.65f)
            {
                PhaseTwo = true;
            }            

            switch (CurrentAIState)
            {
                case AIState.Spawning:
                    SpawnAnimation();
                    break;
                case AIState.PhaseOne_Sideways:
                    StayOnSideways();
                    break;
                case AIState.PhaseOne_Static:
                    StayOnPlace();
                    break;
                case AIState.PhaseOne_Bottom:
                    StayOnBottom();
                    break;
                case AIState.DeathAnimation:
                    DeathAnimation();
                    break;
            }
        }
        #endregion

        #region Attacks

        private void StayOnSideways()
        {
            float timeToShoot = 40f;

            if (PhaseTwo)
            {
                timeToShoot = 30f;
            }

            if (NPC.Distance(Player.Center) > 1000f)
            {
                NPC.Teleport(new Vector2(Player.Center.X + (250f * Player.direction), Player.Center.Y), 1);
            }
            
            Vector2 targetPosition = Player.Center + Vector2.UnitX * 250f - NPC.Center;
            targetPosition.Normalize();
            targetPosition *= 15f;

            NPC.SimpleFlyMovement(targetPosition, acceleration);

            Timer++;
            if (Timer % timeToShoot == 0 && Timer != 40f)
            {
                SummonProjectiles(velocity: 10f);
            }

            if (Timer > 300f)
            {
                Timer = 0f;
                CurrentAIState = AIState.PhaseOne_Static;
                NPC.netUpdate = true;
            }
        }

        private void StayOnPlace()
        {
            float timeToShoot = 12f;

            if (PhaseTwo)
            {
                timeToShoot = 10f;
            }

            NPC.velocity *= 0.5f;

            if (NPC.Distance(Player.Center) > 1300f)
            {
                NPC.Teleport(new Vector2(Player.Center.X + (400f * Player.direction), Player.Center.Y - 100f), 1);
            }

            if (Timer % timeToShoot == 0)
            {
                SummonProjectiles(velocity: 14f);
            }

            Timer++;
            if (Timer > 500f)
            {
                Timer = 0f;
                CurrentAIState = AIState.PhaseOne_Bottom;
                NPC.netUpdate = true;
            }        
        }

        private void StayOnBottom()
        {
            float timeToShoot = 100f;

            if (NPC.Distance(Player.Center) > 800f)
            {
                NPC.Teleport(Player.Center + Vector2.UnitY * 200f, 1);
            }

            if (PhaseTwo)
            {
                timeToShoot = 75f;
                if (Main.expertMode)
                {
                    timeToShoot = 70f;
                }
            }

            Vector2 targetPosition = Player.Center + Vector2.UnitY * 170f - NPC.Center;
            targetPosition.Normalize();
            targetPosition *= 15f;

            NPC.SimpleFlyMovement(targetPosition, acceleration);

            Timer++;
            if (Timer % timeToShoot == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowDust>(), 0, 0, 0, Color.LightCyan, 1.25f);
                }

                for (int i = 0; i < 16; i++)
                {
                    Vector2 spawnPos = new(Player.Center.X + Main.screenWidth / 2 - Main.screenWidth / 16 * i, Main.screenPosition.Y + Main.screenHeight + 500f);
                    Projectile.NewProjectile(null, spawnPos, Vector2.UnitY * -4f, ModContent.ProjectileType<IceBossProjectile>(), NPC.GetAttackDamage_ForProjectiles(30f, 23f), 0f, Main.myPlayer, 1f, 0f);
                }
            }

            if (Timer > 400f)
            {
                Timer = 0f;
                CurrentAIState = AIState.PhaseOne_Sideways;
                NPC.netUpdate = true;
            }
        }
        #endregion

        #region Misc
        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = 0 * frameHeight;
            if (PhaseTwo)
            {
                NPC.frame.Y = 1 * frameHeight;
            }
        }
        private void DeathAnimation()
        {
            Timer++;
            NPC.velocity *= 0.95f;
            NPC.rotation += 0.4f;

            if (NPC.scale < 2f)
            {
                NPC.scale += 0.005f;
            }
            if (NPC.alpha < 255)
            {
                NPC.alpha += 4;
            }
            if (NPC.alpha > 255)
            {
                NPC.alpha = 255;
            }
            if (Timer >= 100f)
            {                
                NPC.HitEffect();
                NPC.NPCLoot();
                NPC.active = false;
                NPC.netUpdate = true;
            }
        }

        private void SpawnAnimation()
        {
            NPC.dontTakeDamage = true;
            NPC.velocity *= 0.96f;
            if (NPC.alpha > 0)
            {
                NPC.alpha -= 2;
                for (int i = 0; i < 4; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(500f, 500f).SafeNormalize(Vector2.UnitY) * 40f;
                    Dust.NewDustPerfect(NPC.Center - speed * 5f, ModContent.DustType<CloudDust>(), speed / 2, 128, default, 0.75f);
                }
            }
            else
            {
                NPC.alpha = 0;
                NPC.dontTakeDamage = false;
                CurrentAIState = AIState.PhaseOne_Sideways;
                NPC.netUpdate = true;
            }
        }      
        
        private void SummonProjectiles(float velocity)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            Vector2 spawnVelocity = Player.Center - NPC.Center;

            float spawnDistance = spawnVelocity.Length();
            spawnDistance = velocity / spawnDistance;
            spawnVelocity.X *= spawnDistance;
            spawnVelocity.Y *= spawnDistance;

            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, spawnVelocity, ModContent.ProjectileType<IceBossProjectile>(), NPC.GetAttackDamage_ForProjectiles(30f, 25f), 0f, Main.myPlayer, 0f, 0f);
        }
        private void HandleRotation()
        {
            if (CurrentAIState == AIState.DeathAnimation)
            {
                return;
            }

            Vector2 npcPlayerPos = new(NPC.Center.X - Player.Center.X, NPC.Center.Y - Player.Center.Y);
            float rotAcceleration = 0.075f;
            float newRotation = npcPlayerPos.ToRotation() + MathHelper.PiOver2;

            if (newRotation < 0f)
            {
                newRotation += MathHelper.TwoPi;
            }
            if (newRotation > MathHelper.TwoPi)
            {
                newRotation -= MathHelper.TwoPi;
            }
            if (NPC.rotation < newRotation)
            {
                if (newRotation - NPC.rotation > MathHelper.Pi)
                {
                    NPC.rotation -= rotAcceleration;
                }
                else
                {
                    NPC.rotation += rotAcceleration;
                }
            }
            else if (NPC.rotation > newRotation)
            {
                if (NPC.rotation - newRotation > MathHelper.Pi)
                {
                    NPC.rotation += rotAcceleration;
                }
                else
                {
                    NPC.rotation -= rotAcceleration;
                }
            }

            NPC.rotation = newRotation;
        }

        #endregion

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedFrigus, -1);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("IceBossFly_Back").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("IceBossFly_Back").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("IceBossFly_Front").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("IceBossFly_Front").Type);
            }
        }

        public override bool CheckDead()
        {
            Timer = 0f;
            CurrentAIState = AIState.DeathAnimation;
            NPC.active = true;
            NPC.life = 1;
            NPC.dontTakeDamage = true;
            NPC.netUpdate = true;
            return false;            
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            SpriteEffects spriteEffects = SpriteEffects.None;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / Main.npcFrameCount[Type] / 2);
            Color npcDrawColorTrail = new(255 - NPC.alpha, 255 - NPC.alpha, 255 - NPC.alpha, 0);
            Color npcDrawColor = Color.White * NPC.Opacity;

            if (NPC.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                npcDrawColorTrail *= 0.6f;
                spriteBatch.Draw(texture, NPC.oldPos[i] + NPC.Size / 2f - screenPos, NPC.frame, npcDrawColorTrail, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
            }

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, npcDrawColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
            return false;
        }
    }
}
