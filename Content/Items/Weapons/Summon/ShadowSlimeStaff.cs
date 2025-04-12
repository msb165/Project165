using Microsoft.Xna.Framework;
using Project165.Buffs;
using Project165.Content.Projectiles.Summon;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Summon;

public class ShadowSlimeStaff : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
    }

    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 32;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.damage = 30;
        Item.DamageType = DamageClass.Summon;
        Item.shoot = ModContent.ProjectileType<ShadowSlimeProj>();
        Item.mana = 10;
        Item.knockBack = 4f;
        Item.shootSpeed = 5f;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.UseSound = SoundID.Item44;
        Item.rare = ItemRarityID.Yellow;
        Item.buffType = ModContent.BuffType<ShadowMinionBuff>();
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        player.AddBuff(Item.buffType, 2);
        Projectile proj = Projectile.NewProjectileDirect(source, Main.MouseWorld, velocity, type, damage, knockback, player.whoAmI);
        proj.originalDamage = Item.damage;
        return false;
    }
}
