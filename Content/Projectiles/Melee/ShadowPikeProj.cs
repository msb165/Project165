using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Dusts;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Melee;

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
        Projectile.extraUpdates = 1;
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
        float itemAnimation = Player.itemAnimation / (float)Player.itemAnimationMax;
        float completionTime = 1f - itemAnimation;
        Vector2 projPos = Player.RotatedRelativePoint(Player.MountedCenter);

        Projectile.Center = projPos;
        Projectile.direction = Projectile.spriteDirection = Player.direction;

        Vector2 spinningpoint = Vector2.UnitX.RotatedBy(MathHelper.Pi + completionTime * MathHelper.TwoPi) * new Vector2(velocityLength, AITimer);
        Projectile.position += spinningpoint.RotatedBy(velocityToRotation) + new Vector2(velocityLength + 22f, 0f).RotatedBy(velocityToRotation);
        Vector2 target = projPos + spinningpoint.RotatedBy(velocityToRotation) + (Vector2.UnitX * (velocityLength + 62f)).RotatedBy(velocityToRotation);

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
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity * 1.25f, Projectile.velocity * 0.3f, ModContent.ProjectileType<ShadowBolt>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner);
            }
        }

        if (Player.whoAmI == Main.myPlayer && Player.itemAnimation <= 2)
        {
            Projectile.Kill();
            Player.reuseDelay = 2;
        }

        SetPlayerValues();
        Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), 0, 0, 100, Color.DarkSlateBlue, 1.25f);
        dust.position = Projectile.Center + Projectile.velocity;
        dust.velocity *= 0f;
    }

    public void SetPlayerValues()
    {
        Player.heldProj = Projectile.whoAmI;
        //Player.SetDummyItemTime(2);
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
        spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.DarkSlateBlue with { A = 100 }, Projectile.rotation, Vector2.Zero, 1.35f, SpriteEffects.None, 0);
        return false;
    }
}
