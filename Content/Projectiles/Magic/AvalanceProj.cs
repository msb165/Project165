using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Dusts;
using Project165.Content.Items.Weapons.Magic;
using ReLogic.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Magic
{
    public class AvalanceProj : ModProjectile
    {
        public override string Texture => ModContent.GetInstance<Avalanche>().Texture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.Size = new(32);
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;            
            Projectile.timeLeft = 800;
            Projectile.tileCollide = true;
            Projectile.scale = 1.5f;
        }

        public bool ShouldSpawnDust => Projectile.ai[0] == 0f;

        public override void AI()
        {   
            if (ShouldSpawnDust)
            {
                Projectile.ai[0] = 1f;
                for (int i = 0; i < 20; i++)
                {
                    Vector2 newPosition = new Vector2(10f, 0f).RotatedBy(i + MathHelper.TwoPi / 20);
                    Dust newDust = Dust.NewDustPerfect(Projectile.Center - newPosition, DustID.SnowSpray, newPosition, 180, default, 1.25f);
                    newDust.noGravity = true;
                }
            }

            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Snow, 0, 0, 190);
                dust.noGravity = true;
            }

            Projectile.rotation += Projectile.velocity.X * 0.06f;
            Projectile.velocity.Y += 0.3f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.Kill();
            }
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath15 with { Pitch = -0.5f }, Projectile.position);
            for (int i = 0; i < 30; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Snow, 0, 0, 100, default, 1.5f);
                dust.noGravity = true;
            }
            
            for (int i = 0; i < 10; i++)
            {
                Vector2 newVelocity = Vector2.UnitX * 12f;
                newVelocity = newVelocity.RotatedBy(-i * MathHelper.TwoPi / 10f, Vector2.Zero);
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.position, newVelocity, ProjectileID.SnowBallFriendly, Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
            }
        }

        float spawnScale = 0.5f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color drawColor = Projectile.GetAlpha(lightColor);
            Color drawColorTrail = drawColor with { A = 0 };

            if (spawnScale < 1.5f)
            {
                spawnScale += 0.1f;
            }

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (i % 2 != 0)
                {
                    continue;
                }

                drawColorTrail *= 0.75f;
                spriteBatch.Draw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, texture.Frame(), drawColorTrail, Projectile.rotation, texture.Size() / 2, spawnScale, SpriteEffects.None, 0);
            }

            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), drawColor, Projectile.rotation, texture.Size() / 2, spawnScale, SpriteEffects.None, 0);
            return false;
        }
    }
}
