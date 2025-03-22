using Project165.Content.Items.SummonItems;
using Project165.Content.NPCs.Bosses.Frigus;
using Project165.Content.NPCs.Bosses.ShadowHand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Project165.Common.Systems
{
    public class ModIntegrationsSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            BossChecklistSupport();
        }

        public void BossChecklistSupport()
        {
            if(!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklist))
            {
                return;
            }
            string bossName = "Frigus";
            bossChecklist.Call(
                "LogBoss",
                Mod,
                bossName,
                11.1f,
                () => DownedBossSystem.downedFrigus,
                ModContent.NPCType<IceBossFly>(),
                new Dictionary<string, object>()
                {
                    ["spawnInfo"] = Language.GetOrRegister("Mods.Project165.BossChecklistSupport.Frigus.SpawnInfo"),
                    ["spawnItems"] = ModContent.ItemType<IceBossSummon>()
                }
            );

            bossName = "Hand of the Shadows";
            bossChecklist.Call(
                "LogBoss",
                Mod,
                bossName,
                11.1f,
                () => DownedBossSystem.downedShadowHand,
                ModContent.NPCType<ShadowHand>(),
                new Dictionary<string, object>()
                {
                    ["spawnInfo"] = Language.GetOrRegister("Mods.Project165.BossChecklistSupport.ShadowHand.SpawnInfo"),
                    ["spawnItems"] = ModContent.ItemType<IceBossSummon>()
                }
            );
        }
    }
}
