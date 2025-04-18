﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Melee;

public class MushroomProj : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.Size = new(20);
        Projectile.DamageType = DamageClass.MeleeNoSpeed;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 10;
        Projectile.penetrate = -1;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.noEnchantmentVisuals = true;
    }

    public override void AI()
    {
        Projectile.velocity *= 0.95f;
        Projectile.rotation += 0.25f * Projectile.direction;

        Projectile.ai[1] += 1f;

        if (Projectile.ai[1] > 30f)
        {                
            Projectile.alpha += 10;
            if (Projectile.alpha >= 255)
            {
                Projectile.alpha = 255;
                Projectile.Kill();
            }
        }
    }
    public override Color? GetAlpha(Color lightColor) => Color.White with { A = 0 } * Projectile.Opacity;
}
