using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Project165.Content.NPCs.Enemies;

public class FlyingIce : ModNPC
{
    public override void SetStaticDefaults()
    {
        NPCID.Sets.TrailingMode[Type] = 3;
    }
    public override void SetDefaults()
    {
        NPC.width = 26;
        NPC.height = 28;
        NPC.aiStyle = NPCAIStyleID.DungeonSpirit;
        NPC.noGravity = true;
        NPC.lifeMax = 100;
        NPC.defense = 8;
        NPC.damage = 15;
        NPC.HitSound = SoundID.NPCHit5;
        NPC.DeathSound = SoundID.NPCDeath7;
        NPC.friendly = false;
        NPC.buffImmune[BuffID.Frostburn] = true;
        NPC.buffImmune[BuffID.Frostburn2] = true;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(
        [
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow
        ]);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0)
        {
            for (int i = 0; i < 30; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.IceTorch, NPC.velocity.X * hit.HitDirection * 0.2f, NPC.velocity.Y * 0.2f, 100, default, 1.25f);
            }
        }
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.ZoneSnow ? SpawnCondition.OverworldDay.Chance * 0.25f : 0f;

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Color outlineColor = Color.White with { A = 0 } * NPC.Opacity;
        Color trailColor = Color.Cyan with { A = 127 };

        spriteBatch.Draw(texture, NPC.Center - screenPos, texture.Frame(), outlineColor, NPC.rotation, texture.Size() / 2, NPC.scale * 1.125f, SpriteEffects.None, 0);
        for (int i = 0; i < NPC.oldPos.Length; i++)
        {
            trailColor *= 0.75f;
            spriteBatch.Draw(texture, NPC.oldPos[i] + NPC.Size / 2 - screenPos, texture.Frame(), trailColor, NPC.rotation, texture.Size() / 2, NPC.scale * 1.125f, SpriteEffects.None, 0);
        }
        spriteBatch.Draw(texture, NPC.Center - screenPos, texture.Frame(), drawColor, NPC.rotation, texture.Size() / 2, NPC.scale, SpriteEffects.None, 0);
        return false;
    }
}
