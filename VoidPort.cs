using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

using VoidPort.Content.Biomes.Void;

namespace VoidPort
{
	public class VoidPort : Mod
	{
		internal static VoidPort Instance;
		
		//For NPC management in case of subworlds
		internal Mod subworldLibrary = null;
		
		internal static VoidPort mod;
		
		public VoidPort()
		{
			mod = this;
		}
		
		public override void Load()
		{
			Instance = this;
			
			ModLoader.TryGetMod("SubworldLibrary", out subworldLibrary);
			
			if (Main.netMode != NetmodeID.Server)
			{
				Filters.Scene["VoidPort:VoidSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(1.0f, 0.15f, 0.15f).UseOpacity(0.1f), EffectPriority.VeryHigh);
                SkyManager.Instance["VoidPort:VoidSky"] = new VoidSky();
			}
		}
		
		public override void Unload()
		{
			subworldLibrary = null;
			
			mod = null;
		}
	}
}