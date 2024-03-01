using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.NPCs.Bosses.AncientSlime
{
    [AutoloadBossHead]
    public class AncientSlime : ModNPC
    {
        public enum AIState : int
        {            
            Jumping = 0,
            Landing = 1
        }

        public AIState CurrentAIState
        {
            get => (AIState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Venom] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Shimmer] = true;
        }

        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.width = 174;
            NPC.height = 120;
            NPC.damage = 1;
            NPC.npcSlots = 5f;
            NPC.alpha = 157;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 5f;
            NPC.defense = 20;
            NPC.aiStyle = -1;
            NPC.lifeMax = 32000;
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

        private Player Player => Main.player[NPC.target];

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

            switch (CurrentAIState)
            {
                case AIState.Jumping:
                    Jump();
                    break;
                case AIState.Landing:
                    Land();
                    break;
            }
        }

        private void Jump()
        {
            NPC.TargetClosest();
            if (NPC.velocity.Y == 0f)
            {
                AITimer++;
                NPC.velocity.X *= 0.8f;
                if (AITimer > 10f)
                {
                    AITimer = 0f;
                    NPC.velocity.Y -= 4f;

                    if (NPC.collideX)
                    {
                        NPC.velocity.Y -= 4f;
                    }

                    NPC.velocity.X = 12f * NPC.direction;
                }
            }
            else
            {
                NPC.velocity.X *= 0.98f;
                if (NPC.direction < 0 && NPC.velocity.X > -1f)
                {
                    NPC.velocity.X = -8f;
                }
                if (NPC.direction > 0 && NPC.velocity.X < 1f)
                {
                    NPC.velocity.X = 8f;
                }
            }
        }

        private void Land()
        {
            NPC.TargetClosest();
            AITimer++;            

            if (NPC.velocity.Y == 0f)
            {
                NPC.velocity.X *= 0.85f;
            }
            else
            {
                NPC.velocity.X *= 0.9f;
            }
            
            if (AITimer > 30f)
            {                
                AITimer = 0f;
                CurrentAIState = AIState.Jumping;
                NPC.netUpdate = true;
            }
        }
    }
}
