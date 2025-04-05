using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Utilites;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Melee
{
    public class ShadowYoYoProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosMaximumRange[Type] = 400f;
            ProjectileID.Sets.YoyosTopSpeed[Type] = 16.5f;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new(16);
            Projectile.scale = 1.125f;
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.noEnchantmentVisuals = true;
        }

        public override void PostAI()
        {
            if (Projectile.localAI[1] == 0f && Main.myPlayer == Projectile.owner)
            {
                Projectile.localAI[1] = 1f;
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero * Math.Sign(Projectile.velocity.X), ModContent.ProjectileType<ShadowBall>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.whoAmI, i * 24f);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
            target.AddBuff(BuffID.Venom, 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D extraThingTexture = (Texture2D)ModContent.Request<Texture2D>(Project165Utils.ImagesPath + "RoquefortiExtra");

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.White, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(extraThingTexture, Projectile.Center - Main.screenPosition, extraThingTexture.Frame(), Color.Purple with { A = 0 }, Projectile.rotation, extraThingTexture.Size() / 2, 0.75f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.Black with { A = 32 }, Projectile.rotation, texture.Size() / 2, Projectile.scale * 1.25f, SpriteEffects.None, 0);

            return false;
        }
    }
}
