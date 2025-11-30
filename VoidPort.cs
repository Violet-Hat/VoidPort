using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace VoidPort
{
	public class VoidPort : Mod
	{
		internal static VoidPort Instance;
		
		//For NPC management in case of subworlds
		internal Mod subworldLibrary = null;
		
		//NPC spawn
		public static int ZeroSpawnX;
		public static int ZeroSpawnY;
		
		internal static VoidPort mod;
		
		public VoidPort()
		{
			mod = this;
		}
		
		public override void Load()
		{
			Instance = this;
			
			ModLoader.TryGetMod("SubworldLibrary", out subworldLibrary);
		}
		
		public override void Unload()
		{
			subworldLibrary = null;
			
			mod = null;
		}
	}
}
