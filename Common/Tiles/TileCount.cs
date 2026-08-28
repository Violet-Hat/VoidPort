using System;
using Terraria.ModLoader;

using VoidPort.Content.Tiles.Void;

namespace VoidPort.Common.Tiles
{
	public class TileCount : ModSystem
	{
		//Counters
		public int VoidTiles { get; set; }
		public int ParthenonTiles { get; set; }
		
		//Reset the counters
		public override void ResetNearbyTileEffects()
        {
            VoidTiles = 0;
			ParthenonTiles = 0;
        }
		
		//Add to the counters
		public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
			VoidTiles = tileCounts[ModContent.TileType<Doomstone>()]
			+ tileCounts[ModContent.TileType<SoftDoomstone>()];
		}
	}
}