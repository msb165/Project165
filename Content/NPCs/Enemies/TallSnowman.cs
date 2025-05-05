using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using Project165.Content.Items.Mounts;
using Project165.Content.Projectiles.Hostile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Project165.Content.NPCs.Enemies
{
    public class TallSnowman : ModNPC
    {
        public enum AIState : int
        {
            JumpingAround = 0,
            ShootingAround = 1
        }

        public float AITimer
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        public AIState CurrentAIState
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[Type] = 5;
            NPCID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.width = 58;
            NPC.height = 98;
            NPC.friendly = false;
            NPC.damage = 27;
            NPC.lifeMax = 120;
            NPC.aiStyle = -1;
            NPC.defense = 8;
            NPC.HitSound = SoundID.NPCHit11;
            NPC.DeathSound = SoundID.NPCDeath15;
            NPC.knockBackResist = 0.6f;
            NPC.value = 400f;
            NPC.coldDamage = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(
            [
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Invasions.FrostLegion,
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow
            ]);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => Main.invasionType == InvasionID.SnowLegion ? SpawnCondition.Invasion.Chance * 0.1f : 0f;

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;
            if (CurrentAIState == AIState.JumpingAround)
            {
                JumpAround();
            }
            else
            {
                ShootAround();
            }
        }

        int maxTime = 240;
        Player Player => Main.player[NPC.target];

        public void JumpAround()
        {
            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.05f);
            AITimer++;
            if (NPC.velocity.Y == 0f)
            {
                NPC.TargetClosest(faceTarget: true);
                NPC.velocity.X = NPC.direction * 4f;
                NPC.velocity.Y = -8f;
                NPC.rotation = MathHelper.Lerp(NPC.rotation, 0f, 1f);

                if (AITimer >= maxTime)
                {
                    AITimer = 0f;
                    CurrentAIState = AIState.ShootingAround;
                    NPC.netUpdate = true;
                }
            }
            NPC.velocity.X += 0.01f * NPC.direction;
        }

        int shootTime = 15;
        public void ShootAround()
        {
            NPC.TargetClosest(faceTarget: true);
            NPC.velocity.X *= 0.9f;
            AITimer++;
            if (AITimer % shootTime == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 newVelocity = NPC.Center.DirectionTo(Player.Center) * 6f;
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Top + Vector2.UnitY * 8f, newVelocity.RotatedByRandom(MathHelper.ToRadians(25f)), ModContent.ProjectileType<SnowBallHostile>(), 20, 6f, Main.myPlayer);
            }
            if (AITimer >= maxTime)
            {
                AITimer = 0f;
                CurrentAIState = AIState.JumpingAround;
                NPC.netUpdate = true;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SnowMount>(), 20));

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawOrigin = texture.Size() / 2;
            Color trailColor = drawColor;
            SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                trailColor *= 0.75f;
                Vector2 drawPos = NPC.oldPos[i] + NPC.Size / 2;
                Main.EntitySpriteDraw(texture, drawPos - NPC.velocity / 10f * i - screenPos, NPC.frame, trailColor, NPC.oldRot[i], drawOrigin, NPC.scale, spriteEffects);
            }

            Main.EntitySpriteDraw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects);
            return false;
        }
    }
}
