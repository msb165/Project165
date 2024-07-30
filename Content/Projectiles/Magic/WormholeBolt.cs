using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Magic
{
    public class WormholeBolt : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.NebulaBolt}";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.Size = new(16);
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.alpha = 0;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 23;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.25f, 1f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity.Y += 0.2f;

            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }

            Dust boltDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 100, Color.Magenta, 0.5f);
            boltDust.scale *= 1.1f;
            boltDust.velocity *= 0f;
            boltDust.velocity += Projectile.velocity * 0.1f;
            boltDust.noGravity = true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit3, Projectile.position);
            for (int i = 0; i < 30; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), 0, 0, 0, Color.Magenta, 1.25f);
                dust.velocity *= 4f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D extraTexture = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            //Color drawColorTrail = Color.Magenta with { A = 0 };
            float j = 1;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {                
                j -= i * 0.005f;
                Color drawColorTrail = Color.Lerp(Color.Magenta with { A = 127 }, new Color(101, 0, 248, 200) * 0.5f, 0.1f * i) * j;
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, texture.Frame(), drawColorTrail, Projectile.rotation, texture.Size() / 2, Projectile.scale - i / (float)Projectile.oldPos.Length, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, texture.Frame(), drawColorTrail * 0.05f, Projectile.rotation, texture.Size() / 2, (Projectile.scale * 2f) - i / (float)Projectile.oldPos.Length, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(extraTexture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, extraTexture.Frame(), drawColorTrail * 0.4f, Projectile.rotation, extraTexture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.White with { A = 0 }, Projectile.rotation, texture.Size() / 2, Projectile.scale * 1.125f, SpriteEffects.None, 0);
            return false;
        }
    }
}
