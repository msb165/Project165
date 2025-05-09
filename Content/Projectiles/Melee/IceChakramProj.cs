﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Project165.Content.Items.Weapons.Melee;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Melee;

public class IceChakramProj : ModProjectile
{
    public override string Texture => ModContent.GetInstance<IceChakram>().Texture;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.Size = new(28);
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Melee;            
        Projectile.aiStyle = -1;
    }

    private Player Owner => Main.player[Projectile.owner];

    private readonly float maxTime = 20f;
    private readonly float acceleration = 1.2f;

    public override void AI()
    {
        Projectile.rotation += 0.25f * Projectile.direction;

        if (Projectile.ai[0] == 0f)
        {
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] % 8f == 0 && Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.position, (Vector2.UnitY * 8f * -Projectile.direction).RotatedBy(Projectile.velocity.ToRotation()), ProjectileID.NorthPoleSnowflake, Projectile.damage / 4, Projectile.knockBack, Owner.whoAmI, Main.rand.Next(3));
            }

            if (Projectile.ai[1] >= maxTime)
            {
                Projectile.ai[0] = 1f;
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
        }
        else
        {
            if (Vector2.Distance(Owner.Center, Projectile.Center) > 3000f)
            {
                Projectile.Kill();
            }

            Vector2 vecTowardsPlayer = Projectile.DirectionTo(Owner.MountedCenter).SafeNormalize(Vector2.Zero);
            Projectile.velocity = Projectile.velocity.MoveTowards(vecTowardsPlayer * 9.5f, acceleration);

            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.Hitbox.Intersects(Owner.Hitbox))
                {
                    Projectile.Kill();
                }
            }
        }

        GenerateDust();
    }

    private void GenerateDust()
    {
        Vector2 dustPosition = Projectile.Center + Vector2.Normalize(Projectile.velocity) * 16f;
        Vector2 dustVelocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2) * 0.33f + Projectile.velocity / 4f;

        Dust iceDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Cyan);
        iceDust.scale = 1.25f;
        iceDust.frame.Y = 10 * 3;
        iceDust.position = dustPosition + dustVelocity;
        iceDust.noGravity = true;
        iceDust.velocity = dustVelocity;

        Dust iceDust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Cyan);
        iceDust2.scale = 1.25f;
        iceDust2.frame.Y = 10 * 3;
        iceDust2.position = dustPosition - dustVelocity;
        iceDust2.noGravity = true;
        iceDust2.velocity = -dustVelocity;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Frostburn, 300);
        if (Main.rand.NextBool(4))
        {
            target.AddBuff(BuffID.Frostburn2, 300);
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        for (int i = 0; i < 20; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Cyan, Scale: 0.5f);
            dust.noGravity = true;
            dust.velocity *= 4f;
        }

        Projectile.ai[0] = 1f;
        Projectile.tileCollide = false;
        if (Projectile.velocity.X != Projectile.oldVelocity.X)
        {
            Projectile.velocity.X = -oldVelocity.X;
        }
        if (Projectile.velocity.Y != Projectile.oldVelocity.Y)
        {
            Projectile.velocity.Y = -oldVelocity.Y;
        }
        SoundEngine.PlaySound(SoundID.Shatter with { Pitch = 1f, Volume = 0.6f }, Projectile.position);

        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Color drawColor = Color.White;
        Color drawColorTrail = Color.SkyBlue with { A = 0 };

        Vector2 drawOrigin = texture.Frame().Size() / 2f;
        Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

        SpriteBatch spriteBatch = Main.spriteBatch;

        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 drawPosTrail = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            drawColorTrail *= 0.6f;

            spriteBatch.Draw(texture, drawPosTrail, texture.Frame(), drawColorTrail, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }

        spriteBatch.Draw(texture, drawPos, texture.Frame(), Color.Cyan with { A = 0 }, Projectile.rotation, drawOrigin, Projectile.scale * 1.25f, SpriteEffects.None, 0);
        spriteBatch.Draw(texture, drawPos, texture.Frame(), drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        return false;
    }
}
