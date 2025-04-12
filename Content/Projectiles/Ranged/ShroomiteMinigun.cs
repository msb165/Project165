using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Projectiles.Ranged;

public class ShroomiteMinigun : ModProjectile
{
    public override string Texture => ModContent.GetInstance<Items.Weapons.Ranged.ShroomiteMinigun>().Texture;
    public override void SetDefaults()
    {
        Projectile.Size = new(16);
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
    }

    Player Player => Main.player[Projectile.owner];

    public override void AI()
    {
        int ammoTouse = Player.HeldItem.useAmmo;
        bool canShoot = Player.channel && !Player.CCed && Player.HasAmmo(Player.HeldItem) && !Player.noItems;
        float KnockBack = Player.HeldItem.knockBack;

        Projectile.ai[0]++;
        if (Main.myPlayer == Projectile.owner && Projectile.ai[0] % Player.HeldItem.useAnimation == 0f)
        {
            Player.PickAmmo(Player.HeldItem, out int projToShoot, out float speed, out int Damage, out float knockBack, out int usedAmmoItemID);
            if (canShoot)
            {
                float shootSpeed = Player.HeldItem.shootSpeed * Projectile.scale;
                IEntitySource source = Player.GetSource_ItemUse_WithPotentialAmmo(Player.HeldItem, usedAmmoItemID);
                Vector2 spinPoint = (Main.MouseWorld - Player.RotatedRelativePoint(Player.MountedCenter)).SafeNormalize(-Vector2.UnitY) * shootSpeed;
                spinPoint = spinPoint.RotatedByRandom(MathHelper.ToRadians(5f));
                if (spinPoint.X != Projectile.velocity.X || spinPoint.Y != Projectile.velocity.Y)
                {
                    Projectile.netUpdate = true;
                }

                SoundEngine.PlaySound(SoundID.Item11, Projectile.position);
                Projectile.velocity = spinPoint;
                Vector2 spawnVel = Vector2.Normalize(Projectile.velocity) * speed;
                spawnVel = spawnVel.RotatedByRandom(MathHelper.ToRadians(15f));
                Projectile.NewProjectile(source, Projectile.Center + Vector2.UnitY * 4f + Projectile.velocity * 4f, spawnVel, projToShoot, Damage, KnockBack, Projectile.owner);
                if (Main.rand.NextBool(4))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Projectile.NewProjectile(source, Projectile.Center + Vector2.UnitY * 4f + Projectile.velocity * 5f, spawnVel.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.Next(1, 8), ProjectileID.Mushroom, Damage / 2, KnockBack, Projectile.owner);
                    }
                }
            }
            else
            {
                Projectile.Kill();
            }
        }
        Projectile.position = Player.MountedCenter - Projectile.Size / 2 - (Vector2.UnitX * 8f).RotatedBy(Projectile.velocity.ToRotation());
        Projectile.spriteDirection = Projectile.direction;
        Projectile.rotation = Projectile.velocity.ToRotation();
        SetPlayerValues();

        Dust dust = Dust.NewDustDirect(Projectile.position, 4, 4, DustID.Cloud, Alpha: 100, Scale: 1.75f);
        dust.position = Projectile.Center + Vector2.UnitY * 4f + Projectile.velocity * 7f;
        dust.velocity = (Vector2.UnitX * 4f * Main.rand.NextFloat(0.9f, 2f)).RotatedBy(Projectile.velocity.ToRotation()).RotatedByRandom(MathHelper.ToRadians(15f));
    }

    public void SetPlayerValues()
    {
        Player.heldProj = Projectile.whoAmI;
        Player.ChangeDir(Projectile.direction);
        Player.SetDummyItemTime(2);
        // Make it so it looks like the player is holding the weapon with both hands
        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
        Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Vector2 origin = new(0, texture.Height / 2 + -4f);
        if (Projectile.spriteDirection == -1)
        {
            origin.Y += 8f;
        }
        SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;

        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, origin, Projectile.scale, spriteEffects);
        return false;
    }
}
