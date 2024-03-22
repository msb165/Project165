using Microsoft.Xna.Framework;
using Project165.Content.Projectiles.Summon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Summon
{
    public class ShadowSlimeStaff : ModItem
    {
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
            Item.noMelee = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }
    }
}
