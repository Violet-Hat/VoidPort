using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

using VoidPort.Common.Tiles;

namespace VoidPort.Content.Biomes.Void
{
    public class VoidBiome : ModBiome
    {
		public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
		public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<VoidBG>();
		public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Biomes/TheVoid");

		//Bestiary stuff
		public override string BestiaryIcon => "VoidPort/Content/Biomes/Void/VoidBiomeIcon";
		public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;
		
		public override void SpecialVisuals(Player player, bool isActive)
		{
			player.ManageSpecialBiomeVisuals("VoidPort:VoidSky", isActive, player.Center);
		}

		//The biome it's active only if the tiles are above 200
		public override bool IsBiomeActive(Player player)
		{
            bool tileCount = ModContent.GetInstance<TileCount>().VoidTiles >= 200;
            return tileCount;
		}
    }
}