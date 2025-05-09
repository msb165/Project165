﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Melee;

public class ShadowSlash : ModProjectile
{
    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.SharpTears}";
    public override void SetStaticDefaults()
    {

    }
    public override void SetDefaults()
    {
        Projectile.friendly = true;
        Projectile.hide = true;
        Projectile.netImportant = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.aiStyle = -1;
        Projectile.Size = new(14);
        Projectile.timeLeft = 180;
        Projectile.extraUpdates = 180;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 25;
    }

    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

        if (Main.rand.NextBool(25))
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height * 4, DustID.Clentaminator_Purple, Alpha:100, Scale:0.75f);
            dust.velocity *= 0.5f;
        }

        if (Projectile.frameCounter++ >= 1)
        {
            Projectile.frameCounter = 0;
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
            {
                PositionInWorld = Projectile.Center,
                MovementVector = Projectile.velocity,
                UniqueInfoPiece = 178
            });
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.ShadowFlame, 300);
        target.AddBuff(BuffID.Venom, 300);
        target.AddBuff(BuffID.Bleeding, 300);
        target.AddBuff(BuffID.Weak, 300);
    }
}
