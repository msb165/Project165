using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Project165.Common.Globals.Tiles;

public class GlobalTileTweaks : GlobalTile
{
    public override bool CanDrop(int i, int j, int type)
    {
        int frameX = Main.tile[i, j].TileFrameX;
        if (type == TileID.LargePiles2 && Main.netMode != NetmodeID.MultiplayerClient)
        {
            if (frameX >= 918 && frameX <= 970)
            {
                if (Main.rand.NextBool(2))
                {
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ItemID.Terragrim);
                }
                else
                {
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ItemID.EnchantedSword);
                }
                return false;
            }
        }
        return true;
    }
}
