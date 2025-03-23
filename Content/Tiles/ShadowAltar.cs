using Microsoft.Xna.Framework;
using Project165.Content.Items.SummonItems;
using Project165.Content.NPCs.Bosses.ShadowHand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Project165.Content.Tiles
{
    public class ShadowAltar : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileLighted[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            LocalizedText slimeAltarName = CreateMapEntryName();
            AddMapEntry(Color.Purple, slimeAltarName);
            MinPick = 210;
        }

        public override void MouseOver(int i, int j)
        {
            Main.LocalPlayer.cursorItemIconEnabled = true;
            Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<ShadowSlimeSummon>();
        }

        public override bool RightClick(int i, int j)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<ShadowHand>()) && Main.hardMode && NPC.downedGolemBoss && Main.LocalPlayer.HasItem(ModContent.ItemType<ShadowSlimeSummon>()))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.SpawnOnPlayer(Main.LocalPlayer.whoAmI, ModContent.NPCType<ShadowHand>());
                }
                else
                {
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, Main.LocalPlayer.whoAmI, ModContent.NPCType<ShadowHand>());
                }
            }
            return true;
        }
    }
}
