using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.ItemDropRules;
using Project165.Content.Items.Weapons.Melee;
using Project165.Content.Items.TreasureBags;
using Project165.Content.Projectiles.Hostile;

namespace Project165.Content.NPCs.Bosses
{
    [AutoloadBossHead]
    public class IceBossFly : ModNPC
    {
        public enum AIState : int
        {
            PhaseOne_SideWays = 0,
            PhaseOne_Static = 1,
            PhaseOne_NextPhase = 2,
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
        public bool PhaseTwo => NPC.ai[2] == 1f;

        public override void SetDefaults()
        {
            NPC.width = 110;
            NPC.height = 98;            
            NPC.defense = 10;
            NPC.damage = 0;
            NPC.lifeMax = 20000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.value = 120000f;
            NPC.npcSlots = 5f;
            NPC.aiStyle = -1;
        }
        private Player Player => Main.player[NPC.target];
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
        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Player.dead || !Player.active || Vector2.Distance(Player.Center, NPC.Center) > 2000f)
            {
                NPC.TargetClosest();
            }

            if (Player.dead)
            {
                NPC.velocity.Y += 0.025f;
                NPC.EncourageDespawn(10);
                return;
            }

            HandleRotation();

            switch (CurrentAIState)
            {
                case AIState.PhaseOne_SideWays:
                    StayOnSideways(); 
                    break;
                case AIState.PhaseOne_Static:
                    StayOnPlace();
                    break;
            }
        }
        private void StayOnPlace()
        {
            NPC.velocity *= 0.5f;

            if (NPC.Distance(Player.Center) > 1250f)
            {
                NPC.Teleport(new Vector2(Player.Center.X + (300f * Player.direction), Player.Center.Y - 100f), 1);
            }

            if (Timer % 12f == 0)
            {
                SummonProjectiles(velocity: 15f);
            }

            Timer++;
            if (Timer > 600f)
            {
                Timer = 0f;
                CurrentAIState = AIState.PhaseOne_SideWays;
                NPC.netUpdate = true;
            }        
        }
        private void StayOnSideways()
        {
            if (NPC.Distance(Player.Center) > 800f)
            {
                NPC.Teleport(new Vector2(Player.Center.X + (250f * Player.direction), Player.Center.Y), 1);
            }

            float acceleration = 0.18f;
            Vector2 targetPosition = Player.Center + new Vector2(200f, 0f) - NPC.Center;

            float npcPlayerDistance = targetPosition.Length();
            npcPlayerDistance = 15f / npcPlayerDistance;
            targetPosition.X *= npcPlayerDistance;
            targetPosition.Y *= npcPlayerDistance;

            NPC.SimpleFlyMovement(targetPosition, acceleration);

            Timer++;
            if (Timer % 40f == 0)
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

        private void StayOnTop()
        {

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
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<IceBossBag>()));

            LeadingConditionRule notExpertConditionRule = new(new Conditions.NotExpert());
            notExpertConditionRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<IceChakram>(), 3));
            npcLoot.Add(notExpertConditionRule);
        }
    }
}
