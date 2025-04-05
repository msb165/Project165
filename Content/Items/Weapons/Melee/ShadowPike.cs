using Microsoft.Xna.Framework;
using Project165.Content.Projectiles.Melee;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Melee
{
    public class ShadowPike : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.Size = new(58);
            Item.damage = 100;
            Item.DamageType = DamageClass.Melee;
            Item.shoot = ModContent.ProjectileType<ShadowPikeProj>();
            Item.shootSpeed = 68f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.UseSound = SoundID.Item1 with { Pitch = -0.25f };
            Item.knockBack = 6f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float distortion = Main.rand.NextFloat() * Item.shootSpeed * 0.2f * player.direction;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, distortion);
            return false;
        }
    }
}
