using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.NPCs.Enemies
{
    internal class TestDummy : ModNPC
    {
        public override string Texture => $"Terraria/Images/NPC_{NPCID.TargetDummy}";
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 11;
        }

        public override void SetDefaults() 
        {
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 1000;
            NPC.HitSound = SoundID.NPCHit15;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.knockBackResist = 0f;
            NPC.value = 0f;
            NPC.netAlways = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.dontCountMe = true;
        }

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;
            if ((NPC.life += 10) > NPC.lifeMax)
            {
                NPC.life = NPC.lifeMax;
            }
        }

        public override bool CheckDead()
        {
            NPC.life = NPC.lifeMax;
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / Main.npcFrameCount[Type] / 2);
            Color npcDrawColor = Color.White * NPC.Opacity;

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, npcDrawColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
            return false;
        }
    }
}
