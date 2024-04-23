using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project165.Content.Dusts
{
    public class CloudDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 36, 36);
            dust.alpha = 128;
            dust.noLightEmittence = true;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += 0.15f;
            dust.alpha += 4;

            if (dust.alpha >= 255)
            {
                dust.alpha = 255;
                dust.active = false;
            }
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Color drawColor = Lighting.GetColor((dust.position + Vector2.One * 4).ToTileCoordinates());
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, dust.GetAlpha(drawColor), dust.rotation, new Vector2(18f), dust.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
