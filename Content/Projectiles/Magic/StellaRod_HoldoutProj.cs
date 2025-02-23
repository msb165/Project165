using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Items.Weapons.Magic;
using Project165.Utilites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
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
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.scale = 1.25f;
            Projectile.timeLeft = 300;
        }

        public Player Player => Main.player[Projectile.owner];
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public bool HasShot
        {
            get => Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = (value ? 1f : 0f);
        }

        Vector2 starShootSpeed = Vector2.Zero;

        public override void AI()
        {
            if (!Player.active || Player.dead || Player.noItems || Player.CCed)
            {
                Projectile.Kill();
                return;
            }
            if (Main.myPlayer == Projectile.owner && Main.mapFullscreen)
            {
                Projectile.Kill();
                return;
            }

            SetPlayerValues();
            Projectile.Center = Player.RotatedRelativePoint(Player.Center) - Vector2.One * Player.direction;
            Timer++;
            if (Timer >= 24f)
            {
                Timer = 0f;
                starShootSpeed = Player.Center - Main.MouseWorld;
                if (Main.myPlayer == Projectile.owner && !HasShot)
                {
                    HasShot = true;
                    SoundEngine.PlaySound(SoundID.Item9, Projectile.Center);
                    ShootStar();
                }
                Projectile.netUpdate = true;
            }

            if (!HasShot)
            {
                Projectile.rotation += 0.6f * Player.direction;

                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.rotation.ToRotationVector2() * 80f - Projectile.velocity / 10f * i, DustID.Clentaminator_Purple, Scale:0.75f);
                    dust.velocity *= 0f;
                    dust.noGravity = true;
                }
            }
        }

        public void ShootStar()
        {
            starShootSpeed = Vector2.Normalize(starShootSpeed);
            Player.ChangeDir(-MathF.Sign(starShootSpeed.X));
            Projectile.rotation = starShootSpeed.SafeNormalize(Vector2.Zero).ToRotation() - MathHelper.PiOver2;
            Projectile.timeLeft = 20;
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center - starShootSpeed * 14f, -starShootSpeed * 14f, ModContent.ProjectileType<UltraStar>(), Player.HeldItem.damage, Player.HeldItem.knockBack, Projectile.owner);
            Player.reuseDelay = 8;
        }

        public void SetPlayerValues()
        {
            Player.SetDummyItemTime(2);
            Player.heldProj = Projectile.whoAmI;
            Player.itemRotation = Projectile.rotation;
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.Pi);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D textureOutline = (Texture2D)ModContent.Request<Texture2D>(Project165Utils.ImagesPath + "StellarRod_Outline");
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height);
            Color drawColor = Projectile.GetAlpha(lightColor);
            Color trailColor = Projectile.GetAlpha(lightColor) with { A = 0 };
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Projectile.rotation;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 trailPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                trailColor *= 0.6f;
                Main.EntitySpriteDraw(textureOutline, trailPos, null, trailColor, Projectile.oldRot[i], drawOrigin, Projectile.scale, spriteEffects);
            }

            Main.EntitySpriteDraw(texture, drawPos, texture.Frame(), drawColor, rotation, drawOrigin, Projectile.scale, spriteEffects);
            Main.EntitySpriteDraw(textureOutline, drawPos, null, drawColor with { A = 0 } * 0.1f, rotation, drawOrigin, Projectile.scale * 1.09f, spriteEffects);
            return false;
        }
    }
}
