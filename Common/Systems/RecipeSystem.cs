using Project165.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Project165.Common.Systems
{
    internal class RecipeSystem : ModSystem
    {
        public override void AddRecipes() => ProjectRecipes.AddRecipes();
        public override void PostAddRecipes() => ProjectRecipes.PostAddRecipes();
    }
}
