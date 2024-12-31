using Project165.Buffs;
using Project165.Content.Projectiles.Summon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Project165.Content.Items.Weapons.Summon
{
    public class JellyfishStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 32;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.damage = 7;
            Item.DamageType = DamageClass.Summon;
            Item.shoot = ModContent.ProjectileType<JellyfishProj>();
            Item.mana = 10;
            Item.knockBack = 4f;
            Item.shootSpeed = 5f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item44;
            Item.rare = ItemRarityID.Yellow;
            Item.buffType = ModContent.BuffType<JellyfishBuff>();
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            Projectile proj = Projectile.NewProjectileDirect(source, Main.MouseWorld, velocity, type, damage, knockback, player.whoAmI, ai2:Main.rand.Next(0, 3));
            proj.originalDamage = Item.damage;
            return false;
        }
    }
}
