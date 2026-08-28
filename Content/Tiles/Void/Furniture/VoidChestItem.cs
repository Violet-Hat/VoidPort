using Terraria.ModLoader;

namespace VoidPort.Content.Tiles.Void.Furniture
{
	public class VoidChestItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.DefaultToPlaceableTile(ModContent.TileType<VoidChest>());
		}
	}
}