using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace VoidPort.Common.Tiles
{
	//This class helps to skip some lines of code in the PreDraw() / PostDraw()
	public class TileGlobal : GlobalTile
	{
		public static Vector2 TileOffset => Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
		public static Vector2 TileCustomPosition(int i, int j, Vector2 off = default) => (new Vector2(i, j) * 16) - Main.screenPosition - off + TileOffset;
	}
}