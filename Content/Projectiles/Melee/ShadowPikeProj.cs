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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 19;
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

        public override void AI()
        {
            float velocityToRotation = Projectile.velocity.ToRotation();
            float velocityLength = Projectile.velocity.Length();
            Vector2 projPos = Player.RotatedRelativePoint(Player.MountedCenter);

            Projectile.Center = projPos;
            Projectile.direction = Projectile.spriteDirection = Player.direction;

            Vector2 spinningpoint = new Vector2(0.4f, 0.9f).RotatedBy(MathHelper.PiOver4 * AITimer * 0.34f * -Player.direction + 64f * Player.direction) * new Vector2(velocityLength, AITimer);
            Projectile.position += (Projectile.velocity * 4f) + spinningpoint.RotatedBy(velocityToRotation) + new Vector2(velocityLength + 44f, 0f).RotatedBy(velocityToRotation);
            Vector2 target = projPos + spinningpoint.RotatedBy(velocityToRotation) + new Vector2(velocityLength + 64f, 0f).RotatedBy(velocityToRotation);

            Projectile.rotation = projPos.AngleTo(target) + MathHelper.PiOver4 + MathHelper.PiOver2 * Player.direction;
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;
            }

            AITimer++;
            if (Counter == 0f)
            {
                Counter = 8f;
                Projectile.netUpdate = true;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity * AITimer, Projectile.velocity * 0.75f, ModContent.ProjectileType<ShadowBolt>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner);
                }
            }

            if (AITimer >= 20f)
            {
                Projectile.Kill();
            }

            SetPlayerValues();
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), Projectile.velocity.X * 0.8f, Projectile.velocity.Y * 0.8f, 100, Color.DarkSlateBlue, 1.25f);
        }
 
        public void SetPlayerValues()
        {
            Player.heldProj = Projectile.whoAmI;
            Player.SetDummyItemTime(2);
            //Player.MatchItemTimeToItemAnimation();
            Player.direction = Player.direction;
            Player.ChangeDir((Projectile.velocity.X > 0f).ToDirectionInt());
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
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.DarkSlateBlue with { A = 0 }, Projectile.rotation, Vector2.Zero, 1.35f, SpriteEffects.None, 0);
            return false;
        }
    }
}
