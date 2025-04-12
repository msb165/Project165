using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Items.Weapons.Magic;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Magic;

public class BlackHoleProj : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.Size = new(32);
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.scale = 2f;
        Projectile.penetrate = 4;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 12;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }

    Player Player => Main.player[Projectile.owner];
    public override void AI()
    {
        for (int i = 0; i < 8; i++)
        {
            Vector2 spinPoint = Main.rand.NextVector2CircularEdge(60, 60);
            Dust dust2 = Dust.NewDustPerfect(Projectile.position, DustID.Corruption, Vector2.Zero, 150);
            dust2.noGravity = true;
            dust2.scale = 1.25f;
            dust2.position = Projectile.Center - spinPoint;
            dust2.velocity = spinPoint.SafeNormalize(Vector2.Zero);
        }

        if (Main.myPlayer == Projectile.owner)
        {
            if (Player.channel && Projectile.ai[0] == 0f)
            {
                Projectile.tileCollide = false;
                Projectile.velocity = Vector2.Zero;
                Projectile.Center = Vector2.Lerp(Projectile.Center, Main.MouseWorld, 0.4f);
            }
            else
            {
                Projectile.tileCollide = true;
                Projectile.netUpdate = true;
                Projectile.ai[0] = 1f;
                if (Projectile.velocity.Length() < 2f)
                {
                    Projectile.velocity = Projectile.DirectionFrom(Player.Center) * 24f;
                }
            }
        }

        bool foundTarget = false;
        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC target = Main.npc[i];
            if (target != null && !foundTarget && !target.boss && target.CanBeChasedBy(this) && Projectile.Distance(target.Center) < 128f)
            {
                Vector2 newVelocity = Vector2.Normalize(Projectile.Center - target.Center) * 8f;
                target.velocity = Vector2.Lerp(target.velocity, newVelocity, 0.1f);
                foundTarget = true;
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.ShadowFlame, 300);
        target.AddBuff(BuffID.Venom, 300);
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item40, Projectile.position);

        for (int i = 0; i < 30; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption, Alpha: 100, Scale: 2f);
            dust.noGravity = true;
            dust.velocity *= 4f;
        }

        if (Projectile.owner == Main.myPlayer)
        {
            int projAmount = Main.rand.Next(2, 5);
            for (int i = 0; i < projAmount; i++)
            {
                Vector2 newVelocity = Utils.RandomVector2(Main.rand, -100, 101).SafeNormalize(Vector2.UnitY);
                newVelocity *= Main.rand.NextFloat(0.5f, 4f);
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, newVelocity, ModContent.ProjectileType<ShadowGas>(), Projectile.damage, 1f, Projectile.owner);
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;

        Vector2 origin = texture.Size() / 2;
        Color drawColor = Color.White * Projectile.Opacity;

        float timer = MathF.Cos((float)Main.timeForVisualEffects * MathHelper.Pi / 15f);

        for (int i = 0; i < 8; i++)
        {
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY.RotatedBy(MathHelper.PiOver4 * i) * (4f + 1f * timer), null, drawColor * 0.3f, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
        }

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);

        return false;
    }
}
