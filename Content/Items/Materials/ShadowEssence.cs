using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Project165.Content.Items.Materials
{
    public class ShadowEssence : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Type] = true;
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.Size = new(24);
            Item.rare = ItemRarityID.Yellow;
            Item.maxStack = Item.CommonMaxStack;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            Vector2 drawPos = Item.position - Main.screenPosition;
            float offset = Item.timeSinceItemSpawned / 240f + Main.GlobalTimeWrappedHourly * 0.04f;
            float globalTimeWrappedHourly = Main.GlobalTimeWrappedHourly % 4f / 2f;

            if (globalTimeWrappedHourly >= 1f)
            {
                globalTimeWrappedHourly = 2f - globalTimeWrappedHourly;
            }
            globalTimeWrappedHourly = globalTimeWrappedHourly * 0.5f + 0.5f;
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                spriteBatch.Draw(texture, drawPos + (Vector2.UnitY * 8f).RotatedBy((i + offset) * MathHelper.TwoPi) * globalTimeWrappedHourly, texture.Frame(), Color.Black * 0.25f, rotation, texture.Size() / 2, scale, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(texture, drawPos, texture.Frame(), Item.GetAlpha(lightColor), rotation, texture.Size() / 2, scale, SpriteEffects.None, 0f);

            for (float j = 0f; j < 1f; j += 0.34f)
            {
                spriteBatch.Draw(texture, drawPos + (Vector2.UnitY * 4f).RotatedBy((j + offset) * MathHelper.TwoPi) * globalTimeWrappedHourly, texture.Frame(), Color.Purple * 0.25f, rotation, texture.Size() / 2, 1.2f, SpriteEffects.None, 0f);
            }

            return false;
        }
    }
}
