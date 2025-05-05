using Microsoft.Xna.Framework;
using Project165.Content.Projectiles.Hostile;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Project165.Content.NPCs.Enemies
{
    public class Snowbomber : ModNPC
    {
        // None3 = Snowman Bomber
        public override string Texture => $"Terraria/Images/NPC_{NPCID.None3}";
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 40;
            NPC.aiStyle = -1;
            NPC.damage = 35;
            NPC.defense = 4;
            NPC.lifeMax = 150;
            NPC.HitSound = SoundID.NPCHit11;
            NPC.DeathSound = SoundID.NPCDeath15;
            NPC.knockBackResist = 0.6f;
            NPC.value = 400f;
            NPC.coldDamage = true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1 * MathF.Sign(NPC.velocity.Y);
            NPC.frameCounter %= 16;
            NPC.frame.Y = frameHeight * (int)(NPC.frameCounter / 6.0);
            if (NPC.frameCounter < 0.0)
            {
                NPC.frameCounter = 0.0;
            }
        }

        public bool ShouldExplode
        {
            get => NPC.ai[1] == 1f;
            set => NPC.ai[1] = value ? 1f : 0f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => Main.invasionType == InvasionID.SnowLegion ? SpawnCondition.Invasion.Chance * 0.2f : 0f;

        Player Player => Main.player[NPC.target];
        public override void AI()
        {
            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.05f);

            NPC.position += NPC.netOffset;
            Dust dust = Dust.NewDustPerfect(NPC.Center + new Vector2(-NPC.direction * 4f, -NPC.height + 1f), DustID.OrangeTorch, -Vector2.UnitY * 4f);
            dust.noGravity = true;
            NPC.position -= NPC.netOffset;
            NPC.velocity.X += 0.01f * NPC.direction;


            if (NPC.velocity.Y == 0f)
            {
                NPC.TargetClosest(faceTarget: true);
                NPC.rotation = MathHelper.Lerp(NPC.rotation, 0f, 1f);
                NPC.spriteDirection = -NPC.direction;
                NPC.velocity.X = 4f * NPC.direction;
                NPC.velocity.Y -= 6f;
            }

            if (Vector2.Distance(NPC.Center, Player.Center) < 64f)
            {
                ShouldExplode = true;
                NPC.netUpdate = true;
                NPC.HitEffect();
                NPC.life = -1;
                NPC.active = false;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (ShouldExplode)
            {
                Projectile.NewProjectile(NPC.GetSource_OnHit(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<SnowbomberExplosion>(), NPC.damage, 4f, Main.myPlayer);
            }
        }
    }
}
