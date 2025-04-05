using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Project165.Content.Projectiles.Hostile;

namespace Project165.Content.NPCs.Bosses.ShadowHand
{
    public class ShadowMinion : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 30;
            NPC.aiStyle = -1;
            NPC.lifeMax = 60;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.alpha = 55;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.scale = 1.25f;
        }

        public float AITimer
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        public float MovementTimer
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        Player Player => Main.player[NPC.target];

        public override void AI()
        {
            if (NPC.FindFirstNPC(ModContent.NPCType<ShadowHand>()) < 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.StrikeInstantKill();
                }
                return;
            }

            NPC.TargetClosest(faceTarget: true);
            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.1f;

            AITimer++;
            MovementTimer += 0.01f;

            NPC.Center = Vector2.Lerp(NPC.Center, Player.Center + new Vector2(MathF.Sin(MovementTimer) * 500f, -250f), 0.01f);

            if (Main.netMode != NetmodeID.Server && NPC.ai[0] >= 100f)
            {
                NPC.ai[0] = 0f;
                Vector2 newProjVelocity = Vector2.Normalize(Player.Center - NPC.Center) * 8f;
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, newProjVelocity, ModContent.ProjectileType<ShadowMinionProj>(), NPC.GetAttackDamage_ForProjectiles(30f, 33f), 2f, Main.myPlayer);
                NPC.netUpdate = true;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.125;
            if (NPC.frameCounter >= Main.npcFrameCount[Type])
            {
                NPC.frameCounter = 0;
            }
            NPC.frame.Y = (int)NPC.frameCounter * frameHeight;
        }
    }
}
