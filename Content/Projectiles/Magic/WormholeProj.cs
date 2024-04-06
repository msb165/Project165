using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Dusts;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Magic
{
    public class WormholeProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.Size = new(30);
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 800;
            Projectile.penetrate = -1;
        }

        public float AITimer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public float AICounter
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override void AI()
        {
            AITimer++;
            if (AITimer > 24f && AICounter == 0f)
            {
                AITimer = 0f;
                AICounter = 1f;
                Projectile.netUpdate = true;
                Projectile.velocity *= 0f;
            }
            Projectile.rotation += 0.125f * Projectile.direction;

            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), Projectile.velocity.X, Projectile.velocity.Y, 0, Color.LightPink, 0.5f);

            if (Main.myPlayer == Projectile.owner && AITimer % 20f == 0f && AICounter == 1f)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-10, -2)), ModContent.ProjectileType<WormholeBolt>(), Projectile.damage, 0f, Projectile.owner);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit3, Projectile.position);
            for (int i = 0; i < 30; i++)
            {
                Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), Projectile.velocity.X * 2f, Projectile.velocity.Y * 2f, 0, Color.BlueViolet, 1.25f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D texture2 = (Texture2D)ModContent.Request<Texture2D>($"Terraria/Images/Projectile_{ProjectileID.StardustTowerMark}");

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.Black * 0.25f, Projectile.rotation, texture.Size() / 2, Projectile.scale * 2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.Blue with { A = 0 } * 0.2f, -Projectile.rotation * 0.5f, texture.Size() / 2, Projectile.scale * 2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.Purple with { A = 0 }, -Projectile.rotation * 0.75f, texture.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.Purple with { A = 0 }, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.White with { A = 0 } * 0.75f, Projectile.rotation, texture.Size() / 2, Projectile.scale * 0.5f, SpriteEffects.None, 0);
            // Glow around the thing
            Main.spriteBatch.Draw(texture2, Projectile.Center - Main.screenPosition, texture2.Frame(), new Color(100, 100, 255, 0) * 0.35f, Projectile.rotation, texture2.Size() / 2, Projectile.scale * 2f, SpriteEffects.None, 0);            return false;
        }
    }
}
