using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace VoidPort.Common
{
    public class Flags : ModSystem
    {
		//NPC positions
		public static Vector2 whereIsZero = Vector2.Zero;
		public static Vector2 oldLadyPrison = Vector2.Zero;
		
		//Bosses
		public static bool downedSagittarius = false;
		public static bool downedTechnoTruffle = false;
		public static bool downedOrthrusUltima = false;
		public static bool downedRetriever = false;
		public static bool downedZero = false;
		
		//Misc
		public static bool destroyPrison = false;
		public static bool freedOldLady = false;
		
		public override void ClearWorld()
		{
			//Bosses
			downedSagittarius = false;
			downedTechnoTruffle = false;
			downedOrthrusUltima = false;
			downedRetriever = false;
			downedZero = false;
			
			//Misc
			destroyPrison = false;
			freedOldLady = false;
		}
		
		public override void SaveWorldData(TagCompound tag)
		{
			//NPC positions
			tag[nameof(whereIsZero)] = whereIsZero;
			tag[nameof(oldLadyPrison)] = oldLadyPrison;
			
			//Bosses
			tag[nameof(downedSagittarius)] = downedSagittarius;
			tag[nameof(downedTechnoTruffle)] = downedTechnoTruffle;
			tag[nameof(downedOrthrusUltima)] = downedOrthrusUltima;
			tag[nameof(downedRetriever)] = downedRetriever;
			tag[nameof(downedZero)] = downedZero;
			
			//Misc
			tag[nameof(destroyPrison)] = destroyPrison;
			tag[nameof(freedOldLady)] = freedOldLady;
		}
		
		public override void LoadWorldData(TagCompound tag)
		{
			//NPC positions
			whereIsZero = tag.Get<Vector2>(nameof(whereIsZero));
			oldLadyPrison = tag.Get<Vector2>(nameof(oldLadyPrison));
			
			//Bosses
			downedSagittarius = tag.GetBool(nameof(downedSagittarius));
			downedTechnoTruffle = tag.GetBool(nameof(downedTechnoTruffle));
			downedOrthrusUltima = tag.GetBool(nameof(downedOrthrusUltima));
			downedRetriever = tag.GetBool(nameof(downedRetriever));
			downedZero = tag.GetBool(nameof(downedZero));
			
			//Misc
			destroyPrison = tag.GetBool(nameof(destroyPrison));
			freedOldLady = tag.GetBool(nameof(freedOldLady));
		}
		
		public override void NetSend(BinaryWriter writer)
		{
			//NPC positions
			writer.WriteVector2(whereIsZero);
			writer.WriteVector2(oldLadyPrison);
			
			//Downed bosses
			writer.WriteFlags(downedSagittarius, downedTechnoTruffle, downedOrthrusUltima, downedRetriever, downedZero);
			
			//Misc (old lady related)
			writer.WriteFlags(destroyPrison, freedOldLady);
		}
		
		public override void NetReceive(BinaryReader reader)
		{
			//NPC positions
			whereIsZero = reader.ReadVector2();
			oldLadyPrison = reader.ReadVector2();
			
			//Downed bosses
			reader.ReadFlags(out downedSagittarius, out downedTechnoTruffle, out downedOrthrusUltima, out downedRetriever, out downedZero);
			
			//Misc (old lady related)
			reader.ReadFlags(out destroyPrison, out freedOldLady);
		}
	}
}