using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Project165.Content.Projectiles.Magic;


namespace Project165.Content.Items.Weapons.Magic;

public class AdamantiteStaff : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.Size = new(46);
        Item.mana = 8;
        Item.useAnimation = 30;
        Item.useTime = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.DamageType = DamageClass.Magic;
        Item.damage = 30;
        Item.rare = ItemRarityID.LightRed;
        Item.UseSound = SoundID.Item109;
        Item.shoot = ModContent.ProjectileType<AdamantiteBolt>();
        Item.shootSpeed = 14f;
        Item.noMelee = true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity = velocity.RotatedByRandom(MathHelper.ToRadians(15f));
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position + velocity * 4f, velocity, type, damage, knockback, player.whoAmI);
        return false;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.AdamantiteBar, 12)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}
