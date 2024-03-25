using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Dusts;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Melee
{
    public class ShadowPikeProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.Size = new(18);
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1.25f;
            Projectile.ownerHitCheck = true;
            Projectile.aiStyle = -1;
        }

        public float AITimer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public float Counter
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public Player Player => Main.player[Projectile.owner];
        public float initialSpeed = 3f;
        public float forwardSpeed = 2.2f;
        public float retractSpeed = 3f;

        public override void AI()
        {
            Projectile.direction = Player.direction;

            float itemAnimMin = MathHelper.Min(Player.itemAnimation, Player.itemAnimationMax / 3);
            float num3 = Player.itemAnimation - itemAnimMin;
            float num7 = Player.itemAnimationMax - Player.itemAnimationMax / 3 - num3;
            float num8 = Player.itemAnimationMax / 3 - itemAnimMin;
            float progress = initialSpeed + forwardSpeed * num7 - retractSpeed * num8;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.PiOver4;
            Projectile.Center = Player.MountedCenter + Projectile.velocity * progress;


            AITimer++;

            if (Counter == 0f)
            {
                Counter = 1f;
                Projectile.netUpdate = true;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 2.5f, ModContent.ProjectileType<ShadowBolt>(), (int)(Projectile.damage * 1.25f), 0f, Projectile.owner);
                }
            }

            if (AITimer >= Player.itemAnimationMax)
            {
                Projectile.Kill();
            }

            SetPlayerValues();
            GenerateDust();
        }

        public void GenerateDust()
        {
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), Projectile.velocity.X, Projectile.velocity.Y, 100, Color.DarkSlateBlue, 1.5f);
        }
        public void SetPlayerValues()
        {
            Player.heldProj = Projectile.whoAmI;
            Player.MatchItemTimeToItemAnimation();
            Player.direction = Player.direction;
            //Projectile.Center = Player.RotatedRelativePoint(Player.MountedCenter);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteBatch spriteBatch = Main.spriteBatch;


            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Vector2.Zero, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.DarkSlateBlue with { A = 0 }, Projectile.rotation, Vector2.Zero, 1.375f, SpriteEffects.None, 0);
            return false;
        }
    }
}
