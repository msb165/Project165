﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Common.Systems;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.NPCs.Bosses.FireBoss
{
    [AutoloadBossHead]
    public class FireBoss : ModNPC
    {
        public enum AIState : int
        {
            Spawning = 0,
            Flying = 1,
            Unused = 2,
            Unused2 = 3,
            Death = 4
        }

        public AIState CurrentState
        {
            get => (AIState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.TrailCacheLength[Type] = 20;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
            Main.npcFrameCount[Type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.lifeMax = 42000;
            NPC.defense = 30;
            NPC.width = 68;
            NPC.height = 96;
            NPC.scale = 2.5f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.Item105 with { Pitch = 1f };
            NPC.DeathSound = SoundID.Item105;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(
            [
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld
            ]);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => true;

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }

        Player Player => Main.player[NPC.target];

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, 2f, 2f, 2f);
            base.AI();
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedFireBoss, -1);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.25;
            if (NPC.frameCounter >= Main.npcFrameCount[Type])
            {
                NPC.frameCounter = 0;
            }
            NPC.frame.Y = (int)NPC.frameCounter * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / Main.npcFrameCount[Type] / 2);
            Color npcDrawColorTrail = new(255 - NPC.alpha, 255 - NPC.alpha, 255 - NPC.alpha, 0);
            Color npcDrawColor = Color.White * NPC.Opacity;

            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 drawPos = NPC.oldPos[i] + NPC.Size * 0.5f - screenPos;
                npcDrawColorTrail *= 0.75f;
                spriteBatch.Draw(texture, drawPos, NPC.frame, npcDrawColorTrail, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            }

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, npcDrawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
