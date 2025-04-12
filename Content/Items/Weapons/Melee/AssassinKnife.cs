using Microsoft.Xna.Framework;
using Project165.Content.Projectiles.Melee;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Weapons.Melee
{
    public class AssassinKnife : ModItem
    {
        public override void SetDefaults()
        {
            Item.Size = new(40);
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = 7;
            Item.useAnimation = 21;
            Item.UseSound = SoundID.Item1 with { Pitch = 1.5f };
            Item.damage = 13;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.shoot = ModContent.ProjectileType<AssassinKnifeProj>();
            Item.shootSpeed = 3f;
            Item.knockBack = 5f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 30);
        }

        public override bool AltFunctionUse(Player player) => true;

        public bool shouldPlaySound = true;

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(shouldPlaySound);
            writer.Write(projAmount);
        }

        public override void NetReceive(BinaryReader reader)
        {
            shouldPlaySound = reader.ReadBoolean();
            projAmount = reader.ReadInt16();
        }

        public override void HoldItem(Player player)
        {
            if (shouldPlaySound && projAmount == 3 && player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.MaxMana with { Pitch = -0.5f });
                for (int i = 0; i < 5; i++)
                {
                    Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.RedTorch, 0f, 0f, Scale: Main.rand.Next(20, 26) * 0.1f);
                    dust.noLight = true;
                    dust.noGravity = true;
                    dust.velocity *= 0.5f;
                }
                shouldPlaySound = false;
            }
        }

        public int projAmount = 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 && projAmount > 0)
            {
                for (int i = 0; i < projAmount; i++)
                {
                    SoundEngine.PlaySound(SoundID.Item103);
                    Projectile.NewProjectile(source, position + velocity * 2f, (velocity * 2f).RotatedByRandom(MathHelper.ToRadians(15f)), ModContent.ProjectileType<AssassinBeam>(), damage, knockback, player.whoAmI, Main.rand.Next(-3, 3));
                }
                shouldPlaySound = true;
                projAmount = 0;
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(30f));
            }
        }
    }
}
