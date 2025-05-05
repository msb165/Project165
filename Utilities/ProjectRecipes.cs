using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;

namespace Project165.Utilities
{
    public class ProjectRecipes
    {
        public static void AddRecipes()
        {
            Recipe.Create(ItemID.Zenith)
                .AddIngredient(ItemID.TerraBlade)
                .AddIngredient(ItemID.Meowmere)
                .AddIngredient(ItemID.StarWrath)
                .AddIngredient(ItemID.InfluxWaver)
                .AddIngredient(ItemID.TheHorsemansBlade)
                .AddIngredient(ItemID.Seedler)
                .AddIngredient(ItemID.Starfury)
                .AddIngredient(ItemID.BeeKeeper)
                .AddIngredient(ItemID.Terragrim)
                .AddIngredient(ItemID.CopperShortsword)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            Recipe.Create(ItemID.SnowGlobe)
                .AddIngredient(ItemID.SnowBlock, 15)
                .AddRecipeGroup(RecipeGroupID.Wood, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }

        public static void PostAddRecipes()
        {
            /*foreach (Recipe r in Main.recipe)
            {
                if (r.HasResult(ItemID.Zenith))
                {                    
                    r.RemoveTile(TileID.MythrilAnvil);
                    r.AddTile(TileID.LunarCraftingStation);
                }
            }*/
        }
    }
}
