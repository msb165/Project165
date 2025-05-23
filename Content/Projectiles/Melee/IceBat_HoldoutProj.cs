﻿using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria;
using Project165.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;
using Terraria.Audio;
using Project165.Content.Dusts;


namespace Project165.Content.Projectiles.Melee;

public class IceBatHoldoutProj : ModProjectile
{
    public override string Texture => ModContent.GetInstance<IceBat>().Texture;

    public enum AttackState : int
    {
        SwingLeft = 0,
        SwingRight = 1
    }

    public AttackState CurrentAttack
    {
        get => (AttackState)Projectile.ai[0];
        set => Projectile.ai[0] = (float)value;
    }

    public float AITimer
    {
        get => Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 60;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.Size = new(24);
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.friendly = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.ownerHitCheck = true;
        Projectile.ownerHitCheckDistance = 300f;
        Projectile.noEnchantmentVisuals = true;
        Projectile.timeLeft = 25;
        Projectile.extraUpdates = 1;
    }

    public Player Player => Main.player[Projectile.owner];

    public float defaultScale = 2f;

    public override void AI()
    {
        float projRotation = CurrentAttack == AttackState.SwingLeft ? 2f : -2f;
        if (Projectile.localAI[0] == 0f)
        {
            Projectile.scale = Projectile.scale * Player.GetAdjustedItemScale(Player.HeldItem);
            Projectile.localAI[0] = 1f;
            SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack with { Pitch = -0.5f }, Projectile.position);
        }

        AITimer += 0.1f;
        Projectile.scale = defaultScale + MathF.Sin(AITimer) * 0.5f;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.SmoothStep(projRotation * Player.direction, -projRotation * Player.direction, AITimer * 0.3f);
        Projectile.Center = Player.Center + Projectile.rotation.ToRotationVector2() * 32f * Projectile.scale;
        Lighting.AddLight(Projectile.position, Color.LightCyan.ToVector3());

        for (int i = 0; i < 2; i++)
        {
            Dust.NewDustDirect(Projectile.Center + 32f * Projectile.rotation.ToRotationVector2() * Projectile.scale, 8, 8, ModContent.DustType<CloudDust>(), Scale: 0.5f);
        }

        SetPlayerValues();
    }

    public void SetPlayerValues()
    {
        float rotation = Projectile.rotation - MathHelper.PiOver2;
        Player.heldProj = Projectile.whoAmI;
        Player.itemRotation = Projectile.rotation;
        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Frostburn, 300);
        target.AddBuff(BuffID.Frostburn2, 300);
        target.AddBuff(BuffID.Weak, 300);
        if (Main.myPlayer == Projectile.owner)
        {
            Projectile.NewProjectileDirect(Projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<BlueFire>(), Projectile.damage / 2, 0f, Projectile.owner);
        }
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float collisionPoint = 0f;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - 48f * Projectile.rotation.ToRotationVector2(), Projectile.Center + 32f * Projectile.rotation.ToRotationVector2() * Projectile.scale, 16f, ref collisionPoint);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        Color drawColor = Projectile.GetAlpha(lightColor);
        Color trailColor = drawColor with { A = 127 };
        float rotation = Projectile.rotation + MathHelper.PiOver4;

        if (Projectile.spriteDirection == -1)
        {
            rotation += MathHelper.PiOver2;
        }

        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 oldDrawPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
            float oldRotation = Projectile.oldRot[i] + MathHelper.PiOver4;

            if (Projectile.spriteDirection == -1)
            {
                oldRotation += MathHelper.PiOver2;
            }

            trailColor *= 0.65f;
            Main.EntitySpriteDraw(texture, oldDrawPos, texture.Frame(), trailColor, oldRotation, texture.Size() / 2, Projectile.scale, spriteEffects);
        }

        Main.EntitySpriteDraw(texture, drawPos, texture.Frame(), drawColor, rotation, texture.Size() / 2, Projectile.scale, spriteEffects);
        return false;
    }
}
