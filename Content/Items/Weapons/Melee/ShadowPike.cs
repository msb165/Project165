using Microsoft.Xna.Framework;
using Project165.Content.Projectiles.Melee;
using Terraria;
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
            Item.shootSpeed = 16f;
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

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(0.3f);
        }
    }
}
