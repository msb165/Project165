﻿using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Project165.Content.Items.Materials;

namespace Project165.Content.Items.SummonItems;

public class ShadowSlimeSummon : ModItem
{
    public override void SetDefaults()
    {
        Item.Size = new(38);
        Item.rare = ItemRarityID.Pink;
        Item.maxStack = 1;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<ShadowGel>(), 20)
            .AddIngredient(ItemID.LihzahrdPowerCell, 1)
            .AddIngredient(ItemID.WormFood)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<ShadowGel>(), 20)
            .AddIngredient(ItemID.LihzahrdPowerCell, 1)
            .AddIngredient(ItemID.BloodySpine)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}
