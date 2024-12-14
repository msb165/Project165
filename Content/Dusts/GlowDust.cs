using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Project165.Content.Dusts;

public class GlowDust : ModDust
{   
    public override void OnSpawn(Dust dust)
    {
        dust.velocity *= 0.5f;
        dust.noGravity = true;       
        dust.frame = new Rectangle(0, 0, 128, 128);
        dust.noLightEmittence = false;
    }

    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        dust.scale *= 0.9f;
        if (dust.scale < 0.01f)
        {
            dust.active = false;
        }
        return false;
    }

    public override bool PreDraw(Dust dust)
    {
        float alpha = 1f - dust.alpha / 255f;
        Color drawColor = Lighting.GetColor((int)(dust.position.X + 4) / 16, (int)(dust.position.Y + 4) / 16);
        Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, dust.color with { A = 0 } * alpha, dust.rotation, dust.frame.Size() / 2, 0.5f * dust.scale, SpriteEffects.None, 0);
        Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Color.White with { A = 0 } * alpha, dust.rotation, dust.frame.Size() / 2, 0.125f * dust.scale, SpriteEffects.None, 0);
        return false;
    }
}
