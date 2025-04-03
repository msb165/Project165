using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;
using Project165.Content.Items.Weapons.Magic;

namespace Project165.Common.Systems.World
{
    internal class WorldGenSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int microBiomesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));

            if (microBiomesIndex != -1)
            {
                tasks.Insert(microBiomesIndex + 1, new PassLegacy("Shadow Arena", (progress, config) =>
                {
                    progress.Message = "Project 165: Shadow Arena";
                    ShadowHandArena slimeArena = GenVars.configuration.CreateBiome<ShadowHandArena>();

                    Point origin2 = default;
                    for (int i = 0; i < 2; i++)
                    {
                        if (i == 0)
                        {
                            int j = 0;
                            while (j++ <= Main.maxTilesX)
                            {
                                origin2.X = GenVars.dungeonSide < 0 ? Main.maxTilesX - 250 : WorldGen.genRand.Next(250, (int)(Main.maxTilesX * 0.25));
                                origin2.Y = (int)GenVars.rockLayer + WorldGen.genRand.Next(300, 500);
                                if (slimeArena.Place(origin2, GenVars.structures))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }));
            }
        }

        public override void PostWorldGen()
        {
            int placedItems = 0;
            int maxItemsToPlace = 6;

            for (int i = 0; i < Main.maxChests; i++)
            {
                Chest chest = Main.chest[i];
                if (chest == null)
                {
                    continue;
                }

                Tile chestTile = Main.tile[chest.x, chest.y];
                if (chestTile.TileType == TileID.Containers && chestTile.TileFrameX == 13 * 36)
                {
                    if (WorldGen.genRand.NextBool(3))
                    {
                        continue;
                    }

                    for (int j = 0; j < Chest.maxItems; j++)
                    {
                        if (chest.item[j].type == ItemID.None)
                        {
                            chest.item[j].SetDefaults(ModContent.ItemType<StellarRod>());
                            placedItems++;
                            break;
                        }
                    }
                }

                if (placedItems >= maxItemsToPlace)
                {
                    break;
                }
            }
        }
    }
}
