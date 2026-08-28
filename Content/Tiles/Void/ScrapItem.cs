using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VoidPort.Content.Tiles.Void
{
	public class ScrapItem : ModItem
	{
		public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 50;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Scrap>());
            Item.width = 22;
			Item.height = 22;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.Orange;
        }
	}
}