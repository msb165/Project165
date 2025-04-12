using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Ranged;

public class GravityKnives : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }

    public override void SetDefaults()
    {
        Item.width = 14;
        Item.height = 24;
        Item.shoot = ModContent.ProjectileType<Projectiles.Ranged.GravityKnives>();
        Item.shootSpeed = 10f;
        Item.knockBack = 5f;
        Item.useAnimation = 16;
        Item.useTime = 16;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.UseSound = SoundID.Item1;
        Item.damage = 60;
        Item.DamageType = DamageClass.Ranged;
        Item.autoReuse = true;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.buyPrice(silver: 50);
        Item.ArmorPenetration = 10;
        Item.consumable = true;
        Item.maxStack = Item.CommonMaxStack;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        for (int i = 0; i < 3; i++)
        {
            Vector2 newVelocity = velocity.RotatedBy(MathHelper.ToRadians(-1f + i * 2.5f));
            Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, player.whoAmI);
        }
        return false;
    }
}
