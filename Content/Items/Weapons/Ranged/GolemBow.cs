using Microsoft.Xna.Framework;
using Project165.Content.Projectiles.Ranged;
using Project165.Utilites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Ranged
{
    public class GolemBow : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 58;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item5;
            Item.damage = 58;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Arrow;
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position + velocity * 2, velocity, type, damage, knockback, player.whoAmI);
            if (Main.rand.NextBool(2))
            {
                Projectile.NewProjectile(source, position + velocity * 2, velocity.RotatedByRandom(MathHelper.ToRadians(15f)), ModContent.ProjectileType<GolemArrow>(), damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            Project165Utils.SmoothHoldStyle(player);
        }
    }
}
