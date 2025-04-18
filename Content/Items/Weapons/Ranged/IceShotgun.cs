﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Project165.Content.Dusts;
using Project165.Content.Projectiles.Ranged;
using System;
using Project165.Utilites;

namespace Project165.Content.Items.Weapons.Ranged;

public class IceShotgun : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 60;
        Item.height = 18;
        Item.shoot = ProjectileID.PurificationPowder;
        Item.shootSpeed = 10f;
        Item.useAmmo = AmmoID.Bullet;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.UseSound = SoundID.Item36 with { Pitch = -0.25f };
        Item.useAnimation = 42;
        Item.useTime = 42;
        Item.DamageType = DamageClass.Ranged;
        Item.damage = 35;
        Item.rare = ItemRarityID.Pink;
        Item.value = Item.sellPrice(gold: 4);
        Item.autoReuse = true;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (player.whoAmI == Main.myPlayer)
        {
            player.velocity -= velocity / 2;
        }
        
        for (int m = 0; m < 5; m++)
        {
            Vector2 offset = Vector2.UnitX.RotatedBy(velocity.ToRotation());
            Vector2 spawnPos = offset.RotatedByRandom(0.5f);
            Dust.NewDustPerfect(player.Center + spawnPos * 50f, ModContent.DustType<CloudDust>(), spawnPos - Vector2.UnitY * 4f, 100, Color.White with { A = 0 }, Main.rand.NextFloat(0.75f, 1.1f));
            Dust.NewDustDirect(player.Center, Item.width, Item.height, ModContent.DustType<CloudDust>(), 0, 0, 100, Color.White with { A = 0 }, Main.rand.NextFloat(0.75f, 1.1f));
        }

        Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<IceShockWave>(), damage / 2, knockback);

        for (int i = 0; i < 4; i++)
        {
            Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15f));
            newVelocity *= 1f - Main.rand.NextFloat(0.4f);

            Projectile.NewProjectileDirect(source, position, newVelocity * 2.5f, ProjectileID.CrystalShard, damage / 4, knockback);
            Projectile.NewProjectileDirect(source, position, newVelocity * 4f, ProjectileID.CrystalShard, damage / 4, knockback);

            Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback);
        }

        return false;
    }


    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        Project165Utils.RecoilEffect(player);
    }

    public override Vector2? HoldoutOffset() => new(-1.25f);
}
