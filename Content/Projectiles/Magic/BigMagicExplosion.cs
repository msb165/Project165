using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Magic;

public class BigMagicExplosion : ModProjectile
{
    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.AmberBolt}";
    public override void SetDefaults()
    {
        Projectile.Size = new(260);
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.timeLeft = 2;
        Projectile.tileCollide = false;
        Projectile.alpha = 255;
        Projectile.penetrate = -1;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire, 300);
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.2f }, Projectile.position);
        for (int i = 0; i < 30; i++)
        {
            Vector2 spinPoint = Utils.RandomVector2(Main.rand, -100, 101);
            Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.InfernoFork, Scale: 1.5f);
            dust.position = Projectile.Center + spinPoint;
            dust.noGravity = true;
            dust.velocity = Vector2.Normalize(spinPoint) * 4f;
        }
    }
}
