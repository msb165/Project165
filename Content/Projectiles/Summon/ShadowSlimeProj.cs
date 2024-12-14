using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Utilites;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Summon
{
    public class ShadowSlimeProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            Main.projFrames[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 26;
            Projectile.height = 20;
            Projectile.scale = 1.25f;
            Projectile.aiStyle = ProjAIStyleID.Pet;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.minion = true;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 100;
            Projectile.minionSlots = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 23;
            Projectile.timeLeft = 18000;
            AIType = ProjectileID.BabySlime;
        }

        Player Player => Main.player[Projectile.owner];

        public override void PostAI()
        {
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, Alpha:220, Scale:2f);
            dust.velocity *= 0.5f;
            dust.noGravity = true;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = Player.position.Y + Player.height - 12f > Projectile.position.Y + height;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D extraThingTexture = (Texture2D)ModContent.Request<Texture2D>(Project165Utils.ImagesPath + "RoquefortiExtra");
            Color drawColor = Color.White * Projectile.Opacity;
            Color drawColorThing = Color.Red with { A = 0, B = 255 };
            Color drawColorTrail = drawColor with { R = 200, B = 255 };
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            int frameHeight = texture.Height / Main.projFrames[Type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                drawColorTrail *= 0.75f;
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, sourceRectangle, drawColorTrail, Projectile.rotation, sourceRectangle.Size() / 2, Projectile.scale, spriteEffects, 0);
                Main.EntitySpriteDraw(texture, Projectile.oldPos[8] + Projectile.Size / 2 - Main.screenPosition, sourceRectangle, Color.Black with { A = 3 }, Projectile.rotation, sourceRectangle.Size() / 2, Projectile.scale, spriteEffects, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, drawColor, Projectile.rotation, sourceRectangle.Size() / 2, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(extraThingTexture, Projectile.Center - Main.screenPosition, null, drawColorThing, Projectile.rotation + (float)Main.timeForVisualEffects / 16f, extraThingTexture.Size() / 2, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
