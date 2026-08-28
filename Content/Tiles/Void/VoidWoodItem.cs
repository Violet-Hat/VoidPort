using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VoidPort.Content.Tiles.Void
{
	public class VoidWoodItem : ModItem
	{
		public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 50;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<VoidWood>());
            Item.width = 16;
			Item.height = 16;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.Orange;
        }
	}
}