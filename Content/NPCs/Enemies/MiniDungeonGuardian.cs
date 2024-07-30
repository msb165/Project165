using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
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
            NPCID.Sets.TrailCacheLength[Type] = 5;
            NPCID.Sets.TrailingMode[Type] = 3;
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

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(
            [
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon
            ]);
        }

        Player Player => Main.player[NPC.target];
        public override void AI()
        {
            NPC.TargetClosest(faceTarget: true);
            if (NPC.localAI[0] == 0f)
            {
                NPC.localAI[0] = 1f;
                SoundEngine.PlaySound(SoundID.Roar with { Pitch = 0.65f }, NPC.position);
            }

            Vector2 newSpeed = NPC.DirectionTo(Player.Center).SafeNormalize(Vector2.Zero);

            NPC.rotation += NPC.direction * 0.4f;            
            NPC.velocity = NPC.velocity.MoveTowards(newSpeed, 5f) * 4f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.GoldenKey, 100));
            npcLoot.Add(ItemDropRule.Common(ItemID.Bone, 1, 1, 3));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => SpawnCondition.Dungeon.Chance * 0.25f;

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust deathDust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Bone, -NPC.velocity.X, -NPC.velocity.Y, 100, default, 1.5f);
                    deathDust.noGravity = true;
                    deathDust.noLight = true;
                    deathDust.velocity *= 1.25f;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Color trailColor = drawColor;

            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                trailColor *= 0.75f;
                spriteBatch.Draw(texture, NPC.oldPos[i] + texture.Size() / 2 - screenPos, texture.Frame(), trailColor, NPC.rotation, texture.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            }

            spriteBatch.Draw(texture, NPC.Center - screenPos, texture.Frame(), drawColor, NPC.rotation, texture.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
