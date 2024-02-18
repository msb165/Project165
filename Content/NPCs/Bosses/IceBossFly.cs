using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.ItemDropRules;
using Project165.Content.Items.Weapons.Melee;
using Project165.Content.Items.TreasureBags;
using Project165.Content.Projectiles.Hostile;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Project165.Content.Dusts;
using Terraria.Graphics.Effects;
using Project165.Content.Items.Weapons.Magic;

namespace Project165.Content.NPCs.Bosses
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
            PhaseTwo_Circling = 3,
            PhaseTwo_Dashing = 4,
            PhaseTwo_Dying = 5,
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
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
            NPCID.Sets.TrailCacheLength[Type] = 15;
            NPCID.Sets.TrailingMode[Type] = 1;
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
            NPC.damage = 0;
            NPC.lifeMax = 20000;
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
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow
            });
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
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return true;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            Conditions.NotExpert notExpertCondition = new();
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<IceBossBag>()));

            npcLoot.Add(ItemDropRule.ByCondition(notExpertCondition, ModContent.ItemType<Avalanche>(), 3));
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Player.dead || !Player.active || Vector2.Distance(Player.Center, NPC.Center) > 2000f)
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
            }
        }

        #region Behavior

        private void SpawnAnimation()
        {
            NPC.velocity *= 0.97f;
            if (NPC.alpha > 0)
            {
                NPC.alpha -= 2;
                for (int i = 0; i < 5; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(500f, 500f).SafeNormalize(Vector2.UnitY) * 40f;
                    Dust.NewDustPerfect(NPC.Center - speed * 5f, ModContent.DustType<GlowDust>(), speed / 2, 0, Color.Cyan, 0.5f);
                }
            }
            else
            {
                NPC.alpha = 0;
                CurrentAIState = AIState.PhaseOne_Sideways;
                NPC.netUpdate = true;
            }
        }

        private void StayOnSideways()
        {
            float timeToShoot = 40f;

            if (PhaseTwo)
            {
                timeToShoot = 30f;
            }

            if (NPC.Distance(Player.Center) > 800f)
            {
                NPC.Teleport(new Vector2(Player.Center.X + (250f * Player.direction), Player.Center.Y), 1);
            }
            
            Vector2 targetPosition = Player.Center + new Vector2(250f, 0f) - NPC.Center;

            float npcPlayerDistance = targetPosition.Length();
            npcPlayerDistance = 15f / npcPlayerDistance;
            targetPosition.X *= npcPlayerDistance;
            targetPosition.Y *= npcPlayerDistance;

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
                NPC.Teleport(Player.Center + new Vector2(0f, 200f), 1);
            }

            if (PhaseTwo)
            {
                timeToShoot = 75f;
                if (Main.expertMode)
                {
                    timeToShoot = 70f;
                }
            }

            Vector2 targetPosition = Player.Center + new Vector2(0f, 200f) - NPC.Center;

            float npcPlayerDistance = targetPosition.Length();
            npcPlayerDistance = 15f / npcPlayerDistance;
            targetPosition.X *= npcPlayerDistance;
            targetPosition.Y *= npcPlayerDistance;

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
                    Vector2 spawnPos = new(Player.Center.X + Main.screenWidth / 2 - Main.screenWidth / 16 * i, Main.screenPosition.Y + Main.screenHeight + 800f);
                    Projectile.NewProjectile(null, spawnPos, new Vector2(0f, -4f), ModContent.ProjectileType<IceBossProjectile>(), NPC.GetAttackDamage_ForProjectiles(30f, 25f), 0f, Main.myPlayer, 1f, 0f);
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
        

        public override void FindFrame(int frameHeight)
        {
            if (PhaseTwo)
            {
                NPC.frame.Y = 1 * frameHeight;
            }
            else
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            SpriteEffects spriteEffects = SpriteEffects.None;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / Main.npcFrameCount[Type] / 2);
            Color npcDrawColorTrail = new(255 - NPC.alpha, 255 - NPC.alpha, 255 - NPC.alpha, 0);
            Color npcDrawColor = new(255 - NPC.alpha, 255 - NPC.alpha, 255 - NPC.alpha, 255 - NPC.alpha);


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
