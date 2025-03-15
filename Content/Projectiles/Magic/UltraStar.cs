using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;


namespace Project165.Content.Projectiles.Magic
{
    public class UltraStar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;

        }

        public override void SetDefaults()
        {
            Projectile.Size = new(20);
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public float AITimer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {
            AITimer++;
            Projectile.rotation += 0.25f;
            for (int i = 0; i < 3; i++)
            {
                Dust starless = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Yellow, Scale:0.5f);
                starless.position = Projectile.Center - Projectile.velocity / 10f * i;
                starless.noGravity = true;
                starless.velocity *= 1.1f;
            }
            if (AITimer >= 20f && Projectile.scale < 2f)
            {
                Projectile.velocity *= 0.95f;
                Projectile.scale += 0.025f;
                Projectile.rotation += 0.25f;
            }

            if (Projectile.alpha < 127)
            {
                Projectile.alpha++;
            }

            if (Projectile.scale >= 2f)
            {
                Projectile.Kill();
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.4f, PitchVariance = 0.3f }, Projectile.position);
            if (Main.myPlayer == Projectile.owner)
            {
                int projAmount = Main.rand.Next(2, 5);
                for (int i = 0; i < projAmount; i++)
                {
                    Vector2 spawnPos = Projectile.Center + Utils.RandomVector2(Main.rand, -30f, 30f);
                    Vector2 newVelocity = Vector2.Normalize(Utils.RandomVector2(Main.rand, -100, 101));
                    while (newVelocity == Vector2.Zero)
                    {
                        newVelocity = Vector2.Normalize(Utils.RandomVector2(Main.rand, -100, 101));
                    }
                    if (newVelocity.Y > 0.2f)
                    {
                        newVelocity.Y *= -1f;
                    }
                    newVelocity *= Main.rand.Next(7, 10);
                    Projectile.NewProjectile(Projectile.GetSource_Death(), spawnPos, newVelocity, ModContent.ProjectileType<YellowBeam>(), (int)(Projectile.damage * 0.65), Projectile.knockBack * 0.8f, Projectile.owner);
                }
            }
            for (int i = 0; i < 30; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SandSpray, newColor: Color.White with { A = 0 }, Scale: 1.25f);
                dust.velocity *= 6f;
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2;
            Color drawColor = Projectile.GetAlpha(lightColor);
            Color trailColor = Projectile.GetAlpha(lightColor) with { A = 0 };
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Projectile.rotation;
            float scale = Projectile.scale;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                scale *= 1.0625f;
                Vector2 trailPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                trailColor *= 0.75f;
                Main.EntitySpriteDraw(texture, trailPos, null, trailColor, Projectile.rotation, drawOrigin, scale, spriteEffects);
            }

            Main.EntitySpriteDraw(texture, drawPos, texture.Frame(), drawColor, rotation, drawOrigin, Projectile.scale, spriteEffects);
            Main.EntitySpriteDraw(texture, drawPos, texture.Frame(), drawColor with { A = 0 }, rotation, drawOrigin, Projectile.scale * 1.25f, spriteEffects);
            return false;
        }
    }
}
