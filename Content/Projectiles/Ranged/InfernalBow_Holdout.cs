using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project165.Content.Dusts;
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

namespace Project165.Content.Projectiles.Ranged;

public class InfernalBow : ModProjectile
{
    public override string Texture => ModContent.GetInstance<Items.Weapons.Ranged.InfernalBow>().Texture;
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Projectile.friendly = true;
        Projectile.Size = new(16);
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
    }

    Player Player => Main.player[Projectile.owner];

    public override bool? CanDamage() => false;

    public override void AI()
    {
        bool canShoot = Player.HasAmmo(Player.HeldItem) && Player.controlUseItem && !Player.noItems && !Player.CCed;
        bool shouldShoot = false;
        int shootTimer = 24;

        Projectile.velocity = Vector2.Normalize(Main.MouseWorld - Projectile.Center);
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.ai[0]++;
        int multiplier = (int)MathHelper.Clamp(Projectile.ai[0], 0, 120) / 40;

        Projectile.ai[1]++;
        if (Projectile.ai[1] >= shootTimer - 6 * multiplier)
        {
            Projectile.ai[1] = 0f;
            shouldShoot = true;
        }

        if (shouldShoot && Main.myPlayer == Projectile.owner)
        {
            if (canShoot)
            {
                Player.PickAmmo(Player.HeldItem, out int shootType, out float speed, out int damage, out float knockback, out int useAmmoItemId);
                if (shootType == ProjectileID.WoodenArrowFriendly)
                {
                    shootType = ProjectileID.FireArrow;
                }

                SoundEngine.PlaySound(SoundID.Item5, Projectile.position);

                for (int i = 0; i < 15; i++)
                {
                    Vector2 newVel = Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / 15f);
                    Vector2 spawnPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * 32f;
                    Dust dust = Dust.NewDustPerfect(spawnPos, ModContent.DustType<GlowDust>(), newColor: Color.Orange, Scale: 0.75f);
                    dust.noGravity = true;
                    dust.velocity = newVel;
                }
                Vector2 projVelocity = Vector2.Normalize(Main.MouseWorld - Player.Center).RotatedByRandom(MathHelper.ToRadians(2.5f)) * Player.HeldItem.shootSpeed;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 rotatedVelocity = projVelocity * (0.6f + Main.rand.NextFloat() * 0.8f);

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Utils.RandomVector2(Main.rand, -15f, 15f) + rotatedVelocity * 2f, rotatedVelocity, shootType, damage, knockback, Projectile.owner);
                }
                if (Main.rand.NextBool(2))
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + projVelocity * 3f, projVelocity, ModContent.ProjectileType<InfernalArrow>(), damage, knockback, Projectile.owner);
                }
            }
            else
            {
                Projectile.Kill();
            }
        }

        SetProjectileValues();
        SetPlayerValues();            
    }

    public void SetPlayerValues()
    {
        Player.SetDummyItemTime(2);
        Player.ChangeDir(Projectile.direction);
        Player.heldProj = Projectile.whoAmI;
        Player.itemRotation = MathHelper.WrapAngle((Projectile.velocity * Projectile.direction).ToRotation());
    }

    public void SetProjectileValues()
    {
        Projectile.position = Player.RotatedRelativePoint(Player.MountedCenter, false, false) - Projectile.Size / 2;
        Projectile.spriteDirection = Projectile.direction;
        Projectile.timeLeft = 2;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
        Vector2 drawOrigin = new(0, texture.Height / 2);

        Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
        return false;
    }
}
