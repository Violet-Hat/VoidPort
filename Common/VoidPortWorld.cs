using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Chat;
using Microsoft.Xna.Framework;
using System;

using VoidPort.Biomes;
using VoidPort.NPCs.Boss.Zero;

namespace VoidPort.Common
{
	public class VoidPortWorld : ModSystem
	{
		//For NPC spawn management
		public bool IsInSubworld()
		{
			if(VoidPort.Instance.subworldLibrary == null)
			{
				return false;
			}
			
			foreach(Mod mod in ModLoader.Mods)
			{
				if(mod.Name.Equals(VoidPort.Instance.subworldLibrary.Name))
				{
					continue;
				}
				
				bool anySubworldForMod = (VoidPort.Instance.subworldLibrary.Call("AnyActive", mod) as bool?) ?? false;
				
				if(anySubworldForMod)
				{
					return true;
				}
			}
			
			return false;
		}
		
		public override void PostUpdateEverything()
		{
			if(!IsInSubworld())
			{
				//Spawn Zero aka Strange Machine
				if(!NPC.AnyNPCs(ModContent.NPCType<ZeroDeactivated>()) && Flags.whereIsZero != Vector2.Zero)
				{
					if(Main.netMode != NetmodeID.MultiplayerClient)
					{
						int strangeMachine = NPC.NewNPC(null, (int)Flags.whereIsZero.X, (int)Flags.whereIsZero.Y, ModContent.NPCType<ZeroDeactivated>());
						Main.npc[strangeMachine].position.X -= 7;
						Main.npc[strangeMachine].position.Y -= 4;
						
						if(Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.SyncNPC, number: strangeMachine);
						}
					}
				}
			}
		}
	}
}