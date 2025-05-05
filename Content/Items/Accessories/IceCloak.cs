using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Content.Items.Accessories
{
    public class IceCloak : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.Size = new(24);
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 1, silver: 40);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ProjectPlayer>().hasIceCloak = true;
            player.GetModPlayer<ProjectPlayer>().iceCloakItem = Item;
        }
    }
}
