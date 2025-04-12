using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Project165.Content.NPCs.Bosses.FireBoss;

namespace Project165.Content.Projectiles.Hostile;

public class FireTornado : ModProjectile
{
    public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.SandnadoHostile}";
    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 500;
        Projectile.aiStyle = -1;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 2;
        Projectile.alpha = 255;
    }

    int Direction => (int)Projectile.ai[1];

    Player Player => Main.player[Projectile.owner];

    public override void AI()
    {
        Projectile.timeLeft = 2;
        Projectile.velocity = Vector2.Zero;

        int ragingFlame = NPC.FindFirstNPC(ModContent.NPCType<FireBoss>());
        if (ragingFlame == -1)
        {
            Projectile.Kill();
        }

        if (Projectile.alpha > 0)
        {
            Projectile.alpha--;
        }

        if (Player.Distance(Projectile.Center) < 4000f)
        {
            HandlePushback();
        }

        for (int i = 0; i < 5; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.Center + new Vector2(200f * i * Direction, -500f), 0, 0, DustID.InfernoFork, 32f, 0f, Scale: 2f);
            dust.velocity *= 0.3f;
            dust.noGravity = true;

            Dust dust2 = Dust.NewDustDirect(Projectile.Center + new Vector2(200f * i * Direction, 400f), 0, 0, DustID.InfernoFork, 32f, 0f, Scale: 2f);
            dust2.velocity *= 0.3f;
            dust2.noGravity = true;
        }
    }

    public void HandlePushback()
    {
        Vector2 projPos = new(Projectile.Center.X + Projectile.width * 8f * Direction, Projectile.Center.Y);
        if (Player.Center.X > Projectile.position.X && Direction < 0)
        {
            MoveBack(projPos);
            Player.AddBuff(BuffID.OnFire, 100);
        }

        if (Player.Center.X < Projectile.Center.X && Direction > 0)
        {
            MoveBack(projPos);
            Player.AddBuff(BuffID.OnFire, 100);
        }

        if (Player.Center.Y < Projectile.Center.Y - 500f)
        {
            projPos.X = Player.position.X;
            projPos.Y += 600f;
            MoveBack(projPos);
            Player.AddBuff(BuffID.OnFire, 100);
        }

        if (Player.Center.Y > Projectile.Center.Y + 400f)
        {
            projPos.X = Player.position.X;
            projPos.Y -= 400f;
            MoveBack(projPos);
            Player.AddBuff(BuffID.OnFire, 100);
        }
    }

    public void MoveBack(Vector2 projPos = default, float maxSpeed = 11f)
    {
        Vector2 center = Player.Center;
        Vector2 newVel = projPos - center;
        float squareVel = newVel.Length();
        float amount = MathF.Min(1f, Projectile.velocity.Length() / 5f);
        maxSpeed = MathHelper.Lerp(maxSpeed, 22f, amount);
        float speed = squareVel > maxSpeed ? maxSpeed / squareVel : 1f;

        newVel *= speed;
        Player.velocity = newVel;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        float j = 0f;
        Texture2D texture = TextureAssets.Projectile[Type].Value;

        Vector2 topPos = Projectile.Top;
        Vector2 bottomPos = Projectile.Bottom;
        Vector2 verticalCenter = new(0f, bottomPos.Y - topPos.Y);
        verticalCenter.X = verticalCenter.Y * 0.2f;
        Rectangle sourceRectangle = texture.Frame();
        Vector2 origin = sourceRectangle.Size() / 2f;
        float timer = -MathHelper.PiOver2 / 20f * (float)Main.timeForVisualEffects;

        Color baseColorBack = Color.OrangeRed with { A = 40 };
        Color baseColor = Color.Orange with { A = 40 };
        for (float i = (int)bottomPos.Y; i > (int)topPos.Y; i -= 4.78f)
        {
            j += 4.78f;
            float lerpColorAmount = j / verticalCenter.Y;
            float rotationOffset = j * MathHelper.TwoPi / -20f;
            float scaleOffset = lerpColorAmount - 0.35f;
            Vector2 drawPos = Vector2.Zero + new Vector2(bottomPos.X, i) - Main.screenPosition;
            Color drawColorBack = Color.Lerp(Color.Transparent, baseColorBack, lerpColorAmount * 2f);
            if (lerpColorAmount > 0.5f)
            {
                drawColorBack = Color.Lerp(Color.Transparent, baseColorBack, 2f - lerpColorAmount * 2f);
            }
            Color drawColor = Color.Lerp(Color.Transparent, baseColor, lerpColorAmount * 2f);
            if (lerpColorAmount > 0.5f)
            {
                drawColor = Color.Lerp(Color.Transparent, baseColor, 2f - lerpColorAmount * 2f);
            }
            Main.EntitySpriteDraw(texture, drawPos, sourceRectangle, drawColor, timer + rotationOffset, origin, (1f + scaleOffset) * 0.8f, SpriteEffects.None);
            Main.EntitySpriteDraw(texture, drawPos, sourceRectangle, drawColorBack, timer + rotationOffset, origin, 1f + scaleOffset, SpriteEffects.None);
        }
        return false;
    }
}
