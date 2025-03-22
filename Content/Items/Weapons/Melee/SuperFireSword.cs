using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Project165.Content.Projectiles.Melee;

namespace Project165.Content.Items.Weapons.Melee
{
    public class SuperFireSword : ModItem
    {
        public int currentAttack = 0;

        public override void SetDefaults()
        {
            Item.Size = new(44);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.damage = 190;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.shoot = ModContent.ProjectileType<SuperFireSwordHoldoutProj>();
            Item.shootSpeed = 1f;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.buyPrice(gold: 20);
            Item.knockBack = 8f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (currentAttack > 2)
            {
                currentAttack = 0;
            }
            int timeleft = currentAttack == 2 ? 90 : 30;
            Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, currentAttack, ai2: timeleft);
            currentAttack++;
            return false;
        }
    }
}
