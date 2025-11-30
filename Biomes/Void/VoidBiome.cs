using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

using VoidPort.Tiles.Void;
using VoidPort.Common;

namespace VoidPort.Biomes.Void
{
    public class VoidBiome : ModBiome
    {
		//Priority, Music, bestiary icon
		public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
		public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Biomes/TheVoid");
		//public override string BestiaryIcon => "VoidPort/Biomes/Void/VoidIcon";
		
		//The biome it's active only if the tiles are above 150
		public override bool IsBiomeActive(Player player)
		{
            bool tileCount = ModContent.GetInstance<TileCount>().VoidTiles >= 150;
            return tileCount;
		}
    }
}