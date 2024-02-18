using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Project165.Content.NPCs.Enemies
{
    public class MiniDungeonGuardian : ModNPC
    {
        public override void SetStaticDefaults() 
        {
            NPCID.Sets.ShimmerTransformToNPC[Type] = NPCID.CursedSkull;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Slow] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 46;
            NPC.height = 48;
            NPC.damage = 9;
            NPC.lifeMax = 80;
            NPC.knockBackResist = 0.4f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 0.5f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(0, 0, 0, 8);
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;
        }

        Player Player => Main.player[NPC.target];
        public override void AI()
        {
            NPC.TargetClosest(faceTarget: true);
            if (NPC.ai[0] == 0f)
            {
                NPC.ai[0] = 1f;
                SoundEngine.PlaySound(SoundID.Roar with { Pitch = 0.65f }, NPC.position);
            }

            Vector2 newSpeed = NPC.DirectionTo(Player.Center).SafeNormalize(Vector2.Zero);

            NPC.rotation += NPC.direction * 0.4f;            
            NPC.velocity = NPC.velocity.MoveTowards(newSpeed, 5f) * 3f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.GoldenKey, 100));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => SpawnCondition.Dungeon.Chance * 0.25f;

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust deathDust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Bone, -NPC.velocity.X, -NPC.velocity.Y, 100, default);
                    deathDust.noGravity = true;
                    deathDust.noLight = true;
                    deathDust.velocity *= 1.25f;
                }
            }
        }
    }
}
