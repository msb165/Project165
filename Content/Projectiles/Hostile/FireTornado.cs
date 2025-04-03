using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using log4net.Appender;
using Project165.Content.NPCs.Bosses.FireBoss;

namespace Project165.Content.Projectiles.Hostile
{
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

        float expandY = 16f;
        float expandX = 16f;

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

            if (Player.Distance(Projectile.Center) < 3000f)
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
            float speed;
            float squareVel = newVel.Length();
            float amount = MathF.Min(1f, Projectile.velocity.Length() / 5f);
            maxSpeed = MathHelper.Lerp(maxSpeed, 22f, amount);

            if (squareVel > maxSpeed)
            {
                speed = maxSpeed / squareVel;
            }
            else
            {
                speed = 1f;
            }

            newVel *= speed;
            Player.velocity = newVel;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float num308 = 0f;
            float num309 = 4.78f;
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 topPos = Projectile.Top;
            Vector2 bottomPos = Projectile.Bottom;
            Vector2 verticalCenter = new(0f, bottomPos.Y - topPos.Y);
            verticalCenter.X = verticalCenter.Y * 0.2f;
            Rectangle sourceRectangle = texture.Frame();
            Vector2 origin = sourceRectangle.Size() / 2f;
            float timer = -MathHelper.PiOver2 / 20f * (float)Main.timeForVisualEffects;

            Color value82 = Color.OrangeRed with { A = 40 };
            Color color78 = Color.Orange with { A = 40 };
            for (float i = (int)bottomPos.Y; i > (int)topPos.Y; i -= num309)
            {
                num308 += num309;
                float num312 = num308 / verticalCenter.Y;
                float num313 = num308 * MathHelper.TwoPi / -20f;
                float scaleOffset = num312 - 0.35f;
                Vector2 drawPos = Vector2.Zero + new Vector2(bottomPos.X, i) - Main.screenPosition;
                Color drawColorBack = Color.Lerp(Color.Transparent, value82, num312 * 2f);
                if (num312 > 0.5f)
                {
                    drawColorBack = Color.Lerp(Color.Transparent, value82, 2f - num312 * 2f);
                }
                Color drawColor = Color.Lerp(Color.Transparent, color78, num312 * 2f);
                if (num312 > 0.5f)
                {
                    drawColor = Color.Lerp(Color.Transparent, color78, 2f - num312 * 2f);
                }
                Main.EntitySpriteDraw(texture, drawPos, sourceRectangle, drawColor, timer + num313, origin, (1f + scaleOffset) * 0.8f, SpriteEffects.None);
                Main.EntitySpriteDraw(texture, drawPos, sourceRectangle, drawColorBack, timer + num313, origin, 1f + scaleOffset, SpriteEffects.None);
            }
            return false;
        }
    }
}
