using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Project165.Content.Items.Weapons.Magic;
using Terraria.Audio;
using Project165.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;

namespace Project165.Content.Projectiles.Magic;

internal class ManaKnifeProj : ModProjectile
{
    public override string Texture => ModContent.GetInstance<ManaKnife>().Texture;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 15;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.Size = new(16);
        Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
        Projectile.ignoreWater = true;
        Projectile.extraUpdates = 1;
        Projectile.penetrate = 1;
        AIType = ProjectileID.ThrowingKnife;
    }

    public Player Owner => Main.player[Projectile.owner];
    
    public override void PostAI()
    {
        Lighting.AddLight(Projectile.Center, 0.1f, 0.1f, 0.5f);
        if (Main.rand.NextBool(2))
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), Alpha:100, newColor:Color.SkyBlue, Scale:0.75f);
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (target.immortal || NPCID.Sets.CountsAsCritter[target.type] || target.SpawnedFromStatue)
        {
            return;
        }

        int manaAmount = Main.rand.Next(4, 11);
        if (hit.Crit)
        {
            manaAmount *= 2;
        }
        Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<ManaHeal>(), 0, 0f, Projectile.owner, Projectile.owner, manaAmount); 
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        for (int i = 0; i < 8; i++)
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), 0, 0, 100, Color.SkyBlue, 0.75f);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        SpriteBatch spriteBatch = Main.spriteBatch;
        Color drawColor = Projectile.GetAlpha(lightColor);
        Color drawColorTrail = new Color(255, 255, 255, 0) * Projectile.Opacity;
        float newScale = Projectile.scale;

        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            drawColorTrail *= 0.75f;
            newScale *= 0.99f;
            spriteBatch.Draw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, texture.Frame(), drawColorTrail, Projectile.rotation, Projectile.Size / 2, newScale, SpriteEffects.None, 0);
        }

        spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);
        return false;
    }
}
