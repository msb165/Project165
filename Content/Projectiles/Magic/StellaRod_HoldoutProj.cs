using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Items.Weapons.Magic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Magic
{
    public class StellaRodHoldoutProj : ModProjectile
    {
        public override string Texture => ModContent.GetInstance<StellarRod>().Texture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
            Projectile.Size = new(8);
            Projectile.friendly = true;
            Projectile.timeLeft = 300;
        }

        public Player Player => Main.player[Projectile.owner];
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {
            if (!Player.channel)
            {
                Projectile.Kill();
            }
            Projectile.rotation += 0.4f * Player.direction;
            SetPlayerValues();
        }

        public void SetPlayerValues()
        {
            Projectile.Center = Player.RotatedRelativePoint(Player.Center);
            Player.heldProj = Projectile.whoAmI;
            Player.itemRotation = Projectile.rotation;
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.Pi);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height);
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Color drawColor = Projectile.GetAlpha(lightColor);
            float rotation = Projectile.rotation;

            Main.EntitySpriteDraw(texture, drawPos, texture.Frame(), drawColor, rotation, drawOrigin, Projectile.scale, spriteEffects);
            return false;
        }
    }
}
