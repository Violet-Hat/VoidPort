using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VoidPort.Content.Tiles.Void
{
	public class SoftDoomstoneWallItem : ModItem
	{
		public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 50;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<SoftDoomstoneWall>());
            Item.width = 32;
			Item.height = 32;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.Orange;
        }
	}
}