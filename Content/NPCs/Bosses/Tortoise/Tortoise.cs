using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.NPCs.Bosses.Tortoise
{
    [AutoloadBossHead]
    public class Tortoise : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 8;
            NPCID.Sets.TrailCacheLength[Type] = 20;
            NPCID.Sets.TrailingMode[Type] = 0;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 46;
            NPC.height = 32;
            NPC.scale = 3.5f;
            NPC.lifeMax = 25500;
            NPC.defense = 200;
            NPC.damage = 25;
            NPC.npcSlots = 5f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit24 with { Pitch = -1f };
            NPC.DeathSound = SoundID.NPCDeath27;
            NPC.boss = true;
            //AnimationType = NPCID.GiantTortoise;
        }

        Player Player => Main.player[NPC.target];
        public enum AIState : int
        {
            Walking = 0,
            PreparingToFly = 1,
            Flying = 2
        }

        public AIState CurrentAIState
        {
            get => (AIState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        public float AITimer
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        public override void AI()
        {
            if (NPC.target < 0 || Player.dead || NPC.target == 255 || !Player.active)
            {
                NPC.TargetClosest();
            }            
            
            switch (CurrentAIState)
            {
                case AIState.Walking:
                    WalkBehaviour();
                    break;
                case AIState.PreparingToFly:
                    PrepareToFly(); 
                    break;
                case AIState.Flying:
                    FlyAround(); 
                    break;
            }
        }

        public void WalkBehaviour()
        {
            NPC.TargetClosest(faceTarget: true);            
            NPC.spriteDirection = NPC.direction;

            AITimer++;

            NPC.velocity.X += 0.25f * NPC.direction;
            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -1.5f, 1.5f);

            if (AITimer >= 100f)
            {
                NPC.netUpdate = true;
                AITimer = 0f;
                CurrentAIState = AIState.PreparingToFly;
            }
        }

        public void PrepareToFly()
        {
            NPC.velocity *= 0.98f;
            AITimer++;
            if (AITimer >= 30f)
            {
                NPC.netUpdate = true;
                AITimer = 0f;
                CurrentAIState = AIState.Flying;
            }
        }

        public void FlyAround()
        {
            NPC.rotation += 0.25f * NPC.direction;

        }

        public override void FindFrame(int frameHeight)
        {
            if (CurrentAIState == AIState.Walking)
            {
                NPC.rotation = 0f;
                if (NPC.velocity.Y == 0f)
                {
                    NPC.spriteDirection = NPC.direction;
                }
                else if (NPC.velocity.Y < 0f)
                {
                    NPC.frameCounter = 0.0;
                }
                NPC.frameCounter += Math.Abs(NPC.velocity.X) * 1.1f;

                switch (NPC.frameCounter)
                {
                    case < 6.0:
                        NPC.frame.Y = 0;
                        break;
                    case < 12.0:
                        NPC.frame.Y = frameHeight;
                        break;
                    case < 18.0:
                        NPC.frame.Y = frameHeight * 2;
                        break;
                    case < 24.0:
                        NPC.frame.Y = frameHeight * 3;
                        break;
                    case < 32.0:
                        NPC.frame.Y = frameHeight * 4;
                        break;
                    default:
                        NPC.frameCounter = 0.0;
                        break;
                }
            }
            else if (CurrentAIState == AIState.PreparingToFly)
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y = AITimer switch
                {
                    < 10f => frameHeight * 5,
                    < 20f => frameHeight * 6,
                    _ => frameHeight * 7,
                };
            }
            else
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y = frameHeight * 7;
            }
        }
    }
}
