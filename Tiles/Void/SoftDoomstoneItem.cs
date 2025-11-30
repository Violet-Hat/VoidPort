using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VoidPort.Tiles.Void
{
	public class SoftDoomstoneItem : ModItem
	{
		public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 50;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SoftDoomstone>());
            Item.width = 16;
			Item.height = 16;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.Orange;
        }
	}
}